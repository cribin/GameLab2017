using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation;
using ColonizingBastards.GameClasses.Match.MatchSound;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Characters.Skills
{
    class SkillCut : Skill
    {
        private CharacterPhysics physics;
        private Spritesheet charSheet;

        public SkillCut(Scene scene, Character character, CharacterPhysics physics, int cooldown) : base(scene, character, cooldown)
        {
            this.physics = physics;
            this.charSheet = character.getRepresentation() as Spritesheet;
        }

        public void Execute(long currentTimeMs, List<Actor> touchedActors) 
        {
            if ((currentTimeMs - timeLastExecuted) < cooldown)
                return;

			scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Machete);

			bool isFacingRight = physics.IsFacingRight();

            foreach (Actor actor in touchedActors)
            {
                (actor as Foliage)?.CharacterCut(character);
                //(actor as DefaultCharacter)?.CharacterStun(character);
            }

            if (isFacingRight)
                charSheet?.PlayAnimation("machete");
            else
                charSheet?.PlayAnimation("machete", 1F, true);


            timeLastExecuted = currentTimeMs;
        }
    }
}
