using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.Config
{
    static class ProjectileConfig
    {
        public const int MAX_DISTANCE = 450;
        public const float SPEED_X = 500f;

        //Bullet Config
        public static Texture2D BULLET_TEXTURE { get; set; }
        public static Vector3 BULLET_SIZE { get; set; }
        public const float BULLET_STUN_TIME = 2;//in seconds
        //Other Projectile config ...

        //Dart config
        public static Texture2D DART_TEXTURE { get; set; }
        public static Vector3 DART_SIZE { get; set; }
        public const float DART_STUN_TIME = 2;//in seconds

    }
}
