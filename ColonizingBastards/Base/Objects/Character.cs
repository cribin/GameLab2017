using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Graphics;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.Base.Objects
{
	class Character : Actor
	{

        protected CharacterController possessedBy;

		public Character(RenderObject rep) : base(rep)
		{
                   
		}

		public virtual void InitCharacter(Vector3 position)
		{

		}

		public virtual void InitCharacter()
		{

		}

		public virtual void Update(GameTime gameTime, ActionSet actions)
		{
			base.Update(gameTime);
		}


		public void AddPossession(CharacterController player)
		{
			this.possessedBy = player;
		}

		public CharacterController GetPosession()
		{
			return possessedBy;
		}
	}
}
