using CrossX.Framework.Input.TextInput;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CrossX.WindowsForms.Input
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class LOGFONT
    {
        public int lfHeight = 0;
        public int lfWidth = 0;
        public int lfEscapement = 0;
        public int lfOrientation = 0;
        public int lfWeight = 0;
        public byte lfItalic = 0;
        public byte lfUnderline = 0;
        public byte lfStrikeOut = 0;
        public byte lfCharSet = 0;
        public byte lfOutPrecision = 0;
        public byte lfClipPrecision = 0;
        public byte lfQuality = 0;
        public byte lfPitchAndFamily = 0;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string lfFaceName = string.Empty;
    }

    internal class NativeTextBox : TextBox, INativeTextBox
    {
        private readonly INativeTextBoxControl crossControl;

        public NativeTextBox(WindowHost form, INativeTextBoxControl control)
        {
            var bounds = control.Bounds.ToDrawing();

            Visible = true;
            Location = Point.Empty;
            Size = new Size(200, 30);
            BorderStyle = BorderStyle.None;
            Multiline = false;

            BackColor = Color.White;
            ForeColor = Color.Black;

            MaxLength = control.MaxLength;

            form.Controls.Add(this);
            AcceptsReturn = true;

            Disposed += (o, e) =>
            {
                form.Redraw();
            };
            Show();
            Focus();

            crossControl = control;
            Text = control.Text;
        }

        public (int start, int length) Selection
        {
            get => (SelectionStart, SelectionLength);

            set
            {
                SelectionStart = value.start;
                SelectionLength = value.length;
            }
        }

        public void Release()
        {
            Parent.Controls.Remove(this);
            DestroyHandle();
            Dispose();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                crossControl.OnLostFocus();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                crossControl.OnLostFocus();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            base.OnKeyDown(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            crossControl.OnLostFocus();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (crossControl != null)
            {
                crossControl.Text = Text;
            }
        }

        void INativeTextBox.Focus()
        {
            Focus();
        }
    }
}
