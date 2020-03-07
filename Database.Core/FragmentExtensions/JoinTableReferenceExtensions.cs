using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class JoinTableReferenceExtensions
    {
        public static List<SchemaObjectReference> GetSchemaObjectReferences(this JoinTableReference qualifiedJoin, ILogger logger, SchemaFile file)
        {
            var databaseObjectReferences = new List<SchemaObjectReference>();

            var firstReferences = qualifiedJoin.FirstTableReference.GetSchemaObjectReferences(logger, file);
            if (firstReferences.Any()) databaseObjectReferences.AddRange(firstReferences);

            var secondReferences = qualifiedJoin.SecondTableReference.GetSchemaObjectReferences(logger, file);
            if (secondReferences.Any()) databaseObjectReferences.AddRange(secondReferences);

            return databaseObjectReferences;
        }
    }
}
