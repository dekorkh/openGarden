using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OpenGarden
{
    //Abstract animation class
    public abstract class Animation
    {
        protected Volume parentVolume;            //Reference to the parent volume to do transformations
        private Stopwatch stopwatch;            //Used to track time
        protected double elapsedT;                //Keeps elapsed time
        protected double deltaT;                  //Keeps delta frame time
        private bool _playing;                  //True when animation is playing
        private bool _finished;                 //True if animation has finished, set to false on play
        private Sound sound;                    //The sound if any to play at the start of animation
        public double Length { get; set; }      //The animation length
               
        //Constructor
        public Animation(Volume parentVolume, double length = 0f, Sound sound = null)
        {
            this.parentVolume = parentVolume;
            this.sound = sound;
            this._finished = false;
            Length = length;
        }

        //Happens every frame
        public void Tick()
        {
            //Initialize the stopwatch if it's not yet
            if (!_playing)
                return;
            if (stopwatch == null)
                stopwatch = new Stopwatch();

            //Update elapsed time
            deltaT = stopwatch.Elapsed.TotalSeconds;
            elapsedT += deltaT;

            //Do TickAnimation.
            TickAnimation();

            //Play including or one frame past the length of animation.
            if (elapsedT >= Length)
                Stop();

            //Restart the stopwatch
            stopwatch.Restart();
        }

        //Implement transform animation on parent volume in here
        protected abstract void TickAnimation();   

        //Return true if the animation is playing.
        public bool Playing
        {
            get
            {
                return _playing;
            }
        }

        //Begin playing the animation
        public void Play()
        {
            _playing = true;
            _finished = false;
            if (sound != null)
                sound.Play();
        }

        //Stop playing the animation
        public void Stop()
        {
            _playing = false;
            _finished = true;
        }

        public bool Finished
        {
            get
            {
                return _finished;
            }
        }
    }
}
