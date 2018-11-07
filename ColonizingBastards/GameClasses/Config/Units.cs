using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.GameClasses.Config
{
    static class Units
    {

        // Unit size (1 ingame unit, i.e. 1 meter, is how many pixels in the window)
        public const float UNIT_SIZE = 100;

        /// <summary>
        /// Returns the scaled world-size-value to pixel-size.
        /// </summary>
        public static float W2P(float value)
        {
            return UNIT_SIZE * value;
        }

        /// <summary>
        /// Returns the pixel-size-value in world-size
        /// </summary>
        public static float P2W(float value)
        {
            return value / UNIT_SIZE;
        }

    }
}
