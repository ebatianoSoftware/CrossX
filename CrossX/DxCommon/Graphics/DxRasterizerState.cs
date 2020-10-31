//using CrossX.Graphics;
//using SharpDX.Direct3D11;

//namespace CrossX.DxCommon.Graphics
//{
//    internal class DxRasterizerState : CrossX.Graphics.RasterizerState
//    {
//        public override RasterizerStateDesc Desc { get; }

//        public RasterizerState1 State { get; }

//        public DxRasterizerState(Device1 device, RasterizerStateDesc desc)
//        {
//            Desc = desc;
//            State = new RasterizerState1(device, new RasterizerStateDescription1
//            {
//                CullMode = (SharpDX.Direct3D11.CullMode)desc.CullMode,
//                FillMode = FillMode.Solid,
//                IsScissorEnabled = desc.IsScissorEnabled,
//                IsDepthClipEnabled = desc.IsDepthClipEnabled,
//                DepthBias = 0,
//                DepthBiasClamp = 0,
//                IsAntialiasedLineEnabled = true,
//                IsFrontCounterClockwise = true,
//                IsMultisampleEnabled = true,
//                SlopeScaledDepthBias = 0,
//            });
//        }

//        public override void Dispose()
//        {
//            State.Dispose();
//        }
//    }
//}
