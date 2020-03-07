using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Validation.Rules
{
    public class ImplicitConversionInSetVariableStatement : ImplicitConversionRuleBase<SetVariableStatement>
    {
        public ImplicitConversionInSetVariableStatement(ILogger logger) : base(logger)
        {
        }

        protected override IEnumerable<FieldPairReference> GetFieldPairReferences(SchemaFile file, SetVariableStatement setVariableStatement)
        {
            var variable = setVariableStatement.Variable.GetField(null, Logger, file);
            var value = (Field)null;

            if (setVariableStatement.FunctionCallExists)
            {
                var functionName = setVariableStatement.Identifier.Value;
                value = new DefaultField()
                {
                    Type = FieldType.Xml, // TODO : can this only be xml?
                    Origin = OriginType.FunctionReturn,
                    IsNullable = false,
                };
            }
            else
            {
                value = setVariableStatement.Expression.GetField(null, Logger, file);
            }

            return new List<FieldPairReference>()
            {
                new FieldPairReference()
                {
                    Fragment = setVariableStatement,
                    Left = variable,
                    Right = value,
                }
            };
        }

        protected override string GetValidationMessage(FieldPairReference pair)
        {
            return $"{typeof(SetVariableStatement).Name}: Variable \"{pair.Left.Name}\" is of \"{pair.Left.Type}\" type and expression is of \"{pair.Right.Type}\" type.";
        }
    }
}
