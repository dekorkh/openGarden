using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGarden
{
    class Anim_Blank : Animation
    {
        public Anim_Blank(Volume parentVolume, 
            double length, 
            Sound sound = null) : base(parentVolume, length, sound)
        {
            return;
        }
        protected override void TickAnimation()
        {
            return;
        }
    }
}
