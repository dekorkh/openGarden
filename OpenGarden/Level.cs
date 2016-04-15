using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGarden
{
    public abstract class Level
    {
        public List<Volume> scene;
        public Level()
        {
            scene = new List<Volume>();
        }
        public abstract void LoadLevel(GameAudio GA);
        public abstract void Tick();
    }
}
