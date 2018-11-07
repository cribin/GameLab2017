using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.ScreenUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.Base.Graphics
{
	abstract class HudElement : RenderObject
	{

		protected Hud hud;

		protected HudElement(Hud hud)
		{
			this.hud = hud;
		}

		public override void SetPosition(Vector3 position)
		{
			this.position = position;
		}

		public override void SetRotation(Vector3 rotation)
		{
			this.rotation = base.rotation;
		}

		public override void SetSize(Vector3 size)
		{
			this.size = size;
		}


	}
}
