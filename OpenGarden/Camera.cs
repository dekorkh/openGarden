using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace OpenGarden
{
    class Camera
    {
        private Vector3 target;
        private Vector3 origin;
        private float rotation;
        public Matrix4 ModelViewMatrix; //we can querry this for the camera

        public Camera(Vector3 target)   //construct via a vec3 for the target
        {
            this.target = target;
            origin = Vector3.Zero;
            SetMatrix();
        }

        public void Move(float x, float y, float z)
        {
            Move(new Vector3(x, y, z));
        }

        public void Move(Vector3 translation)
        {
            target += translation;
            origin += translation;
            SetMatrix();
        }

        public void Rotate(float degrees)
        {
            rotation += MathHelper.DegreesToRadians(degrees);
            SetMatrix();
        }

        public void Zoom(float zoom)
        {
            origin += new Vector3(zoom);
            SetMatrix();
        }

        private void SetMatrix()
        {
            //Create rot-matrix around up vector (z) - rotation is angle
            var rotationMatrix = Matrix4.CreateFromAxisAngle(Vector3.UnitY, rotation); // rotation around target
            //Move the caemra to target, rotate by the rotationmatrix, then move back out;
            var t_origin = Vector4.Transform(new Vector4(origin - target, 1), rotationMatrix) + new Vector4(target, 1);
            origin = t_origin.Xyz;
            //Create/Set the modeViewMatrix
            ModelViewMatrix = Matrix4.LookAt(origin, target, Vector3.UnitY);
            //ModelViewMatrix = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ * 4f, Vector3.UnitY);
        }
    }
}
