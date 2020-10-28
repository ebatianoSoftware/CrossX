// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Graphics;
using EbatianoSoftware.CrossX.Graphics;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

namespace CrossX.DxCommon.Graphics
{
    internal class DxEffect : IDisposable
    {
        public PixelShader PixelShader { get; }
        public VertexShader VertexShader { get; }

        public InputLayout InputLayout { get; }

        public static DxEffect FromResource(string name, VertexContent content, Device1 device)
        {
            var assembly = typeof(DxEffect).Assembly;
            var resName = assembly.FullName.Split(',')[0];
            
            ShaderBytecode vsCode;
            using (var stream = assembly.GetManifestResourceStream(resName + $".FX.Output.{name}.vsbc"))
            {
                vsCode = ShaderBytecode.FromStream(stream);
            }

            ShaderBytecode psCode;
            using (var stream = assembly.GetManifestResourceStream(resName + $".FX.Output.{name}.psbc"))
            {
                psCode = ShaderBytecode.FromStream(stream);
            }

            var elements = ElementsFromVertexContent(content);

            InputLayout inputLayout = null;
            inputLayout = new InputLayout(device, vsCode, elements);

            var vertexShader = new VertexShader(device, vsCode);
            var pixelShader = new PixelShader(device, psCode);

            return new DxEffect(vertexShader, pixelShader, inputLayout);
        }

        private DxEffect(VertexShader vs, PixelShader ps, InputLayout il)
        {
            VertexShader = vs;
            PixelShader = ps;
            InputLayout = il;
        }

        private static InputElement[] ElementsFromVertexContent(VertexContent content)
        {
            var list = new List<InputElement>();
            var offset = 0;
            if (content.HasFlag(VertexContent.Position))
            {
                list.Add(new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0));
                offset += 16;
            }

            if (content.HasFlag(VertexContent.Color))
            {
                list.Add(new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, offset, 0));
                offset += 4;
            }

            if (content.HasFlag(VertexContent.TextureCoordinates))
            {
                list.Add(new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, offset, 0));
                offset += 8;
            }

            return list.ToArray();
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    VertexShader.Dispose();
                    PixelShader.Dispose();
                    InputLayout.Dispose();
                }
                _disposedValue = true;
            }
        }
        
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
