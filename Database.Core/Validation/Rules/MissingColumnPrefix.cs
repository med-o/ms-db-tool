using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;

namespace Database.Core.Validation.Rules
{
    public class MissingColumnPrefix : ValidationRule<ColumnReferenceExpression>
    {
        public MissingColumnPrefix(ILogger logger) : base(logger)
        {
        }

        public override ValidationRuleType Type => ValidationRuleType.MissingColumnPrefix;

        public override IList<ValidationResult> Execute(SchemaFile file)
        {
            
            return Fragments
                .Where(ScalarExpressionIsNotPrefixed)
                .ToValidationResults();
        }

        private static bool ScalarExpressionIsNotPrefixed(ColumnReferenceExpression columnReferenceExpression)
        {
            return columnReferenceExpression.ColumnType == ColumnType.Regular &&
                   !columnReferenceExpression.MultiPartIdentifier.GetTokenText().Contains(".");
        }
    }
}
