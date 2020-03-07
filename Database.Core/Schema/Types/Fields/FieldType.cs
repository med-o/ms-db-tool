namespace Database.Core.Schema.Types.Fields
{
    public enum FieldType
    {
        NotSpecified,

        Null,

        // SQL native types
        BigInt,
        Int,
        SmallInt,
        TinyInt,
        Bit,
        Decimal,
        Numeric,
        Money,
        SmallMoney,
        Float,
        Real,
        DateTime,
        SmallDateTime,
        Binary,
        VarBinary,
        Image,
        Table,
        Timestamp,
        UniqueIdentifier,
        Date,
        Time,
        DateTime2,
        DateTimeOffset,
        SqlVariant,

        Xml,
        Json,

        String,
        Text,
        //Char,
        //VarChar,
        //Text,
        //NChar,
        //NVarChar,
        //NText,

        // custom types
        UserDataType,

        WildCard,
    }
}
