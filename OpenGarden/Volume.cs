using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGarden
{
    public abstract class Volume
    {
        public Vector3 X_Axis { get; set; } = Vector3.UnitX;
        public Vector3 Y_Axis { get; set; } = Vector3.UnitY;
        public Vector3 Z_Axis { get; set; } = Vector3.UnitZ;

        public Vector3 Target
        {
            get; set;
        } = Vector3.UnitZ;

        public Vector3 Position
        {
            get; set;
        }  = Vector3.Zero;

        public Vector3 Scale
        {
            get; set;
        } = Vector3.One;

        public PrimitiveType PrimType
        {
            get; set;
        } = PrimitiveType.Points;

        public int VertCount;
        public int IndiceCount;
        public int VColorDataCount;
        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

        public abstract Vector3[] GetVPosition_Data();
        public abstract int[] GetVIndex_Data(int offset = 0);
        public abstract Vector3[] GetVColor_Data();
        public abstract void CalculateModelMatrix();

        //Runs every frame
        public abstract void Tick();
    }
}
