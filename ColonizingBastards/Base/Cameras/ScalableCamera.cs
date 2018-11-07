using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.Base.Cameras
{
    class ScalableCamera:Camera
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Size { get; set; }

        private GraphicsDeviceManager graphics;
        public Vector2 BaseScreenSize { get; }

        private float aspectRatio;

        public ScalableCamera(GraphicsDeviceManager graphics, Vector2 baseScreenSize, Vector2 position, float rotation, float size)
        {
            this.graphics = graphics;
            BaseScreenSize = baseScreenSize;
            Position = position;
            Rotation = rotation;
            Size = size;

            aspectRatio = BaseScreenSize.X / BaseScreenSize.Y - 0.5f;
        }

        public override Matrix GetViewMatrix()
        {
            float horScaling = graphics.GraphicsDevice.Viewport.Width / BaseScreenSize.X;
            float verScaling = graphics.GraphicsDevice.Viewport.Height / BaseScreenSize.Y;
            //float horScaling = verScaling * aspectRatio;//graphics.GraphicsDevice.Viewport.Width / BaseScreenSize.X;

            return
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(horScaling , verScaling, 1);
        }
    }
}
