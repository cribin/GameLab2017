using ColonizingBastards.Base.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects
{
    class ButtonActor : Actor
    {
        public ButtonActor(RenderObject rep) : base(rep)
        {
            // nothing
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
        }

        public override void Draw(SpriteBatch batch)
        {
            rep.Draw(batch);
            // maybe draw text

        }
    }
}
