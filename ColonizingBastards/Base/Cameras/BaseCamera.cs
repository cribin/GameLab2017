using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.Cameras
{
    class BaseCamera : Camera
    {

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Size { get; set; }

        public BaseCamera(Vector2 position, float rotation, float size)
        {
            Position = position;
            Rotation = rotation;
            Size = size;
        }

        public override Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Size, Size, 1);
        }

    }
}
