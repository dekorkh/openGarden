using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGarden
{
    //An abstract volume class that has tick
    public abstract class DynVolume : Volume
    {
        public List<Animation> animlist = new List<Animation>(32);
        private Animation Animation { get; set; }
        private int _animlist_position = 0;

        public override void Tick()
        {
            if (_animlist_position < animlist.Count)
            {
                //play if animation is not playing.
                if (!animlist[_animlist_position].Playing)
                    animlist[_animlist_position].Play();
                //tick the animation
                animlist[_animlist_position].Tick();
                //if animation is finished after tick, increment animlist_pos
                if (animlist[_animlist_position].Finished)
                    _animlist_position++;
            }
        }
    }
}