using CrossX;
using CrossX.Audio;
using CrossX.Audio.Decoders;
using CrossX.Core;
using CrossX.Graphics;
using CrossX.Graphics2D;
using XxIoC;
using System;

namespace T05.Audio
{
    public class T05_AudioApp : IApp
    {
        private readonly IObjectFactory objectFactory;

        private Sound sound;
        private AudioEmitter emitter;
        private AudioListener listener;
        private SoundInstance soundInstance;
        private float angle = 0;
        private PrimitiveBatch primitiveBatch;
        private readonly IGraphicsDevice graphicsDevice;

        private IAudioStream audioStream;
        private AudioStreamPlayer streamPlayer;

        public T05_AudioApp(IObjectFactory objectFactory, IGraphicsDevice graphicsDevice)
        {
            this.objectFactory = objectFactory;
            this.graphicsDevice = graphicsDevice;
        }

        public void LoadContent()
        {
            primitiveBatch = objectFactory.Create<PrimitiveBatch>();

            using (var stream = typeof(T05_AudioApp).Assembly.GetManifestResourceStream("T05.Audio.Sound.wav"))
            {
                sound = objectFactory.Create<Sound>(stream);
            }

            var mediaStream = typeof(T05_AudioApp).Assembly.GetManifestResourceStream("T05.Audio.Music.ogg");
            audioStream = new OggAudioStream(mediaStream);
            

            streamPlayer = objectFactory.Create<AudioStreamPlayer>(audioStream);
            streamPlayer.Play(true);

            emitter = objectFactory.Create<AudioEmitter>();
            listener = objectFactory.Create<AudioListener>();

            emitter.Forward = Vector3.Backward;
            emitter.Up = Vector3.Up;
            emitter.Rolloff = 0.1f;
            emitter.MinDistance = 0.01f;
            emitter.MaxDistance = 12;
            emitter.Position = new Vector3(0,0,5);

            soundInstance = sound.NewInstance(emitter, listener);
            soundInstance.IsLooped = true;
            soundInstance.Play();
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Black);

            primitiveBatch.DrawCircle(new Vector2(graphicsDevice.Size.Width / 2, graphicsDevice.Size.Height / 2), 50, Color4.White);

            var vector = Vector2.Rotate(new Vector2(0, 1), angle) * 200;
            var center = new Vector2(graphicsDevice.Size.Width / 2, graphicsDevice.Size.Height / 2);
            
            primitiveBatch.DrawCircle(center, 50, Color4.White);
            primitiveBatch.DrawCircle(center + vector, 50, Color4.White);

            graphicsDevice.Present();
        }

        public void Update(TimeSpan frameTime)
        {
            angle += (float)frameTime.TotalSeconds;
            var oldPos = emitter.Position;
            emitter.Position = Vector3.Transform(new Vector3(0, 0, 5), Matrix.CreateRotationY(angle));
            var velocity = (emitter.Position - oldPos) / (float)frameTime.TotalSeconds;
            emitter.Velocity = velocity;
        }
    }
}
