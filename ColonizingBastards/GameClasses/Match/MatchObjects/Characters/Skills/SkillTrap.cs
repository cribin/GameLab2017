using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Match.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects.Projectiles;
using ColonizingBastards.GameClasses.Match.MatchSound;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Characters.Skills
{
	class SkillTrap : Skill
	{
		private CharacterPhysics physics;
		private DefaultCharacter character;

		public SkillTrap(Scene scene, Character character, CharacterPhysics physics, int cooldown) : base(scene, character, cooldown)
        {
			this.physics = physics;
	        this.character = (DefaultCharacter) character;
        }

		public void Execute(long currentTimeMs)
		{
			if ((currentTimeMs - timeLastExecuted) < cooldown)
				return;

			physics.IsLayingTrap = true;
			physics.CurrentStunTime = CharacterConfig.SKILL_DEPLOYTIME_TRAP;

			//scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Shoot);

		    timeLastExecuted = currentTimeMs;
		}

		public void ConcludePlacing()
		{			
			character.TrapDropEventList.Execute(new List<object>() { character.CharacterPhysics.TrapPlacementPos }, new List<object>() { });		
		}
	}

}
