using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class ProcedureParameterExtensions
    {
        public static List<Parameter> GetParameters(this IEnumerable<ProcedureParameter> procedureParameters, ILogger logger, SchemaFile file)
        {
            var parameters = procedureParameters
                .Select(p => p.GetParameter(logger, file))
                .ToList();

            var newVariables = parameters.Select(x => x.Value).ToList();
            file.FileContext.Variables.Peek().AddRange(newVariables);

            return parameters;
        }

        private static Parameter GetParameter(this ProcedureParameter procedureParameter, ILogger logger, SchemaFile file)
        {
            var name = procedureParameter.VariableName.Value;
            var isNullable = procedureParameter.Nullable?.Nullable ?? false;
            var isReadOnly = procedureParameter.Modifier.Equals(ParameterModifier.ReadOnly);
            var hasDefaultValue = procedureParameter.Value != null;
            var isOutput = procedureParameter.Modifier == ParameterModifier.Output;
            var field = procedureParameter
                .DataType
                .GetField(name, isNullable, logger, file);

            field.Origin = OriginType.Parameter;
            
            if (field.Type == FieldType.UserDataType)
            {
                var referenceKey = procedureParameter.DataType.Name.GetQualifiedIdentfier(file);

                SchemaObject reference;
                if (file.Schema.ContainsKey(referenceKey))
                {
                    reference = file.Schema[referenceKey];
                }
                else
                {
                    logger.Log(LogLevel.Error, $"Unable to locate \"{referenceKey}\" schema object. Returning \"Unknown\".");

                    reference = new Unknown();
                }

                field = new TableReferenceField()
                {
                    Name = field.Name,
                    Type = field.Type,
                    IsNullable = field.IsNullable,
                    Reference = reference,
                };
            }

            return new Parameter(field)
            {
                IsReadOnly = isReadOnly,
                IsOutput = isOutput,
                HasDefaultValue = hasDefaultValue,
            };
        }
    }
}
