using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGarden
{
    class DynBox : DynVolume
    {
        public Vector3 Color { get; set; }

        public DynBox(Vector3? Color = null)
        {
            this.Color = Color ?? new Vector3(1f, 1f, 1f);
            VertCount = 8;
            IndiceCount = 36;
            VColorDataCount = 8;
            PrimType = PrimitiveType.Triangles;
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
            return new Vector3[] {
                this.Color,
                this.Color,
                this.Color,
                this.Color,
                this.Color,
                this.Color,
                this.Color,
                this.Color};
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
            ModelMatrix = Matrix4.CreateScale(Scale);
            Z_Axis = (Target - Position).Normalized();
            X_Axis = Vector3.Cross(Vector3.UnitY, Z_Axis).Normalized();
            Y_Axis = Vector3.Cross(X_Axis, Z_Axis).Normalized();
            ModelMatrix *= new Matrix4(new Vector4(X_Axis, 0),
                new Vector4(Y_Axis, 0),
                new Vector4(Z_Axis, 0),
                new Vector4(Position, 1));
            //ModelMatrix *= Matrix4.LookAt(Position, Target, Vector3.UnitY);
            ModelMatrix *= Matrix4.CreateTranslation(Position);
        }
    }
}



