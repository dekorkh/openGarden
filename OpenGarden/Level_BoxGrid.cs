using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Audio;
using System.IO;

namespace OpenGarden
{
    class Level_BoxGrid : Level
    {
        public override void LoadLevel(GameAudio GA)
        {
            Random rnd = new Random(System.Environment.TickCount);
            int numBoxesAcross = 8;
            int numBoxesDown = 3;

            float aspectRatio = 1920f / 1080f;
            float gridW = 7.5f;
            float gridH = gridW / aspectRatio;
            float spaceW = gridW / numBoxesAcross;
            float spaceH = gridH / numBoxesDown;
            float left = (-gridW + spaceW) * .5f;
            float top = (-gridH + spaceH) * .5f;
            float boxsize = .4f;
            float animdelay = .4f;
            float spinLength = animdelay * numBoxesAcross * numBoxesDown;
            int boxindex = 0;
            int numIterations = 3;

            Sound ding = GA.CreateSoundSource(Path.Combine(Path.Combine("Data", "Sounds"), "Ding.wav"));

            for (int y = 0; y < numBoxesDown; y++)
            {
                for (int x = 0; x < numBoxesAcross; x++)
                {
                    DynBox dynbox = new DynBox(new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
                    dynbox.Position = new Vector3(left + spaceW * x, top + spaceH * y, -4f);
                    dynbox.Scale = new Vector3(boxsize);
                    dynbox.animlist.Add(new Anim_Blank(dynbox, animdelay * boxindex, ding));
                    for (int iter = 0; iter < numIterations; iter++)
                    {
                        //new float array of 0f to make the rot vector - set 1 random component to random rotation
                        float[] arr_rotation = new float[3] { 0f, 0f, 0f };
                        arr_rotation[rnd.Next(2)] = (float)(rnd.NextDouble() * Math.PI + Math.PI);
                        Vector3 vec_rotation = new Vector3(arr_rotation[0], arr_rotation[1], arr_rotation[2]);
                        dynbox.animlist.Add(new Anim_Spin(dynbox, vec_rotation, spinLength, ding));
                    }
                    scene.Add(dynbox);
                    boxindex++;
                }
            }
        }

        public override void Tick()
        {
            foreach (Volume v in scene)
            {
                v.Tick();
            }
        }
    }
}
