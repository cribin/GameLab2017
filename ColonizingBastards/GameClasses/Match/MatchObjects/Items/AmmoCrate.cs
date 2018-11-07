using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.GameClasses.GameEvent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Items
{
	class AmmoCrate : Item
	{

		public readonly GameEventList AmmoCrateCollectEventList;


		// Is not interactive and invisible if false
		public bool Active;

		public AmmoCrate(RenderObject rep) : base(rep)
		{
			SetRotation(Vector3.Zero);
			//TODO: Load from xml (not there yet)?
			SetSize(new Vector3(36, 44, 0));
			AmmoCrateCollectEventList = new GameEventList();
		}

		public void CharacterCollect(Character actuatingCharacter)
		{
			if (Active)
			{
				AmmoCrateCollectEventList.Execute(new List<object>() { actuatingCharacter }, new List<object>() { this });
			}
		}


		public override void Draw(SpriteBatch batch)
		{
			if (Active)
			{
				base.Draw(batch);
			}
			else
			{
				// Don't draw
			}

		}

	}
}
