using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;

namespace Database.Core.Validation.Rules
{
    /// <summary>
    /// Detects leading wildcard in like expression
    /// https://blogs.msdn.microsoft.com/arvindsh/2013/03/27/considerations-when-using-the-transactsql-scriptdom-parsers/
    /// </summary>
    public class NoLeadingWildcard : ValidationRule<LikePredicate>
    {
        public NoLeadingWildcard(ILogger logger) : base(logger)
        {
        }

        public override ValidationRuleType Type => ValidationRuleType.NoLeadingWildcard;

        public override IList<ValidationResult> Execute(SchemaFile file)
        {
            return Fragments
                .Where(x => CheckForStringLiteral(x) || CheckForBinaryExpression(x))
                .ToValidationResults();
        }

        private static bool CheckForStringLiteral(LikePredicate node)
        {
            if (node.SecondExpression is StringLiteral stringLiteral)
            {
                if (stringLiteral.Value.StartsWith("%"))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckForBinaryExpression(LikePredicate node)
        {
            // TODO : this incorrectly reports on fragment "LIKE @temp + '%'"
            if (node.SecondExpression is BinaryExpression binaryExpression)
            {
                if (binaryExpression.SecondExpression is StringLiteral stringLiteral)
                {
                    if (stringLiteral.Value.StartsWith("%"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
