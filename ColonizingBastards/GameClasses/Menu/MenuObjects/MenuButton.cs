using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.GameClasses.Menu.MenuObjects
{
    class MenuButton:Actor
    {
        //bool indicates whether this button is currently selected or not
        public bool CurrentlySelected { get; set; }
        private float alpha = 0.5f;

        public MenuButton(RenderObject rep, Vector3 position, Vector3 size) : base(rep)
        {
            SetPosition(position);
            SetSize(size);
        }

        public void OnPress()
        {
            
        }

        public override void Draw(SpriteBatch batch)
        {
            if(CurrentlySelected)
                ((Sprite)rep).Draw(batch, alpha);
            else
                rep.Draw(batch);
            // maybe draw text

        }
    }
}

