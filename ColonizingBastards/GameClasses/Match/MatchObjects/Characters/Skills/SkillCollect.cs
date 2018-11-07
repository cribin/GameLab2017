using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Match.MatchObjects.Items;
using ColonizingBastards.GameClasses.Match.MatchSound;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Characters.Skills
{
    class SkillCollect : Skill
    {
        private CharacterPhysics physics;
        private Spritesheet charSheet;

        public SkillCollect(Scene scene, Character character, CharacterPhysics physics, int cooldown) : base(scene, character, cooldown)
        {
            this.physics = physics;
            this.charSheet = character.getRepresentation() as Spritesheet;
        }

        public void Execute(long currentTimeMs, List<Actor> touchedActors)
        {
			DefaultCharacter currentCharacter = (DefaultCharacter)character;
			if (currentCharacter == null)
				return;

			if ((currentTimeMs - timeLastExecuted) < cooldown)
                return;

            bool isFacingRight = physics.IsFacingRight();

	        if (currentCharacter.InvTreasure == null)
	        {
		        foreach (Actor actor in touchedActors)
		        {
			        Treasure treasure = actor as Treasure;
			        if (treasure != null)
			        {
				        treasure.CharacterCollect(character);
			        }

					AmmoCrate ammoCrate = actor as AmmoCrate;
					if (ammoCrate != null)
					{
						ammoCrate.CharacterCollect(character);
					}

			        Trap trap = actor as Trap;
			        if (trap != null)
			        {
				        trap.CharacterCollect(character);
			        }
		        }
	        }


			timeLastExecuted = currentTimeMs;
        }

    }
}
