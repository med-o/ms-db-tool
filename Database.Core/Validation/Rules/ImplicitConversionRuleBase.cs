using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Validation.Rules
{
    public abstract class ImplicitConversionRuleBase<TFragment> : ValidationRule<TFragment>
        where TFragment : TSqlFragment
    {
        protected ImplicitConversionRuleBase(ILogger logger) : base(logger)
        {
        }

        public override ValidationRuleType Type => ValidationRuleType.ImplicitConversion;

        public override IList<ValidationResult> Execute(SchemaFile file)
        {
            var validationResults = Fragments
                .SelectMany(fragment => GetFieldPairReferences(file, fragment))
                .Where(pair => InvokesImplicitConversion(pair))
                .Select(pair => ToValidationResult(pair))
                .ToList();

            return validationResults;
        }

        protected abstract IEnumerable<FieldPairReference> GetFieldPairReferences(SchemaFile file, TFragment fragment);

        protected virtual string GetValidationMessage(FieldPairReference pair)
        {
            return $"{typeof(TFragment).Name}: First column is of \"{pair.Left.Type}\" type and second column is of \"{pair.Right.Type}\" type.";
        }

        private static bool InvokesImplicitConversion(FieldPairReference x)
        {
            // TODO : flag conversion between varchars and decimals with different params?
            // TODO : separate conditions for better context
            return x.Left.Type != FieldType.NotSpecified
                && x.Right.Type != FieldType.NotSpecified
                && x.Left.Type != FieldType.WildCard
                && x.Right.Type != FieldType.WildCard
                && x.Right.Type != FieldType.Null
                && x.Left.Origin != OriginType.Literal
                && x.Right.Origin != OriginType.Literal
                && x.Left.Origin != OriginType.SystemType
                && x.Right.Origin != OriginType.SystemType
                && x.Left.Type != x.Right.Type;
        }

        private ValidationResult ToValidationResult(FieldPairReference pair)
        {
            return new ValidationResult()
            {
                Fragment = pair.Fragment,
                Message = GetValidationMessage(pair)
            };
        }
    }
}
