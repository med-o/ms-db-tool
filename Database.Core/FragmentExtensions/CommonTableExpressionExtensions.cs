using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Contextes;
using Database.Core.Schema.References;
using Database.Core.Schema.Types;

namespace Database.Core.FragmentExtensions
{
    public static class CommonTableExpressionExtensions
    {
        public static IList<SchemaObjectReference> GetSchemaObjectReferences(
            this IEnumerable<CommonTableExpression> commonTableExpressions,
            ILogger logger,
            SchemaFile file
        )
        {
            var schemaObjectReferences = new List<SchemaObjectReference>();

            foreach (var commonTableExpression in commonTableExpressions)
            {
                using (new StatementContext(file.FileContext, schemaObjectReferences))
                {
                    // current CTE can be used by subsequent CTE, get reference and next iteration will push it onto the stack
                    var schemaObjectReference = commonTableExpression.GetSchemaObjectReference(logger, file);
                    schemaObjectReferences.Add(schemaObjectReference);
                }
            }

            return schemaObjectReferences;
        }

        public static SchemaObjectReference GetSchemaObjectReference(
            this CommonTableExpression commonTableExpression,
            ILogger logger,
            SchemaFile file
        )
        {
            var cteColumns = commonTableExpression
                .QueryExpression
                .GetFields(logger, file);

            if (commonTableExpression.Columns.Any())
            {
                for (var i = 0; i < commonTableExpression.Columns.Count(); i++)
                {
                    cteColumns[i].Name = commonTableExpression.Columns[i].Value;
                }
            }

            var cte = new Cte()
            {
                Identifier = commonTableExpression.ExpressionName.Value,
                File = file,
                Columns = cteColumns,
            };

            var cteReference = new SchemaObjectReference()
            {
                Alias = commonTableExpression.ExpressionName.Value,
                Identifier = cte.GetQualifiedIdentfier(),
                Value = cte,
            };

            return cteReference;
        }

        public static IList<FieldPairReference> GetFieldPairReferences(
            this IList<CommonTableExpression> commonTableExpressions,
            ILogger logger,
            SchemaFile file
        )
        {
            var pairs = new List<FieldPairReference>();
            var schemaObjectReferences = new List<SchemaObjectReference>();

            foreach (var commonTableExpression in commonTableExpressions)
            {
                using (new StatementContext(file.FileContext, schemaObjectReferences))
                {
                    var ctePairs = commonTableExpression
                        .QueryExpression
                        .GetFieldPairReferences(logger, file);

                    pairs.AddRange(ctePairs);

                    // current CTE can be used by subsequent CTE, get reference and next iteration will push it onto the stack
                    var schemaObjectReference = commonTableExpression.GetSchemaObjectReference(logger, file);
                    schemaObjectReferences.Add(schemaObjectReference);
                }
            }

            return pairs;
        }
    }
    }
