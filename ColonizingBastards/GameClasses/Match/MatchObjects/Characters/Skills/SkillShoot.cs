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
    class SkillShoot : Skill
    {
        private CharacterPhysics physics;
        private Spritesheet charSheet;

        public SkillShoot(Scene scene, Character character, CharacterPhysics physics, int cooldown) : base(scene, character, cooldown)
        {
            this.physics = physics;
            this.charSheet = character.getRepresentation() as Spritesheet;
        }

        public void Execute(long currentTimeMs)
        {
            if ((currentTimeMs - timeLastExecuted) < cooldown)
                return;

	        if ((character as DefaultCharacter)?.AmmoCount > 0)
	        {
		        (character as DefaultCharacter).AmmoCount--;
	        }
	        else
	        {
		        return;
	        }

			scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Shoot);

			bool isFacingRight = physics.IsFacingRight();

            Point bulletStartPos = physics.BoundingBox.Center;
            bulletStartPos.Y += 10;
            Projectile projectile;
            //Shoot bullet
            if (isFacingRight)
            {
                bulletStartPos.X += 40;
                projectile = new Projectile(new Sprite(ProjectileConfig.BULLET_TEXTURE),
                    bulletStartPos.ToVector2(), 1);
                charSheet?.PlayAnimation("shoot");
            }
            else
            {
                bulletStartPos.X -= 40;
                projectile = new Projectile(new Sprite(ProjectileConfig.BULLET_TEXTURE, true),
                    bulletStartPos.ToVector2(), -1);
                charSheet?.PlayAnimation("shoot", 1F, true);
            }

            physics.AddBullet(projectile);

            timeLastExecuted = currentTimeMs;
        }
    }
}
