namespace Database.Core.Logging
{
    public enum LogType
    {
        NotSpecified,
        NotSupportedYet,
        ParsingError,
        Validation,
        MissingSchemaObject,
        MissingDatabaseContext,
        MissingColumnDefinition,
        InvalidParameterIndetifier,
        TooManyParameters,
        AddingUnknownColumn,
    }
}
