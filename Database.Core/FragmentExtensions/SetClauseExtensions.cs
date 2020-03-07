using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class SetClauseExtensions 
    {
        public static IList<FieldPairReference> GetFieldPairs(
            this IList<SetClause> setClauses,
            ILogger logger,
            SchemaFile file
        )
        {
            var fieldPairs = new List<FieldPairReference>();

            foreach (var setClause in setClauses)
            {
                if (setClause is AssignmentSetClause assignmentSetClause)
                {
                    var newValue = assignmentSetClause.NewValue.GetField(null, logger, file);
                    var assignedField = assignmentSetClause.Column?.GetField(null, logger, file)
                        ?? assignmentSetClause.Variable.GetField(null, logger, file);

                    fieldPairs.Add(new FieldPairReference() { Left = assignedField, Right = newValue, Fragment = assignmentSetClause });
                }
                else
                {
                    logger.Log(LogLevel.Warning, 
                        LogType.NotSupportedYet,
                        file.Path, 
                        $"\"{setClause.GetType()}\" set clause type is not supported yet.");
                }
            }

            return fieldPairs;
        }
    }
}
