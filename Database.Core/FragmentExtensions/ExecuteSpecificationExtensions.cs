using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class ExecuteSpecificationExtensions
    {
        public static IList<FieldPairReference> GetFieldPairReferences(
            this ExecuteSpecification executeSpecification,
            ILogger logger,
            SchemaFile file
        )
        {
            var isSystemObject = executeSpecification
                .ExecutableEntity
                .GetSchema(logger, file)
                .Schema
                .Equals(SchemaObject.SystemSchema, StringComparison.InvariantCultureIgnoreCase);

            if (isSystemObject) // ignore system procs for now, we don't store their representation
            {
                return new List<FieldPairReference>();
            }

            var inputParameters = executeSpecification
                .ExecutableEntity
                .GetParameters(logger, file)
                .ToList();

            if (!inputParameters.Any()) // missing schema object reference
            {
                return new List<FieldPairReference>();
            }

            // TODO : this needs to be able to handle DEFAULT keyword

            var sourceFieldReferences = executeSpecification
                .ExecutableEntity
                .Parameters
                .Select(p => new
                {
                    Fragment = p,
                    Reference = new FieldReference()
                    {
                        Alias = string.Empty,
                        Identifier = p.Variable?.Name ?? string.Empty,
                        Value = p.ParameterValue.GetField(p.Variable?.Name, logger, file)
                    }
                })
                .ToList();

            var pairs = sourceFieldReferences
                .Where(x => string.IsNullOrWhiteSpace(x.Reference.Identifier))
                .Select((fieldReference, index) =>
                {
                    Field parameterValue = null;

                    if (index < inputParameters.Count)
                    {
                        parameterValue = inputParameters[index].Value;
                    }
                    else
                    {
                        logger.Log(LogLevel.Warning, 
                            LogType.TooManyParameters,
                            file.Path,
                            $"Executable entity specifies more than \"{inputParameters.Count}\" parameter(s).");
                        parameterValue = fieldReference.Reference.Value;
                    }

                    return new FieldPairReference()
                    {
                        Left = parameterValue,
                        Right = fieldReference.Reference.Value,
                        Fragment = fieldReference.Fragment
                    };
                })
                .ToList();

            var parametersMap = inputParameters.ToDictionary(k => k.Value.Name, v => v.Value, StringComparer.InvariantCultureIgnoreCase);
            var mappedPairs = sourceFieldReferences
                .Where(x => !string.IsNullOrWhiteSpace(x.Reference.Identifier))
                .Select(unorderedReference => {

                    Field parameter = null;

                    if (parametersMap.ContainsKey(unorderedReference.Reference.Identifier))
                    {
                        parameter = parametersMap[unorderedReference.Reference.Identifier];
                    }
                    else
                    {
                        logger.Log(LogLevel.Warning, 
                            LogType.InvalidParameterIndetifier,
                            file.Path,
                            $"Target procedure doesn't have \"{unorderedReference.Reference.Identifier}\" parameter.");
                        parameter = unorderedReference.Reference.Value;
                    }

                    return new FieldPairReference()
                    {
                        Left = parameter,
                        Right = unorderedReference.Reference.Value,
                        Fragment = unorderedReference.Fragment
                    };
                })
                .ToList();

            return pairs
                .Concat(mappedPairs)
                .ToList();
        }
    }
}
