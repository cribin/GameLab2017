using ColonizingBastards.Base.MiscUtil;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.Objects
{
	abstract class Entity
	{

        long id { get; }

		public Entity()
		{
            id = IdHandler.GetUniqueId();
		}

        public abstract void Update(GameTime gameTime);

	}
}
