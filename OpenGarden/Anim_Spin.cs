using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGarden
{
    class Anim_Spin : Animation
    {
        private Vector3 spinVector;

        public Anim_Spin(Volume parentVolume,
            Vector3 spinVector,
            double length,
            Sound sound = null) : base (parentVolume, length, sound)
        {
            this.spinVector = spinVector;
        }
        protected override void TickAnimation()
        {
            return;
        }
    }
}
