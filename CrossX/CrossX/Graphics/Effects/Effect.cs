using CrossX.Graphics.Shaders;
using XxIoC;
using System;
using System.Reflection;

namespace CrossX.Graphics.Effects
{
    public abstract class Effect: IDisposable
    {
        protected readonly IGraphicsDevice GraphicsDevice;
        protected readonly IObjectFactory ObjectFactory;
        private readonly IShadersRepository shadersRepository;

        protected Effect(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository)
        {
            GraphicsDevice = graphicsDevice;
            ObjectFactory = objectFactory;
            this.shadersRepository = shadersRepository;
        }

        protected VertexShader CreateVertexShader(Assembly assembly, string name, VertexContent vertexContent)
        {
            return CreateVertexShader(assembly, name, vertexContent, out var _);
        }

        protected VertexShader CreateVertexShader<TBuffer>(Assembly assembly, string name, VertexContent vertexContent) where TBuffer : struct
        {
            var shader = CreateVertexShader(assembly, name, vertexContent, out var created);
            if (created)
            {
                shader.CreateConstBuffer<TBuffer>(0);
            }
            return shader;
        }

        protected VertexShader CreateVertexShader<TBuffer0, TBuffer1>(Assembly assembly, string name, VertexContent vertexContent) where TBuffer0 : struct where TBuffer1: struct
        {
            var shader = CreateVertexShader(assembly, name, vertexContent, out var created);
            if (created)
            {
                shader.CreateConstBuffer<TBuffer0>(0);
                shader.CreateConstBuffer<TBuffer1>(1);
            }
            return shader;
        }

        protected VertexShader CreateVertexShader<TBuffer0, TBuffer1, TBuffer2>(Assembly assembly, string name, VertexContent vertexContent) 
            where TBuffer0 : struct where TBuffer1 : struct where TBuffer2: struct
        {
            var shader = CreateVertexShader(assembly, name, vertexContent, out var created);
            if (created)
            {
                shader.CreateConstBuffer<TBuffer0>(0);
                shader.CreateConstBuffer<TBuffer1>(1);
                shader.CreateConstBuffer<TBuffer2>(2);
            }
            return shader;
        }

        protected VertexShader CreateVertexShader<TBuffer0, TBuffer1, TBuffer2, TBuffer3>(Assembly assembly, string name, VertexContent vertexContent)
            where TBuffer0 : struct where TBuffer1 : struct where TBuffer2 : struct where TBuffer3: struct
        {
            var shader = CreateVertexShader(assembly, name, vertexContent, out var created);
            if (created)
            {
                shader.CreateConstBuffer<TBuffer0>(0);
                shader.CreateConstBuffer<TBuffer1>(1);
                shader.CreateConstBuffer<TBuffer2>(2);
                shader.CreateConstBuffer<TBuffer3>(3);
            }
            return shader;
        }

        protected PixelShader CreatePixelShader(Assembly assembly, string name)
        {
            return CreatePixelShader(assembly, name, out var _);
        }

        protected PixelShader CreatePixelShader<TBuffer>(Assembly assembly, string name) where TBuffer : struct
        {
            var shader = CreatePixelShader(assembly, name, out var created);
            if (created)
            {
                shader.CreateConstBuffer<TBuffer>(0);
            }
            return shader;
        }

        protected PixelShader CreatePixelShader<TBuffer0, TBuffer1>(Assembly assembly, string name) where TBuffer0 : struct where TBuffer1 : struct
        {
            var shader = CreatePixelShader(assembly, name, out var created);
            if (created)
            {
                shader.CreateConstBuffer<TBuffer0>(0);
                shader.CreateConstBuffer<TBuffer1>(1);
            }
            return shader;
        }

        protected PixelShader CreatePixelShader<TBuffer0, TBuffer1, TBuffer2>(Assembly assembly, string name) where TBuffer0 : struct where TBuffer1 : struct where TBuffer2: struct
        {
            var shader = CreatePixelShader(assembly, name, out var created);
            if (created)
            {
                shader.CreateConstBuffer<TBuffer0>(0);
                shader.CreateConstBuffer<TBuffer1>(1);
                shader.CreateConstBuffer<TBuffer2>(2);
            }
            return shader;
        }

        protected PixelShader CreatePixelShader<TBuffer0, TBuffer1, TBuffer2, TBuffer3>(Assembly assembly, string name) 
            where TBuffer0 : struct where TBuffer1 : struct where TBuffer2 : struct where TBuffer3: struct
        {
            var shader = CreatePixelShader(assembly, name, out var created);
            if (created)
            {
                shader.CreateConstBuffer<TBuffer0>(0);
                shader.CreateConstBuffer<TBuffer1>(1);
                shader.CreateConstBuffer<TBuffer2>(2);
                shader.CreateConstBuffer<TBuffer3>(3);
            }
            return shader;
        }

        protected PixelShader CreatePixelShader(Assembly assembly, string name, out bool created)
        {
            created = false;
            name = assembly.FullName.Split(',')[0] + ".FX." + name;

            var shader = shadersRepository.GetPixelShader(name);
            if (shader != null) return shader;

            shader = ObjectFactory.Create<PixelShader>(new CreatePixelShaderFromResource
            {
                Assembly = assembly,
                Path = name
            });

            shadersRepository.RegisterPixelShader(name, shader);
            created = true;
            return shader;
        }

        protected VertexShader CreateVertexShader(Assembly assembly, string name, VertexContent vertexContent, out bool created)
        {
            created = false;
            name = assembly.FullName.Split(',')[0] + ".FX." + name;

            var shader = shadersRepository.GetVertexShader(name);
            if (shader != null) return shader;

            shader = ObjectFactory.Create<VertexShader>(new CreateVertexShaderFromResource
            {
                Assembly = assembly,
                Path = name,
                VertexContent = vertexContent
            });

            shadersRepository.RegisterVertexShader(name, shader);
            created = true;
            return shader;
        }

        public virtual void Dispose()
        {
            
        }
    }
}
