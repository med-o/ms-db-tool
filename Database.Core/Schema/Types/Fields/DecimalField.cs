namespace Database.Core.Schema.Types.Fields
{
    public class DecimalField : Field
    {
        public int Precision { get; set; }

        public int Scale { get; set; }
    }
}
