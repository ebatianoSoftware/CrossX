
namespace CrossX.WindowsForms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.skglControl = new SkiaSharp.Views.Desktop.SKGLControl();
            this.SuspendLayout();
            // 
            // skglControl
            // 
            this.skglControl.BackColor = System.Drawing.Color.Black;
            this.skglControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skglControl.Location = new System.Drawing.Point(0, 0);
            this.skglControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.skglControl.Name = "skglControl";
            this.skglControl.Size = new System.Drawing.Size(800, 450);
            this.skglControl.TabIndex = 0;
            this.skglControl.VSync = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.skglControl);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion

        private SkiaSharp.Views.Desktop.SKGLControl skglControl;
    }
}