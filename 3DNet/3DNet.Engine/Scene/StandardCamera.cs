using _3DNet.Engine.Rendering;
using _3DNet.Math;
using System;

namespace _3DNet.Engine.Scene
{
    internal class StandardCamera : BaseSceneObject, ICamera
    {
        private Vector3F _up = (0, 1, 0);
        private Vector3F _zaxis;
        private Vector3F _xaxis;
        private Vector3F _yaxis;
        private Matrix4x4 _view;

        public StandardCamera(Scene scene, string name) : base(scene, name)
        {
        }

        public void LookAt(ISceneObject obj)
        {
            _zaxis = (Position - obj.Position).Normalize();// The "forward" vector.
            _xaxis = _up.Cross(_zaxis).Normalize();// The "right" vector.
            _yaxis = _zaxis.Cross(_xaxis);     // The "up" vector.
            ReCalculateviewMatrix();
            }

        private void ReCalculateviewMatrix()
        {

            // Create a 4x4 orientation matrix from the right, up, and forward vectors
            // This is transposed which is equivalent to performing an inverse 
            // if the matrix is orthonormalized (in this case, it is).
            var orientation = new Matrix4x4(
               (_xaxis.X, _yaxis.X, _zaxis.X, 0),
               (_xaxis.Y, _yaxis.Y, _zaxis.Y, 0),
               (_xaxis.Z, _yaxis.Z, _zaxis.Z, 0),
               (0, 0, 0, 1));

            // Create a 4x4 translation matrix.
            // The eye position is negated which is equivalent
            // to the inverse of the translation matrix. 
            // T(v)^-1 == T(-v)


            var translation = new Matrix4x4(
                (1, 0, 0, 0),
                (0, 1, 0, 0),
                (0, 0, 1, 0),
                -Position.ToVerctor4F());
            // Combine the orientation and translation to compute 
            // the final view matrix. Note that the order of 
            // multiplication is reversed because the matrices
            // are already inverted.


            _view = (orientation * translation);
        }

        public override void Render(IRenderEngine engine)
        {
            engine.SetView(_view);
        }
    }
}