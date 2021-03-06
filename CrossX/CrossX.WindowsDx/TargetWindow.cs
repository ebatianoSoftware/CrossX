using CrossX.DxCommon.Graphics;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Windows.Forms;
using DxDevice1 = SharpDX.Direct3D11.Device1;
using DxDevice = SharpDX.Direct3D11.Device;
using System.Drawing;

namespace CrossX.WindowsDx
{
    internal class TargetWindow : ITargetWindow
    {
        private readonly Form form;
        public Size Size => form.ClientSize;

        public event EventHandler<Size> SizeChanged;
        private Rectangle formBounds;

        public TargetWindow(Form form)
        {
            this.form = form;
            formBounds = this.form.DesktopBounds;
            form.SizeChanged += Form_SizeChanged;
        }

        private void Form_SizeChanged(object sender, EventArgs e)
        {
            SizeChanged?.Invoke(sender, Size);
        }

        public SwapChain1 CreateSwapChain(int width, int height, bool fullscreen, out DxDevice1 device)
        {
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = !fullscreen,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
                Flags = SwapChainFlags.None
            };

            // Create Device and SwapChain
            SwapChain swapChain;
            DxDevice dxDevice;
            
            DxDevice.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.SingleThreaded, desc, out dxDevice, out swapChain);
            var context = dxDevice.ImmediateContext;

            // Ignore all windows events
            var factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            using (dxDevice)
            {
                device = dxDevice.QueryInterface<DxDevice1>();
            }

            using (swapChain)
            {
                return swapChain.QueryInterface<SwapChain1>();
            }
        }
    }
}
