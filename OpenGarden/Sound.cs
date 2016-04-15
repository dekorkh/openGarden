using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace OpenGarden
{
    public class Sound
    {
        public string filename;
        public int source;
        public Sound(string filename, int source)
        {
            this.filename = filename;
            this.source = source;
        }
        public void Play()
        {
            AL.SourcePlay(source);
        }
        ~Sound()
        {
            AL.SourceStop(source);
            AL.DeleteSource(source);
        }
    }
}
