using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Objects;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.Base.PlayerInteraction
{
    abstract class CharacterController
    {
        public List<Character> PossesedCharacters { get; protected set; }

        public CharacterController()
        {
            this.PossesedCharacters = new List<Character>();
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Possess(Character character);
    }
}
