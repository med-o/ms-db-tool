using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class TSqlStatementExtensions
    {
		// TODO : manage everything through context, this could then return void
        public static IEnumerable<SchemaObject> CollectLocalSchema(this IEnumerable<TSqlStatement> statements, ILogger logger, SchemaFile file)
        {
            var dataSets = new List<SchemaObject>();

            if (statements.Any())
            {
                foreach (var statement in statements)
                {
                    var data = statement
                        .CollectLocalSchema(logger, file)
                        .ToList();

                    dataSets.AddRange(data);
                }
            }

            return dataSets;
        }

        public static IEnumerable<SchemaObject> CollectLocalSchema(this TSqlStatement statement, ILogger logger, SchemaFile file)
        {
            switch (statement)
            {
                case BeginEndBlockStatement beginEndBlockStatement:
                    return beginEndBlockStatement
                        .StatementList
                        .Statements
                        .CollectLocalSchema(logger, file)
                        .ToList();

                case DeclareVariableStatement declareVariableStatement:
                    {
                        foreach (var declaration in declareVariableStatement.Declarations)
                        {
                            var name = declaration.VariableName.Value;
                            var isNullable = false; // TODO : how to determine this?
                            var variable = declaration.DataType.GetField(name, isNullable, logger, file);
                            variable.Origin = OriginType.Variable;

                            file.FileContext.Variables.Peek().Add(variable);
                        }

                        // TODO : what should I return here?
                        break;
                    }

                case DeclareTableVariableStatement declareTableVariableStatement:
                    {
                        var columns = declareTableVariableStatement
                            .Body
                            .Definition
                            .ColumnDefinitions
                            .GetFields(logger, file)
                            .ToList();

                        var tableReference = new Table()
                        {
                            Columns = columns,
                            File = file,
                            Database = SchemaObject.TempDb,
                            Schema = SchemaObject.DefaultSchema,
                            Identifier = declareTableVariableStatement.Body.VariableName.Value,
                        };

                        var field = new TableReferenceField()
                        {
                            Name = declareTableVariableStatement.Body.VariableName.Value,
                            Type = FieldType.Table,
                            Origin = OriginType.Variable,
                            IsNullable = false,
                            Reference = tableReference,
                        };

                        file.FileContext.Variables.Peek().Add(field);

                        // TODO : what should I return here?
                        break;
                }

                    // TODO : this could be an actual create table statement and not just a temp table
                case CreateTableStatement createTableStatement:
                    {
                        if (!createTableStatement.SchemaObjectName.BaseIdentifier.Value.StartsWith("#"))
                        {
                            break; // not a temp table
                        }

                        var columns = createTableStatement
                            .Definition
                            .ColumnDefinitions
                            .GetFields(logger, file)
                            .ToList();

                        columns.ForEach(c => c.Origin = OriginType.Table);

                        var tempTable = new TemporaryTable()
                        {
                            Database = createTableStatement.SchemaObjectName.DatabaseIdentifier?.Value ?? SchemaObject.TempDb,
                            Schema = createTableStatement.SchemaObjectName.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                            Identifier = createTableStatement.SchemaObjectName.BaseIdentifier.Value,
                            File = file,
                            Columns = columns,
                        };

                        file
                            .LocalSchema
                            .Add(new KeyValuePair<string, SchemaObject>(tempTable.GetQualifiedIdentfier(), tempTable));

                        break;
                    }

                case IfStatement ifStatement:
                    {
                        // TODO : conditional output? which data set to return? we don't know till runtime
                        var thenReferences = ifStatement.ThenStatement.CollectLocalSchema(logger, file).ToList();

                        if (ifStatement.ElseStatement != null)
                        {
                            var elseReferences = ifStatement.ElseStatement.CollectLocalSchema(logger, file).ToList();
                            return thenReferences.Concat(elseReferences);
                        }

                        return thenReferences;
                    }

                case SelectStatement selectStatement:
                    {
                        var columns = selectStatement.GetFields(logger, file);

                        if (!columns.Any())
                        {
                            // if there are no columns there's no data set to return.. 
                            // this happens for SELECT statement that assigns values to variables
                            break;
                        }

                        if (selectStatement.Into != null 
                            && selectStatement.Into.BaseIdentifier.Value.StartsWith("#")
                            && !file.LocalSchema.ContainsKey(selectStatement.Into.GetTemporaryQualifiedIdentfier()))
                        {
                            var tempTableColumns = selectStatement.GetFields(logger, file); ;
                            var tempTable = new TemporaryTable()
                            {
                                Columns = tempTableColumns,
                                File = file,
                                Database = SchemaObject.TempDb,
                                Schema = SchemaObject.DefaultSchema,
                                Identifier = selectStatement.Into.BaseIdentifier.Value,
                            };

                            file
                                .LocalSchema
                                .Add(new KeyValuePair<string, SchemaObject>(tempTable.GetQualifiedIdentfier(), tempTable));
                        }

                        var dataSet = new DerivedTable()
                        {
                            Columns = columns,
                            File = file,
                            Identifier = selectStatement.GetTokenText(),
                        };

                        return new List<SchemaObject>() { dataSet };
                    }

                case WhileStatement whileStatement:
                    return whileStatement.Statement.CollectLocalSchema(logger, file);

                case TryCatchStatement tryCatchStatement:
                {
                    var tryReferences = tryCatchStatement.TryStatements.Statements.CollectLocalSchema(logger, file);
                    var catchReferences = tryCatchStatement.CatchStatements.Statements.CollectLocalSchema(logger, file);
                    return tryReferences.Concat(catchReferences).ToList();
                }

                case ReturnStatement x:
                    {
                        // TODO : check this statement, do I want to stop collecting data sets now?
                        // what if it is conditinal return statement?
                        break;
                    }

                case MergeStatement mergeStatement:
                    break; // TODO : what to do with this one?

                // NOTE : I don't care about these statements yet
                case PredicateSetStatement x: break;
                case SetVariableStatement x: break;
                case SetCommandStatement x: break;
                case SetRowCountStatement x: break;
                case UseStatement x: break;

                case DenyStatement x: break;
                case RevokeStatement x: break;
                case SetIdentityInsertStatement x: break;
                case SetTransactionIsolationLevelStatement x: break;
                case BeginTransactionStatement x: break;
                case RollbackTransactionStatement x: break;
                case CommitTransactionStatement x: break;
                case RaiseErrorStatement x: break;
                case ThrowStatement x: break;
                case BreakStatement x: break;
                case ContinueStatement x: break;
                case SaveTransactionStatement x: break;
                case UpdateStatisticsStatement x: break;

                case InsertStatement x: break;
                case UpdateStatement x: break;
                case DeleteStatement x: break;
                case ExecuteStatement x: break;
                case GrantStatement x: break;

                case CreateIndexStatement x: break;
                case GoToStatement x: break;
                case LabelStatement x: break;
                case PrintStatement x: break;

                case DeclareCursorStatement x: break;
                case OpenCursorStatement x: break;
                case FetchCursorStatement x: break;
                case CloseCursorStatement x: break;
                case DeallocateCursorStatement x: break;
                case WaitForStatement x: break;

                case BeginDialogStatement x: break;
                case SendStatement x: break;
                case EndConversationStatement x: break;

                // TODO : statements to generate schema.. might be useful for sql in the test project
                case TruncateTableStatement x: break;
                case DropTableStatement x: break;
                case DropViewStatement x: break;
                case CreateFunctionStatement x: break;
                case AlterFunctionStatement x: break;
                case CreateOrAlterFunctionStatement x: break;
                case DropFunctionStatement x: break;
                case AlterTableAddTableElementStatement x: break;
                case AlterTableConstraintModificationStatement x: break;
                case CreateTypeTableStatement x: break;
                case CreateViewStatement x: break;
                case AlterViewStatement x: break;
                case DropProcedureStatement x: break;
                case CreateProcedureStatement x: break;
                case CreateOrAlterProcedureStatement x: break;
                case CreateOrAlterViewStatement x: break;
                case AlterTableSetStatement x: break;
                case AlterProcedureStatement x: break;
                case CreateTypeUddtStatement x: break;

                default:
                    {
                        logger.Log(LogLevel.Warning, 
                            LogType.NotSupportedYet,
                            file.Path, 
                            $"\"{statement.GetType()}\" Tsql statement is not supported yet. " +
                            $"Fragment: \"{statement.GetTokenText()}\"");
                        break;
                    }
            }

            return new List<SchemaObject>();
        }
    }
}
