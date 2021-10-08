using ControlzEx.Standard;
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

    internal class NativeTextBox : RichTextBox, INativeTextBox
    {
        private readonly INativeTextBoxControl crossControl;

        public NativeTextBox(WindowHost form, INativeTextBoxControl control, Point clickPosition)
        {
            form.SuspendLayout();
            var bounds = control.Bounds.ToDrawing();
            
            var fontFace = control.FontFamily;

            float fontSize = control.FontSize.Calculate() * 72 / 96 * 0.99f;
            
            Visible = true;
            BorderStyle = BorderStyle.None;
            Multiline = false;
            
            BackColor = control.BackgroundColor.ToDrawing();
            ForeColor = control.TextColor.ToDrawing();

            Font = new Font(fontFace, fontSize, GraphicsUnit.Point);

            bounds.Inflate(1, 1);

            bounds.X -= DefaultPadding.Left;
            bounds.Y -= DefaultPadding.Top;

            bounds.Width += DefaultPadding.Size.Width;
            bounds.Height += DefaultPadding.Size.Height;

            Location = bounds.Location;
            Size = new Size(bounds.Size.Width, bounds.Size.Height);
            MinimumSize = Size;
            MaximumSize = Size;

            form.Controls.Add(this);
            form.Controls.SetChildIndex(this, 0);

            Disposed += (o, e) =>
            {
                form.Redraw();
                Font.Dispose();
            };
            Focus();

            form.ResumeLayout();
            form.PerformLayout();

            crossControl = control;
            Text = control.Text;

            clickPosition.X -= Location.X;
            clickPosition.Y -= Location.Y;

            SelectAll();
        }

        public void Release()
        {
            Parent.Controls.Remove(this);
            DestroyHandle();
            Dispose();
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
    }
}
