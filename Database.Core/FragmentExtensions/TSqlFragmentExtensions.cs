using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Validation;

namespace Database.Core.FragmentExtensions
{
    public static class TSqlFragmentExtensions
    {
        public static string GetTokenText(this TSqlFragment fragment)
        {
            StringBuilder tokenText = new StringBuilder();

            for (int counter = fragment.FirstTokenIndex; counter <= fragment.LastTokenIndex; counter++)
            {
                tokenText.Append(fragment.ScriptTokenStream[counter].Text);
            }

            return tokenText.ToString();
        }

        public static string GetTokenText<TFragment>(this TFragment fragment) where TFragment : TSqlFragment
        {
            StringBuilder tokenText = new StringBuilder();

            for (int counter = fragment.FirstTokenIndex; counter <= fragment.LastTokenIndex; counter++)
            {
                tokenText.Append(fragment.ScriptTokenStream[counter].Text);
            }

            return tokenText.ToString();
        }

        public static ValidationResult ToValidationResult<TFragment>(this TFragment fragment) where TFragment : TSqlFragment
        {
            return fragment.ToValidationResult(string.Empty);
        }

        public static ValidationResult ToValidationResult<TFragment>(this TFragment fragment, string message) where TFragment : TSqlFragment
        {
            return new ValidationResult()
            {
                Fragment = fragment,
                Message = message
            };
        }

        public static IList<ValidationResult> ToValidationResults<TFragment>(this IEnumerable<TFragment> fragments) where TFragment : TSqlFragment
        {
            return fragments.ToValidationResults(string.Empty);
        }

        public static IList<ValidationResult> ToValidationResults<TFragment>(this IEnumerable<TFragment> fragments, string message) where TFragment : TSqlFragment
        {
            return fragments
                .Select(f => f.ToValidationResult(message))
                .ToList();
        }
    }
}
