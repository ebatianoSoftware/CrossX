using CrossX.DxCommon.Graphics;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using Windows.UI.Core;
using SharpDX;

using SdxDevice2 = SharpDX.DXGI.Device2;
using SdxDevice = SharpDX.Direct3D11.Device;
using SdxDevice1 = SharpDX.Direct3D11.Device1;
using Windows.Graphics.Display;

namespace CrossX.UWP.Graphics
{
    internal class UwpWindow : ITargetWindow
    {
        private Size size;
        private readonly CoreWindow coreWindow;

        public bool IsFullscreen => throw new NotImplementedException();

        public Size Size
        {
            get => size;
            private set
            {
                size = value;
                SizeChanged?.Invoke(this, size);
            }
        }

        public event EventHandler<Size> SizeChanged;

        public UwpWindow(CoreWindow coreWindow)
        {
            this.coreWindow = coreWindow;
            coreWindow.SizeChanged += (s, a) => Resize();
            Resize();
        }

        private void Resize()
        {
            var displayInformation = DisplayInformation.GetForCurrentView();
            Size = new Size((int)(coreWindow.Bounds.Width * displayInformation.RawPixelsPerViewPixel), (int)(coreWindow.Bounds.Height * displayInformation.RawPixelsPerViewPixel));
        }

        public SwapChain1 CreateSwapChain(int width, int height, bool fullscreen, out SdxDevice1 device)
        {
            using (var defaultDevice = new SdxDevice(DriverType.Hardware, DeviceCreationFlags.None))
            {
                device = defaultDevice.QueryInterface<SdxDevice1>();
            }

            // SwapChain description
            var desc = new SwapChainDescription1
            {
                Width = width,
                Height = height,
                Format = Format.B8G8R8A8_UNorm,
                Stereo = false,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.BackBuffer | Usage.RenderTargetOutput,
                BufferCount = 2,
                Scaling = Scaling.None,
                Flags = SwapChainFlags.None,
                SwapEffect = SwapEffect.FlipSequential,
            };

            // Once the desired swap chain description is configured, it must be created on the same adapter as our D3D Device
            // First, retrieve the underlying DXGI Device from the D3D Device.
            // Creates the swap chain
            using (var dxgiDevice2 = device.QueryInterface<SdxDevice2>())
            using (Adapter dxgiAdapter = dxgiDevice2.Adapter)
            using (var dxgiFactory2 = dxgiAdapter.GetParent<Factory2>())
            {
                // Creates a SwapChain from a CoreWindow pointer
                using (var comWindow = new ComObject(coreWindow))
                {
                    try
                    {
                        return new SwapChain1(dxgiFactory2, dxgiDevice2, comWindow, ref desc);
                    }
                    catch(Exception ex)
                    {
                        throw;
                    }
                }
            }
        }

        public void SetFullscreen(bool fullscreen)
        {

        }
    }
}
