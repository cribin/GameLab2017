using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Characters.Skills
{
    abstract class Skill
    {
        protected Character character;

	    protected Scene scene;

        protected long timeLastExecuted;
        protected int cooldown;

        // Acting character and cooldown (in Milliseconds) of this skill
        public Skill(Scene scene, Character character, int cooldown)
        {
	        this.scene = scene;
            this.character = character;
            this.timeLastExecuted = 0;
            this.cooldown = cooldown;
        }

    }
}
