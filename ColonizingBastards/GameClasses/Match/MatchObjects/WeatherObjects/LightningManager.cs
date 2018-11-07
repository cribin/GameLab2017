using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.WeatherObjects
{
    class LightningManager
    {
        public List<BranchLightning> CurrentLightnings { get; }

        public LightningManager()
        {
            CurrentLightnings =  new List<BranchLightning>();
        }

        public void Update()
        {
            foreach(BranchLightning bl in CurrentLightnings)
                bl.Update();

            CurrentLightnings.RemoveAll(bl => bl.IsComplete);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);

            foreach (BranchLightning bl in CurrentLightnings)
                bl.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
