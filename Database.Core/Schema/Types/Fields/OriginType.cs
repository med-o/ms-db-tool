namespace Database.Core.Schema.Types.Fields
{
    public enum OriginType
    {
        NotSpecified,
        Literal,
        Table,
        Parameter,
        FunctionReturn,
        ForClause,
        Variable,
        GlobalVariable,
        SystemType,
        Reference,
        Computed
    }
}
