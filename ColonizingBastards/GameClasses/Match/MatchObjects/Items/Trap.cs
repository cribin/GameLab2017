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
	class Trap : Item
	{
		public readonly GameEventList TrapTouchEventList;
		public readonly GameEventList TrapCollectEventList;

		public Character Owner { get; protected set; }

		// Is not interactive and invisible if false
		public bool Active;
		public bool Loaded;

		private RenderObject closedTrapRep, openTrapRep;

		

		public Trap(RenderObject closedTrapRep, RenderObject openTrapRep, Character owner) : base(openTrapRep)
		{
			this.closedTrapRep = closedTrapRep;
			this.openTrapRep = openTrapRep;
			this.Owner = owner;
			SetRotation(Vector3.Zero);
			//TODO: Load from xml (not there yet)?
			SetSize(new Vector3(60, 35, 0));
			TrapTouchEventList = new GameEventList();
			TrapCollectEventList = new GameEventList();
		}

		public void CharacterTouch(Character actuatingCharacter)
		{
			if (Active)
			{
				TrapTouchEventList.Execute(new List<object>() { actuatingCharacter }, new List<object>() { this });
			}
		}

		public void CharacterCollect(Character actuatingCharacter)
		{
			if (Active)
			{
				TrapCollectEventList.Execute(new List<object>() { actuatingCharacter }, new List<object>() { this });
			}
		}
		

		public void Close()
		{
			closedTrapRep.SetPosition(rep.position);
			closedTrapRep.SetSize(rep.size);
			rep = closedTrapRep;
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
