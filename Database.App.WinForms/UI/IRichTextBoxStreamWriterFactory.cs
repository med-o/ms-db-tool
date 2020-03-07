using System.Windows.Forms;

namespace Database.App.WinForms.UI
{
    public interface IRichTextBoxStreamWriterFactory
    {
        RichTextBoxStreamWriter Create(RichTextBox output);
    }
}
