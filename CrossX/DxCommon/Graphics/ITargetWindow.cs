using SharpDX.DXGI;
using System;
using System.Drawing;
using SdxDevice1 = SharpDX.Direct3D11.Device1;

namespace CrossX.DxCommon.Graphics
{
    internal interface ITargetWindow
    {
        event EventHandler<Size> SizeChanged;

        Size Size { get; }

        SwapChain1 CreateSwapChain(int width, int height, bool fullscreen, out SdxDevice1 device);
    }
}
