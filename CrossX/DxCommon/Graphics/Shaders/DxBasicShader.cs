using CrossX.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using CrossX.Graphics.Shaders;
using SharpDX.Mathematics.Interop;
using CrossX.DxCommon.Helpers;

namespace CrossX.DxCommon.Graphics.Shaders
{
    internal class DxBasicShader : BasicShader, IDxShader
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ConstBuffer
        {
            public RawMatrix Matrix;
            public RawColor4 Color;
        }

        private SharpDX.Direct3D11.Buffer constBuffer;
        public DxGraphicsDevice graphicsDevice;

        private readonly Dictionary<VertexContent, DxEffect> Effects = new Dictionary<VertexContent, DxEffect>();

        public DxBasicShader(DxGraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            RegisterDefaultEffects();

            constBuffer = new SharpDX.Direct3D11.Buffer(graphicsDevice.D3dDevice, Utilities.SizeOf<ConstBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

        public DxEffect EffectForContent(VertexContent content)
        {
            if (Effects.TryGetValue(content, out var effect)) return effect;
            throw new InvalidProgramException($"Could not find proper program for {content} vertices.");
        }

        private void RegisterDefaultEffects()
        {
            var efPc = DxEffect.FromResource("DefaultPC", VertexContent.Position | VertexContent.Color, graphicsDevice.D3dDevice);
            Effects.Add(VertexContent.Position | VertexContent.Color, efPc);

            var efPct = DxEffect.FromResource("DefaultPCT", VertexContent.Position | VertexContent.Color | VertexContent.TextureCoordinates, graphicsDevice.D3dDevice);
            Effects.Add(VertexContent.Position | VertexContent.Color | VertexContent.TextureCoordinates, efPct);

            var efPt = DxEffect.FromResource("DefaultPT", VertexContent.Position | VertexContent.TextureCoordinates, graphicsDevice.D3dDevice);
            Effects.Add(VertexContent.Position | VertexContent.TextureCoordinates, efPt);
        }

        public override void Apply()
        {
            var context = graphicsDevice.D3dDevice.ImmediateContext1;
            graphicsDevice.CurrentShader = this;
            context.VertexShader.SetConstantBuffer(0, constBuffer);
        }

        public void ApplyShaderParameters()
        {
            var dxTexture = Texture as DxTexture;
            var context = graphicsDevice.D3dDevice.ImmediateContext1;
            context.PixelShader.SetShaderResource(0, dxTexture?.View);

            var color = DiffuseColor * Alpha;
            
            var consts = new ConstBuffer
            {
                Matrix = Matrix.Multiply(worldMatrix, viewProjectionMatrix).ToRawMatrix(),
                Color = new RawColor4(color.Rf, color.Gf, color.Bf, color.Af)
            };

            context.UpdateSubresource(ref consts, constBuffer);
        }
    }
}
