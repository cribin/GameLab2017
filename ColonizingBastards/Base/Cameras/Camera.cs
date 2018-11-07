using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.Cameras
{
    abstract class Camera
    {

        public abstract Matrix GetViewMatrix();

    }
}
