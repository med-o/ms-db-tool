using System;
using System.Runtime.Serialization;

namespace Database.Core.Schema.Types.Fields
{
    [DataContract]
    [KnownType(typeof(UnknownField))]
    [KnownType(typeof(WildCardField))]
    [KnownType(typeof(DefaultField))]
    [KnownType(typeof(StringField))]
    [KnownType(typeof(DecimalField))]
    [KnownType(typeof(TableReferenceField))]
    public abstract class Field : ICloneable
    {
        public Field()
        {
            Type = FieldType.NotSpecified;
            Origin = OriginType.NotSpecified;
            IsNullable = false;
            Name = "NotSpecified";
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public FieldType Type { get; set; }

        [DataMember]
        public OriginType Origin { get; set; }

        [DataMember]
        public bool IsNullable { get; set; }

        [DataMember]
        public bool HasIdentity { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

    }

    public static class FieldExtensions
    {
        public static Field Copy(this Field column, string newName)
        {
            var clone = (Field)column.Clone();
            clone.Name = newName ?? clone.Name;
            return clone;
        }
    }
}
