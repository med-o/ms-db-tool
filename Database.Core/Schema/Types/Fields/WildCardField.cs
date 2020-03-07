namespace Database.Core.Schema.Types.Fields
{
    public class WildCardField : Field
    {
        public WildCardField()
        {
            Name = "WildCard";
            Type = FieldType.WildCard;
            Origin = OriginType.NotSpecified;
            IsNullable = false;
            HasIdentity = false;
        }
    }
}
