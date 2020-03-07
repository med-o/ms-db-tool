namespace Database.Core.Schema
{
    public enum SchemaObjectType
    {
        NotSpecified,
        UserDefinedType,
        UserDefinedTableType,
        Table,
        View,
        StoredProcedure,
        Function,
        Cte,
        DerivedTable,
        TemporaryTable,
    }
}
