using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGarden
{
    class Box : Volume
    {
        public Box()
        {
            VertCount = 8;
            IndiceCount = 36;
            VColorDataCount = 8;
        }

        public override Vector3[] GetVPosition_Data()
        {
            return new Vector3[] {
                new Vector3(-0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, 0.5f,  0.5f),
                new Vector3(-0.5f, 0.5f,  0.5f),
            };
        }

        public override Vector3[] GetVColor_Data()
        {
            Vector3 col = new Vector3(1f, 1f, 1f);
            return new Vector3[] {
                col,
                col,
                col,
                col,
                col,
                col,
                col,
                col};
        }

        public override int[] GetVIndex_Data(int offset = 0)
        {
            //      Vert Index data
            int[] inds = new int[] {
            //front
            0, 7, 3,
            0, 4, 7,
            //back
            1, 2, 6,
            6, 5, 1,
            //left
            0, 2, 1,
            0, 3, 2,
            //right
            4, 5, 6,
            6, 7, 4,
            //top
            2, 3, 6,
            6, 3, 7,
            //bottom
            0, 1, 5,
            0, 5, 4
            };

            if (offset != 0)
            {
                for (int i = 0; i < inds.Length; i++)
                {
                    inds[i] += offset;
                }
            }

            return inds;
        }

        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale)
                * Matrix4.CreateTranslation(Position);
        }

        public override void Tick()
        {
            return;
        }
    }
}
