using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace OpenGarden
{
    class Anim_Orbit : Animation
    {
        Vector3 Center { get; set; } = new Vector3(0f, 0f, 0f);
        double OrbitRadius { get; set; } = 1f;
        public Anim_Orbit(Volume parentVolume, 
            double length, 
            Vector3 center,
            double orbitRadius,
            Sound sound) : base(parentVolume, length, sound)
        {
            Center = center;
            OrbitRadius = orbitRadius;
        }
        protected override void TickAnimation()
        {
            parentVolume.Position = Center + new Vector3((float)(Math.Sin(elapsedT) * OrbitRadius),
                (float)(Math.Sin(elapsedT * 5) * .3), 
                (float)(Math.Cos(elapsedT) * OrbitRadius));
        }
    }
}
