using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;
using OpenTK.Audio;

using OpenTK.Graphics.OpenGL;

namespace OpenGarden
{
    class Level_LocalSpin : Level
    {
        public override void LoadLevel(GameAudio GA)
        {
            Random rnd = new Random(System.Environment.TickCount);

            Vector3 dynbox_Pos = new Vector3(0f, 0f, -4f);
            Vector3 dynbox_target_Pos = new Vector3(0f, -1f, -4f);
            float boxsize = .9f;
            float targetsize = .3f;

            Sound ding = GA.CreateSoundSource(Path.Combine(Path.Combine("Data", "Sounds"), "Ding.wav"));

            Line xline = new Line(dynbox_Pos, Vector3.UnitX * boxsize, new Vector3(1f, 0f, 0f));
            Line yline = new Line(dynbox_Pos, Vector3.UnitY * boxsize, new Vector3(0f, 1f, 0f));
            Line zline = new Line(dynbox_Pos, Vector3.UnitZ * boxsize, new Vector3(0f, 0f, 1f));

            DynBox_Lines dynbox = new DynBox_Lines(new Vector3(0f, 1f, 0f));
            dynbox.Position = dynbox_Pos;
            dynbox.Scale = new Vector3(boxsize);
            dynbox.Target = new Vector3(-1f, -1f, -1f);

            DynBox_Lines dynbox_target = new DynBox_Lines(new Vector3(0f, 0f, 1f));
            dynbox_target.Position = dynbox_target_Pos;
            dynbox_target.Scale = new Vector3(targetsize);

            dynbox.animlist.Add(new Anim_Orbit(dynbox_target, 120f, new Vector3(0f, -1f, -4f), 1.5f, null));

            scene.Add(dynbox);
            scene.Add(dynbox_target);
            scene.Add(xline);
            scene.Add(yline);
            scene.Add(zline);
        }

        public override void Tick()
        {
            
            foreach (Volume v in scene)
            {
                v.Tick();
            }
            scene[0].Target = scene[1].Position;
            ((Line)scene[2]).End = ((DynBox_Lines)scene[0]).X_Axis * .2f;
            ((Line)scene[3]).End = ((DynBox_Lines)scene[0]).Y_Axis * .2f;
            ((Line)scene[4]).End = ((DynBox_Lines)scene[0]).Z_Axis * .2f;
        }
    }
}
