using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.GameClasses.Match.Config;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Projectiles
{
    internal class Projectile : Actor
    {
        private Vector2 startPos;
        private readonly float directionX;
        private readonly float speedX;
        private readonly float maxDistance;
        public bool IsActive { get; private set; }

        public Projectile(RenderObject rep, Vector2 startPos, float directionX) : base(rep)
        {
            this.startPos = startPos;
            SetPosition(startPos);
            SetSize(ProjectileConfig.BULLET_SIZE);
            this.directionX = directionX;
            IsActive = true;

            speedX = ProjectileConfig.SPEED_X;
            maxDistance = ProjectileConfig.MAX_DISTANCE;
        }

        public void DestroyProjectile()
        {
            IsActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            var delta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            position.X += directionX * speedX * delta;
            SetPosition(position);
            if (Math.Abs(startPos.X - position.X) > maxDistance)
                IsActive = false;

            base.Update(gameTime);
        }
    }
}
