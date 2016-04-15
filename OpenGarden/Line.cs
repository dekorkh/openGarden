using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGarden
{
    class Line : Volume
    {
        public Vector3 Color { get; set; }
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }

        public Line(Vector3 start, Vector3 end, Vector3? Color = null)
        {
            this.Color = Color ?? new Vector3(1f, 1f, 1f);
            VertCount = 2;
            IndiceCount = 2;
            VColorDataCount = 2;
            PrimType = PrimitiveType.Lines;
            Start = start;
            End = end;
        }

        public override void CalculateModelMatrix()
        {
            //Ignore all rotation/scale/position and rely only on start and end
            ModelMatrix = Matrix4.Identity;
        }

        public override Vector3[] GetVColor_Data()
        {
            return new Vector3[] { Color, Color };
        }

        public override int[] GetVIndex_Data(int offset = 0)
        {
            //  Vert Index data
            int[] inds = new int[] {0, 1};

            if (offset != 0)
            {
                for (int i = 0; i < inds.Length; i++)
                {
                    inds[i] += offset;
                }
            }

            return inds;
        }

        public override Vector3[] GetVPosition_Data()
        {
            return new Vector3[] { Start, End };
        }

        public override void Tick()
        {
            return;
        }
    }
}
