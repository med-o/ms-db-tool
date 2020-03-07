using System.Runtime.Serialization;

namespace Database.Core.Schema.Types.Fields
{
    [DataContract]
    public class Parameter
    {
        public Parameter(Field field)
        {
            Value = field;
        }

        [DataMember]
        public Field Value { get; set; }

        [DataMember]
        public bool IsReadOnly { get; set; }

        [DataMember]
        public bool IsOutput { get; set; }

        [DataMember]
        public bool HasDefaultValue { get; set; }
    }
}
