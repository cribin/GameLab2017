using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonizingBastards.GameClasses.Match.Config
{
	static class CharacterConfig
	{
	    
	    // Fill witch settings for the characters

        //scale = optimal frame rate
	    public const float SCALE = 60;

	    public const float BASE_ACC_X =  SCALE * SCALE;
	    public const float BASE_ACC_Y = 0.3f * SCALE * SCALE;
	   
        //the accl. if the player is carrying loot but not touching foliage
        public const float CARRY_ACC_X = 0.2f * BASE_ACC_X;
        //the accl. if the player is touching foliage and is not carrying anything
        public const float FOLIAGE_ACC_X = 0.1f * BASE_ACC_X;      
        //the accl. if the player touching foliage and carrying something
	    public const float FOLIAGE_AND_CARRY_ACC_X = 0.05f * BASE_ACC_X;//FOLIAGE_ACC_X;

        public const float CLIMB_SPEED_Y = 2.5f * SCALE;
        public const float CLIMB_SPEED_X = 0.01f * BASE_ACC_X; 

        public const float BASE_DEC_X = 4f * SCALE * SCALE;

        public const float MAX_SPEED_X = 3f * SCALE;
        public const float MAX_SPEED_X_CARRY = 2.5f * SCALE;
        public const float MAX_SPEED_X_FOLIAGE = 2f * SCALE;
        public const float MAX_SPEED_Y = 10.0f * SCALE; 

	    public const float JUMP_START_SPEED_Y = 8.0f * SCALE;
	    public const float CARRY_JUMP_SPEED_Y = 0.7f * JUMP_START_SPEED_Y;
	    public const float FOLIAGE_JUMP_SPEED_Y = 0.8f * JUMP_START_SPEED_Y;
	    public const float FOLIAGE_AND_CARRY_JUMP_SPEED_Y = 0.4f * JUMP_START_SPEED_Y;

        // Skill Config
	    public const int SKILL_COOLDOWN_SHOOT = 500;
        public const int SKILL_COOLDOWN_CUT = 250;
	    public const int SKILL_COOLDOWN_COLLECT = 100;
		public const int SKILL_COOLDOWN_TRAP = 500;

		public const int SKILL_DEPLOYTIME_TRAP = 3; // In seconds


		public const int SNAKE_STUN_TIME = 2; // In seconds
	    public static float HIT_STUN_TIME = 2;

        //Input sensibility settings
	    public const float INPUT_X_SENS = 0.5f;
        public const float INPUT_Y_SENS = 0.5f;

    }
}
