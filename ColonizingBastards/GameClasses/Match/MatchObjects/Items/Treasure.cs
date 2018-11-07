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


	class Treasure : Item
	{

		public readonly GameEventList TreasureCollectEventList;


		// Is not interactive and invisible if false
		public bool Active;

		public Treasure(RenderObject rep) : base(rep)
		{
			SetRotation(Vector3.Zero);
			//TODO: Load from xml (not there yet)?
            Sprite sprite = rep as Sprite;
           
            if (sprite != null)
		    {
                SetSize(new Vector3(sprite.Texture.Width, sprite.Texture.Height, 0f));
                
            }
		    else
		    {
                //TODO :set size if treasure isn't represented by a sprite
               
            }

            TreasureCollectEventList = new GameEventList();
        }

		public void CharacterCollect(Character actuatingCharacter)
		{
			if (Active)
			{
				TreasureCollectEventList.Execute(new List<object>() { actuatingCharacter }, new List<object>() { this });
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
