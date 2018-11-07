using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Items
{
	class Item : Actor
	{
		public Item(RenderObject rep) : base(rep)
		{
		}

		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);
		}
	}

}
