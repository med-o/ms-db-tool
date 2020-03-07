namespace Database.Core.Validation
{
    public enum ValidationRuleType
    {
        NotSpecified,
        ImplicitConversion,
        NoLeadingWildcard,
        NoSelectStart,
        SchemaNotDefined,
        MissingColumnPrefix,
        RoundingFound,
        NoStringColumnWithDefaultLength,
        DecimalFieldWithDefaultValues,
        FileNameCorrespondsToSchemaObject,
    }
}
