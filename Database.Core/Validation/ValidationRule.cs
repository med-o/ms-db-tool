using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Validation.Settings;
using Database.Core.Visitors;

namespace Database.Core.Validation
{
    public abstract class ValidationRule<TFragment> : TSqlFragmentVisitor<TFragment>, IValidationRule
        where TFragment : TSqlFragment
    {
        public ValidationRule(ILogger logger)
        {
            Logger = logger;

            Settings = new ValidationRuleSettings()
            {
                Enabled = false,
                Type = ValidationRuleType.NotSpecified,
                Level = LogLevel.Error,
            };
        }

        public ValidationRuleSettings Settings { get; private set; }
        public abstract ValidationRuleType Type { get; }
        protected ILogger Logger { get; }

        public IValidationRule Configure(ValidationRuleSettings settings)
        {
            Settings = settings;
            return this;
        }

        // TODO : should be protected
        public abstract IList<ValidationResult> Execute(SchemaFile file);

        public IList<ValidationResult> Validate(SchemaFile file) 
        {
            if (IsConfigured() && Settings.Enabled)
            {
                // TODO : pick implementation that will be easier to debug/understand

                var results = new List<ValidationResult>();
                foreach (var batch in file.TsqlScript.Batches)
                {
                    foreach (var statement in batch.Statements)
                    {
                        var partialResults = ValidateStatement(statement, file);
                        results.AddRange(partialResults);
                    }
                }

                // TODO : make sure this doesn't pollute the scope
                //var results = file
                //    .TsqlScript
                //    .Batches
                //    .SelectMany(x => x.Statements)
                //    .SelectMany(x => ValidateStatement(x, file))
                //    .ToList();

                return results;
            }
            
            return new List<ValidationResult>(); 
        }

        private List<ValidationResult> ValidateStatement(TSqlStatement statement, SchemaFile file)
        {
            Fragments = new List<TFragment>();

            var statementReferences = statement
                .CollectLocalSchema(Logger, file)
                .ToList();

            statement.Accept(this);

            var result = Execute(file)
                .Where(vr => vr != null)
                .ToList();

            return result;
        }

        private bool IsConfigured()
        {
            if (Settings.Type.Equals(ValidationRuleType.NotSpecified))
            {
                Logger.Log(LogLevel.Error, $"Attempting to call validation on a rule that is not configured: {this.GetType().Name}");
                return false;
            }
            return true;
        }
    }
}
