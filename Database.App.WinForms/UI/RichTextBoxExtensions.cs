using System;
using System.Drawing;
using System.Windows.Forms;

namespace Database.App.WinForms.UI
{
    public static class RichTextBoxExtensions
    {
        public static void AppendLine(this RichTextBox textBox)
        {
            textBox.AppendText(Environment.NewLine);
        }

        // TODO : how to pass in information about text style?
        //public static void AppendLine(this RichTextBox textBox, string text, Color color)
        public static void AppendLine(this RichTextBox textBox, string text)
        {
            //textBox.SelectionStart = textBox.TextLength;
            //textBox.SelectionLength = 0;
            //textBox.SelectionColor = color;
            textBox.AppendText($"{text}{Environment.NewLine}");
            //textBox.SelectionColor = textBox.ForeColor;

        }
    }
}
