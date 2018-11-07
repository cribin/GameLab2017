using ColonizingBastards.Base.PlayerInteraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Ai
{
    abstract class AiController : CharacterController
    {
        protected Scene scene;
        protected NavGraph navGraph;

        // Nav-Graph and other critical references (scene & co.)

        public AiController(Scene scene, NavGraph navGraph)
        {
            this.scene = scene;
            this.navGraph = navGraph;
        }

        protected abstract ActionSet Simulate(GameTime gameTime);

        public override void Update(GameTime gameTime)
        {
            ActionSet actions = Simulate(gameTime);

            foreach (Character character in PossesedCharacters)
            {
                character.Update(gameTime, actions);
            }
        }

        public override void Possess(Character character)
        {
            this.PossesedCharacters.Add(character);
            character.AddPossession(this);
        }
    }
}
