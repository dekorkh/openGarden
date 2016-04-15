using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGarden
{
    //Basic scale animation
    class Anim_Grow : Animation
    {
        private double Speed { get; set; }
        public Anim_Grow(Volume parentVolume, 
            double speed = 1f, 
            double length = 0f,
            Sound sound = null) : base(parentVolume, length, sound)
        {
            Speed = speed;
        }

        protected override void TickAnimation()
        {
            //apply a scale transform over time
            Vector3 scale = new Vector3(1f + (float)(Speed * deltaT));
            parentVolume.Scale *= scale;
        }
    }
}
