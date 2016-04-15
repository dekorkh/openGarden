using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGarden
{
    class DynBox_Lines : DynVolume
    {
        public Vector3 Color { get; set; }
        private DynBox _dynbox = new DynBox();

        public DynBox_Lines(Vector3? Color = null)
        {
            this.Color = Color ?? new Vector3(1f, 1f, 1f);
            _dynbox.Color = this.Color;
            VertCount = 8;
            IndiceCount = 24;
            VColorDataCount = 8;
            PrimType = PrimitiveType.Lines;
        }

        public override Vector3[] GetVColor_Data()
        {
            return _dynbox.GetVColor_Data();
        }

        public override int[] GetVIndex_Data(int offset = 0)
        {
            //      Vert Index data
            int[] inds = new int[] {
            0, 1, //bottom back
            0, 3, //left back
            0, 4, //mid bottom left

            1, 2, //right back
            1, 5, //mid bottom right
            2, 3, //top back

            5, 6, //near right
            6, 2, //mid top right
            3, 7, //mid top left

            4, 7, //front left
            7, 6, //front top
            4, 5, //front bottom
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

        public override Vector3[] GetVPosition_Data()
        {
            return _dynbox.GetVPosition_Data();
        }

        public override void CalculateModelMatrix()
        {
            _dynbox.Position = Position;
            _dynbox.Scale = Scale;
            _dynbox.Target = Target;
            X_Axis = _dynbox.X_Axis;
            Y_Axis = _dynbox.Y_Axis;
            Z_Axis = _dynbox.Z_Axis;
            _dynbox.CalculateModelMatrix();
            ModelMatrix = _dynbox.ModelMatrix;
        }
    }
}
