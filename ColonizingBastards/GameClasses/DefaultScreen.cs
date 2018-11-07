using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Cameras;
using ColonizingBastards.Base.Director;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.Base.ScreenUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses
{
    //Default screens to be used by logics, if further processing is unneccesary
    class DefaultScreen:Screen
    {
        private Hud hud;
        public DefaultScreen(Director director, Scene scene, Vector2 baseScreenSize, Hud hud = null)
        {
            this.director = director;
            this.scene = scene;
            this.camera = new ScalableCamera(director.Graphics, baseScreenSize, new Vector2(0.0f, 0.0f), 0, 1);
            this.hud = hud;
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Begin(transformMatrix: camera.GetViewMatrix());

            foreach (Actor a in scene.GetActors())
            {
                a.Draw(batch);
            }

            batch.End();

            if (hud == null) return;
            batch.Begin(transformMatrix: camera.GetViewMatrix());

            hud.DrawHud(batch);

            batch.End();
        }
    }
}
