using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Database.App.WinForms.UI
{
    public class RichTextBoxStreamWriter : TextWriter
    {
        private readonly RichTextBox _output;

        public RichTextBoxStreamWriter(RichTextBox output)
        {
            _output = output;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine()
        {
            _output.AppendLine();
        }

        public override void Write(char value)
        {
            _output.AppendText(value.ToString());
        }

        public override void WriteLine(string value)
        {
            _output.AppendLine(value);
        }
    }
}
