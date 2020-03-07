using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;
using Database.Core.Schema.Types;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class NamedTableReferenceExtensions
    {
        public static string GetTableReferenceIdentifier(this NamedTableReference tableReference)
        {
            var qualifiers = new List<string>() {
                tableReference.Alias?.Value,
                tableReference.SchemaObject.BaseIdentifier?.Value,
            };

            return qualifiers.GetQualifiedIdentfier();
        }

        public static SchemaObjectReference GetSchemaObjectReference(
            this NamedTableReference namedTableReference, 
            ILogger logger, 
            SchemaFile file
        )
        {
            var alias = namedTableReference.Alias?.Value;
            var referenceIdentifier = namedTableReference.SchemaObject.GetTemporaryQualifiedIdentfier();

            // try local context first
            var statementReference = file
                .FileContext
                .StatementReferences
                .SelectMany(x => x)
                .FirstOrDefault(x => x.Identifier.Equals(referenceIdentifier, StringComparison.InvariantCultureIgnoreCase));

            if (statementReference != null)
            {
                return new SchemaObjectReference()
                {
                    Alias = alias,
                    Identifier = referenceIdentifier,
                    Value = statementReference.Value,
                };
            }

            // temp tables defined in this file
            if (file.LocalSchema.ContainsKey(referenceIdentifier))
            {
                return new SchemaObjectReference()
                {
                    Alias = alias,
                    Identifier = referenceIdentifier,
                    Value = file.LocalSchema[referenceIdentifier],
                };
            }

            // objects created outside of this file
            referenceIdentifier = namedTableReference.SchemaObject.GetQualifiedIdentfier(file);
            if (file.Schema.ContainsKey(referenceIdentifier))
            {
                return new SchemaObjectReference()
                {
                    Alias = alias,
                    Identifier = referenceIdentifier,
                    Value = file.Schema[referenceIdentifier],
                };
            }

            // data modification statements can have target that is just an alias but it's captured as base identifier :-/
            if (namedTableReference.SchemaObject.Identifiers.Count == 1)
            {
                var tempAlias = namedTableReference.SchemaObject.BaseIdentifier.Value;
                statementReference = file
                    .FileContext
                    .StatementReferences
                    .SelectMany(x => x)
                    .FirstOrDefault(x => x.Alias != null && x.Alias.Equals(tempAlias, StringComparison.InvariantCultureIgnoreCase));

                if (statementReference != null)
                {
                    return statementReference;
                }
            }

            var columns = new List<Field>();
            // TODO : create a model for identifiers?
            var identifiers = referenceIdentifier.Split('.');

            // TODO : system tables and views, this could be handled by SQL files as well, might be easier to maintain
            // TODO : actually.. the best option might be to return object that behaves as a wildcard.. 
            // TODO : also, these were replaced by other system views and live in the sys schema? how so it works with dbo?
            switch (identifiers.Last().ToUpper())
            {
                case "SYSPROCESSES":
                case "SYSLOCKINFO":
                case "SPT_VALUES":
                case "OBJECTS":
                case "PARTITIONS":
                case "SYSOBJECTS":
                case "SYSCOLUMNS":
                case "SYSTYPES":
                case "TRIGGERS":
                case "PARTITION_SCHEMES":
                case "PARTITION_PARAMETERS":
                    columns = new List<Field>()
                    {
                        new WildCardField() { Origin = OriginType.SystemType },
                    };
                    break;

            }

            if (columns.Any())
            {
                var value = new Table()
                {
                    File = file,
                    Database = identifiers[0],
                    Schema = identifiers[1],
                    Identifier = identifiers[2],
                    Columns = columns,
                };

                return new SchemaObjectReference()
                {
                    Alias = alias,
                    Identifier = referenceIdentifier,
                    Value = value,
                };
            }

            logger.Log(
                LogLevel.Error, 
                LogType.MissingSchemaObject,
                file.Path,
                $"\"{referenceIdentifier}\" object is missing in the schema. Adding reference to \"Unknown\" type."
            );

            return new SchemaObjectReference()
            {
                Alias = alias,
                Identifier = referenceIdentifier,
                Value = new Unknown()
                {
                    Database = identifiers[0],
                    Schema = identifiers[1],
                    Identifier = identifiers[2],
                    File = file,
                },
            };
        }
    }
}
