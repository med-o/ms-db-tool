using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Contextes;
using Database.Core.Schema.References;
using Database.Core.Schema.Types;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class TableReferenceExtensions
    {
        public static IEnumerable<SchemaObjectReference> GetSchemaObjectReferences(
            this IEnumerable<TableReference> tableReferences, 
            ILogger logger, 
            SchemaFile file
        )
        {
            return tableReferences
                .SelectMany(x => x.GetSchemaObjectReferences(logger, file))
                .ToList();
        }

        public static IEnumerable<SchemaObjectReference> GetSchemaObjectReferences(
            this TableReference tableReference, 
            ILogger logger, 
            SchemaFile file
        )
        {
            switch (tableReference)
            {
                case JoinTableReference joinTableReference:
                    // NOTE : handles both, qualified and unqualified joins
                    return joinTableReference.GetSchemaObjectReferences(logger, file);

                case NamedTableReference namedReference:
                    return new List<SchemaObjectReference>() {
                        namedReference.GetSchemaObjectReference(logger, file)
                    };

                case QueryDerivedTable queryDerivedTable:
                    {
                        var queryDerivedTableColumns = queryDerivedTable
                            .QueryExpression
                            .GetFields(logger, file);

                        if (queryDerivedTable.Columns.Any())
                        {
                            for (int i = 0; i < queryDerivedTable.Columns.Count(); i++)
                            {
                                queryDerivedTableColumns[i].Name = queryDerivedTable.Columns[i].Value;
                            }
                        }

                        var derivedTable = new DerivedTable()
                        {
                            Identifier = queryDerivedTable.Alias.Value, // TODO : do they have a name?
                            File = file,
                            Columns = queryDerivedTableColumns,
                        };

                        var identifier = derivedTable.GetQualifiedIdentfier();

                        // TODO : do I need to add it to local schema? why?
                        file
                            .LocalSchema
                            .Add(new KeyValuePair<string, SchemaObject>(identifier, derivedTable));

                        return new List<SchemaObjectReference>()
                        {
                            new SchemaObjectReference()
                            {
                                Alias = queryDerivedTable.Alias.Value,
                                Identifier = identifier,
                                Value = derivedTable,
                            }
                        };
                    }

                case InlineDerivedTable inlineDerivedTable:

                    var inlineTableColumns = inlineDerivedTable
                            .RowValues
                            .First()
                            .ColumnValues
                            .Select(c => c.GetField("", logger, file))
                            .ToList();

                    if (inlineDerivedTable.Columns.Any())
                    {
                        for (int i = 0; i < inlineDerivedTable.Columns.Count(); i++)
                        {
                            inlineTableColumns[i].Name = inlineDerivedTable.Columns[i].Value;
                        }
                    }

                    var inlineTable = new DerivedTable()
                    {
                        Identifier = inlineDerivedTable.Alias.Value, // TODO : do they have a name?
                        File = file,
                        Columns = inlineTableColumns,
                    };

                    return new List<SchemaObjectReference>()
                    {
                        new SchemaObjectReference()
                        {
                            Alias = inlineDerivedTable.Alias.Value,
                            Identifier = inlineTable.GetQualifiedIdentfier(),
                            Value = inlineTable,
                        }
                    };

                case VariableTableReference variableTableReference:
                    {
                        // TODO : wrap in GetVariable()
                        var name = variableTableReference.Variable.Name;
                        var variable = (TableReferenceField)file
                            .FileContext
                            .Variables
                            .SelectMany(x => x)
                            .Distinct(new KeyEqualityComparer<Field, string>(x => x.Name))
                            .First(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                        return new List<SchemaObjectReference>()
                        {
                            new SchemaObjectReference()
                            {
                                Alias = variableTableReference.Alias?.Value,
                                Identifier = variable.Reference.GetQualifiedIdentfier(),
                                Value = variable.Reference,
                            }
                        };
                    }

                case SchemaObjectFunctionTableReference schemaObjectFunctionTableReference:
                    {
                        // TODO : add support for XML handling
                        // "col.nodes('entry') y(row)"
                        // schema.base(parameter) alias(column)

                        var qualifiedIdentifier = schemaObjectFunctionTableReference.SchemaObject.GetQualifiedIdentfier(file);
                        var tempQualifiedIdentifier = schemaObjectFunctionTableReference.SchemaObject.GetTemporaryQualifiedIdentfier();

                        SchemaObject reference;
                        if (file.Schema.ContainsKey(qualifiedIdentifier))
                        {
                            reference = file.Schema[qualifiedIdentifier];
                        }
                        else if (file.LocalSchema.ContainsKey(tempQualifiedIdentifier))
                        {
                            reference = file.LocalSchema[tempQualifiedIdentifier];
                        }
                        else
                        {
                            // TODO : it doesn't have to be just XML?
                            // TODO : columns can be null
                            var functionColumns = schemaObjectFunctionTableReference
                                .Columns
                                .Select(x => new DefaultField()
                                {
                                    Name = x.Value,
                                    Type = FieldType.Xml,
                                    IsNullable = false,
                                })
                                .Cast<Field>()
                                .ToList();

                            reference = new DerivedTable()
                            {
                                Columns = functionColumns,
                                File = file,
                                Identifier = schemaObjectFunctionTableReference.SchemaObject.BaseIdentifier.Value,
                                Database = SchemaObject.MasterDb,
                                Schema = schemaObjectFunctionTableReference.SchemaObject.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                            };
                        }

                        return new List<SchemaObjectReference>()
                        {
                            new SchemaObjectReference()
                            {
                                Alias = schemaObjectFunctionTableReference.Alias?.Value,
                                Identifier = reference.GetQualifiedIdentfier(),
                                Value = reference,
                            }
                        };
                    }

                case OpenJsonTableReference openJsonTableReference:
                {
                    var f = openJsonTableReference.Variable.GetField(null, logger, file);

                    var columns = openJsonTableReference
                        .SchemaDeclarationItems
                        .Select(declarationItem =>
                            {
                                var column = declarationItem
                                    .ColumnDefinition
                                    .DataType
                                    .GetField(declarationItem.ColumnDefinition.ColumnIdentifier.Value, false, logger, file);
                                column.Origin = OriginType.SystemType;
                                return column;
                            })
                        .ToList();

                    var jsonTable = new DerivedTable()
                    {
                        Columns = columns,
                        File = file,
                        Identifier = $"{f.Name}-openjson",
                        Database = SchemaObject.TempDb,
                        Schema = SchemaObject.DefaultSchema,
                    };

                    return new List<SchemaObjectReference>()
                    {
                        new SchemaObjectReference()
                        {
                            Alias = openJsonTableReference.Alias?.Value,
                            Identifier = jsonTable.GetQualifiedIdentfier(),
                            Value = jsonTable,
                        }
                    };
                }

                case VariableMethodCallTableReference variableMethodCallTableReference:
                    {
                        // TODO : find out how this really works in SQL
                        // FROM	@delivery_xml.nodes('/delivery/fixedPrices') AS x(col) .. (fn_get_highest_shipping_price.udf)
                        
                        //variableMethodCallTableReference.MethodName; // nodes
                        //variableMethodCallTableReference.Parameters; // '/delivery/fixedPrices'
                        
                        var columns = variableMethodCallTableReference
                            .Columns
                            .Select(x => new DefaultField()
                            {
                                Name = x.Value,
                                Type = FieldType.Xml,
                                IsNullable = false,
                            })
                            .Cast<Field>()
                            .ToList();

                        var variableMethodCallTable = new DerivedTable() {
                            Columns = columns,
                            File = file,
                            Identifier = variableMethodCallTableReference.Variable.Name,
                            Database = SchemaObject.TempDb,
                            Schema = SchemaObject.DefaultSchema,
                        };

                        return new List<SchemaObjectReference>()
                        {
                            new SchemaObjectReference()
                            {
                                Alias = variableMethodCallTableReference.Alias?.Value,
                                Identifier = variableMethodCallTable.GetQualifiedIdentfier(),
                                Value = variableMethodCallTable,
                            }
                        };
                    }

                case JoinParenthesisTableReference joinParenthesisTableReference:
                    return joinParenthesisTableReference
                        .Join
                        .GetSchemaObjectReferences(logger, file);

                case BuiltInFunctionTableReference builtInFunctionTableReference:

                    SchemaObject value = null;

                    switch (builtInFunctionTableReference.Name.Value)
                    {
                        case "fn_virtualfilestats":
                            value = new Table()
                            {
                                File = file,
                                Database = SchemaObject.MasterDb,
                                Schema = SchemaObject.SystemSchema,
                                Identifier = builtInFunctionTableReference.Name.Value,
                                Columns = new List<Field>()
                                {
                                    // https://docs.microsoft.com/en-us/sql/relational-databases/system-functions/sys-fn-virtualfilestats-transact-sql?view=sql-server-2017
                                    new DefaultField() { Name = "DbId", Type = FieldType.SmallInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "FileId", Type = FieldType.SmallInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "TimeStamp", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "NumberReads", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "BytesRead", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "IoStallReadMS", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "NumberWrites", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "BytesWritten", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "IoStallWriteMS", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "IoStallMS", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "FileHandle", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                    new DefaultField() { Name = "BytesOnDisk", Type = FieldType.BigInt, Origin = OriginType.FunctionReturn  },
                                },
                            };
                            break;
                    }

                    return new List<SchemaObjectReference>()
                    {
                        new SchemaObjectReference()
                        {
                            Alias = builtInFunctionTableReference.Alias?.Value,
                            Identifier = value.GetQualifiedIdentfier(),
                            Value = value,
                        }
                    };

                case FullTextTableReference fullTextTableReference:
                    var fullTextTableIdentifier = fullTextTableReference
                        .TableName
                        .GetQualifiedIdentfier(file);

                    var fullTextTableSource = file.Schema.ContainsKey(fullTextTableIdentifier)
                        ? file.Schema[fullTextTableIdentifier]
                        : new Unknown() {
                            File = file,
                            Database = "",
                            Schema = "",
                            Identifier = "",

                        };
                    
                    var newReferences = new List<SchemaObjectReference>()
                    {
                        new SchemaObjectReference()
                        {
                            Alias = null,
                            Identifier = fullTextTableSource.GetQualifiedIdentfier(),
                            Value = fullTextTableSource,
                        }
                    };

                    // TODO : or should these be added to current scope instead of pushing new scope?
                    using (new StatementContext(file.FileContext, newReferences))
                    {
                        var keyColumn = fullTextTableSource
                            .Columns
                            .FirstOrDefault(x => x.HasIdentity)
                            ?.Copy("KEY")
                            ?? new DefaultField()
                            {
                                Name = "KEY",
                                Type = FieldType.Int,
                                Origin = OriginType.Table,
                                IsNullable = false,
                            };

                        var rankColumn = new DefaultField()
                        {
                            Name = "RANK",
                            Type = FieldType.Int,
                            Origin = OriginType.Table,
                            IsNullable = false,
                        };

                        // TODO : are these part of the result set or not?
                        //var fullTextSourceTableColumns = fullTextTableReference.Columns.Count == 1
                        //    && fullTextTableReference.Columns.First().ColumnType == ColumnType.Wildcard
                        //    ? newReferences
                        //        .SelectMany(x => x.Value.Columns)
                        //        .ToList()
                        //    : fullTextTableReference
                        //        .Columns
                        //        .Select(x => x.GetField(null, logger, file));

                        var fullTextTableColumns = new List<Field>()
                        {
                            keyColumn,
                            rankColumn,
                        };
                            //.Concat(fullTextSourceTableColumns)
                            //.ToList();

                        var fullTextTable = new DerivedTable()
                        {
                            Database = SchemaObject.TempDb,
                            Schema = SchemaObject.DefaultSchema,
                            Identifier = $"{fullTextTableIdentifier}-containstable",
                            File = file,
                            Columns = fullTextTableColumns,
                        };

                        return new List<SchemaObjectReference>()
                        {
                            new SchemaObjectReference()
                            {
                                Alias = fullTextTableReference.Alias?.Value,
                                Identifier = fullTextTable.GetQualifiedIdentfier(),
                                Value = fullTextTable,
                            }
                        };
                    }
                    
                case UnpivotedTableReference unpivotedTableReference:

                    var unpivotReferences = unpivotedTableReference
                        .TableReference
                        .GetSchemaObjectReferences(logger, file)
                        .ToList();

                    // TODO : or should these be added to current scope instead of pushing new scope?
                    using (new StatementContext(file.FileContext, unpivotReferences))
                    {
                        var sourceColumn = unpivotedTableReference
                            .InColumns
                            .First() // TODO : taking first, do I need to compute the value?
                            .GetField(null, logger, file);

                        var columnName = unpivotedTableReference.ValueColumn?.Value;

                        var unpivotTable = new DerivedTable()
                        {
                            Database = SchemaObject.TempDb,
                            Schema = SchemaObject.DefaultSchema,
                            Identifier = unpivotedTableReference.PivotColumn?.Value, // TODO : find a better name
                            File = file,
                            Columns = new List<Field>() { sourceColumn.Copy(columnName) },
                        };

                        return new List<SchemaObjectReference>()
                        {
                            new SchemaObjectReference()
                            {
                                Alias = unpivotedTableReference.Alias?.Value,
                                Identifier = unpivotTable.GetQualifiedIdentfier(),
                                Value = unpivotTable,
                            }
                        };
                    }

                case PivotedTableReference pivotedTableReference:
                    
                    var schemaObjectReferences = pivotedTableReference
                        .TableReference
                        .GetSchemaObjectReferences(logger, file)
                        .ToList();

                    // TODO : or should these be added to current scope instead of pushing new scope?
                    using (new StatementContext(file.FileContext, schemaObjectReferences))
                    {
                        var valueField = pivotedTableReference
                            .ValueColumns
                            .Select(x => x.GetField(null, logger, file))
                            .First(); // TODO : can there be more than one? I haven't seen any examples

                        var pivotColumns = pivotedTableReference
                            .InColumns
                            .Select(x => valueField.Copy(x.Value))
                            .ToList();

                        var pivotTable = new DerivedTable()
                        {
                            File = file,
                            Database = SchemaObject.TempDb,
                            Schema = SchemaObject.DefaultSchema,
                            Identifier = "TODO", // TODO : find suitable name
                            Columns = pivotColumns,
                        };

                        var pivotTableReference = new SchemaObjectReference()
                        {
                            Alias = pivotedTableReference.Alias?.Value,
                            Identifier = pivotTable.GetQualifiedIdentfier(),
                            Value = pivotTable,
                        };

                        schemaObjectReferences.Add(pivotTableReference);

                        return schemaObjectReferences;
                    }

                case GlobalFunctionTableReference globalFunctionTableReference:
                    {
                        switch (globalFunctionTableReference.Name.Value.ToUpper())
                        {
                            case "STRING_SPLIT":
                                {
                                    // Returns a single - column table with fragments. The name of the column is value.
                                    // Returns nvarchar if any of the input arguments are either nvarchar or nchar. Otherwise returns varchar.
                                    // The length of the return type is the same as the length of the string argument.
                                    var inputStringField = globalFunctionTableReference
                                        .Parameters
                                        .First()
                                        .GetField(null, logger, file)
                                        as StringField;

                                    var table = new Table()
                                    {
                                        File = file,
                                        Database = SchemaObject.TempDb,
                                        Schema = SchemaObject.DefaultSchema,
                                        Identifier = "STRING_SPLIT",
                                        Columns = new List<Field>()
                                        {
                                            new StringField()
                                            {
                                                Name = "value",
                                                Type = FieldType.String,
                                                Origin = OriginType.SystemType,
                                                IsNullable = false,
                                                Length = inputStringField?.Length ?? 0,
                                            }
                                        }
                                    };

                                    return new List<SchemaObjectReference>()
                                    {
                                        new SchemaObjectReference()
                                        {
                                            Alias = globalFunctionTableReference.Alias?.Value,
                                            Identifier = table.GetQualifiedIdentfier(),
                                            Value = table,
                                        }
                                    };
                                }
                            default:
                                break;
                        }
                        break;
                    }
            }

            logger.Log(LogLevel.Warning, 
                LogType.NotSupportedYet,
                file.Path, 
                $"{tableReference.GetType()} table reference type is not supported yet. " +
                $"Fragment \"{tableReference.GetTokenText()}\"");

            return new List<SchemaObjectReference>();
        }

        public static IEnumerable<FieldPairReference> GetFieldPairs(
            this IEnumerable<TableReference> tableReferences,
            ILogger logger,
            SchemaFile file
        )
        {
            return tableReferences
                .SelectMany(x => x.GetFieldPairs(logger, file))
                .ToList();
        }

        public static IEnumerable<FieldPairReference> GetFieldPairs(
            this TableReference tableReference,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (tableReference)
            {
                case QualifiedJoin qualifiedJoin:
                    return qualifiedJoin
                        .SearchCondition
                        .GetFieldPairs(logger, file)
                        .ToList();

                // TODO : do I care about unqualified joins?
                case UnqualifiedJoin unqualifiedJoin:
                default:
                    return new List<FieldPairReference>();
            }
        }
    }
}
