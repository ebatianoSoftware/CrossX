using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;

namespace CrossX.WindowsForms
{
    internal class MenuItemEx: ToolStripMenuItem
    {
        private static Image image = new Bitmap(24, 24);

        private readonly ICommand command;
        private readonly object model;

        public (string fontFamily, string iconText) Icon { get; set; }


        public MenuItemEx(object model, string text, ToolStripItem[] items): base(text, null, items)
        {
            this.model = model;
            Image = image;
            ImageScaling = ToolStripItemImageScaling.None;
        }

        public MenuItemEx(object model, string text, ICommand command) : base(text, null, OnClick)
        {
            this.model = model;
            this.command = command;
            Image = image;
            ImageScaling = ToolStripItemImageScaling.None;
            command.CanExecuteChanged += Command_CanExecuteChanged;
            Command_CanExecuteChanged(this, EventArgs.Empty);
        }

        private void OnClick() => command?.Execute(model);

        private static void OnClick(object sender, EventArgs e)
        {
            var item = (MenuItemEx)sender;
            item.OnClick();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(disposing)
            {
                if(command != null)
                {
                    command.CanExecuteChanged -= Command_CanExecuteChanged;
                }
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e) => Enabled = command.CanExecute(model);
    }
}
