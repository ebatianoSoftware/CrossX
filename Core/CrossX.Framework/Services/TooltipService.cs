using CrossX.Abstractions.IoC;
using CrossX.Framework.Core;
using CrossX.Framework.Drawables;
using CrossX.Framework.UI;
using CrossX.Framework.UI.Controls;
using System;
using System.Numerics;

namespace CrossX.Framework.Services
{
    internal class TooltipService : ITooltipService
    {
        private readonly IObjectFactory objectFactory;
        private readonly IAppValues appValues;

        private readonly Drawable backgroundDrawable;

        public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(700);

        private Label label;
        private View currentContext;
        private DateTime firstShowAttempt;

        public TooltipService(IObjectFactory objectFactory, IAppValues appValues)
        {
            this.objectFactory = objectFactory;
            this.appValues = appValues;
            backgroundDrawable = appValues.GetResource("SystemTooltipBackgroundDrawable") as Drawable;
        }

        public void ShowTooltip(View control, string text, Vector2 position)
        {
            if (string.IsNullOrEmpty(text)) return;

            var window = control?.Parent?.Window;
            if (window == null) return;

            if(currentContext != control)
            {
                firstShowAttempt = DateTime.Now;
                currentContext = control;
                window.MainFrame.Children.Remove(label);
                label?.Dispose();
                label = null;
            }
            else
            {
                if (label?.Text == text) return;

                if(label != null)
                {
                    label.Text = text;
                    return;
                }
            }

            if ((DateTime.Now - firstShowAttempt) < Delay) return;

            label = objectFactory.Create<Label>().Set(l =>
            {
                l.Text = text;
                l.TextPadding = new Thickness(5);
                l.ForegroundColor = (Color)appValues.GetValue("SystemTooltipForegroundColor");
                l.BackgroundColor = (Color)appValues.GetValue("SystemTooltipBackgroundColor");
                l.BackgroundDrawable = backgroundDrawable;
                l.FontSize = 12;
                l.FontWeight = FontWeight.Normal;
                l.FontFamily = "Default";
                l.Margin = new Thickness(position.X, position.Y, 0, 0);
                l.HorizontalAlignment = Alignment.Start;
                l.VerticalAlignment = Alignment.Start;
            });

            window.MainFrame.Children.Add(label);
            window.MainFrame.InvalidateLayout();
            
        }

        public void HideTooltip(View control)
        {
            if(currentContext == control)
            {
                var window = control?.Parent?.Window;
                if (window == null) return;

                window.MainFrame.Children.Remove(label);
                label?.Dispose();
                label = null;
                currentContext = null;
            }
        }
    }

    public interface ITooltipService
    {
        void ShowTooltip(View control, string text, Vector2 position);
        void HideTooltip(View control);
    }
}
