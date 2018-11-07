using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Config
{
    static class ParticleConfig
    {
        public static Texture2D WHITE_CIRCLE { get; set; }

        public static Texture2D RAIN_DROP { get; set; }

        public static List<RenderObject> FOOTSTEP_TEXTURE { get;} = new List<RenderObject>();

        public static List<RenderObject> FOLIAGE_CUT_TEXTURE { get;} = new List<RenderObject>();

        //Default params for fire particle effect

    }
}
