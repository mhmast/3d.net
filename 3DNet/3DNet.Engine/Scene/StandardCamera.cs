using _3DNet.Engine.Rendering;
using System.Numerics;

namespace _3DNet.Engine.Scene
{
    internal class StandardCamera : BaseSceneObject, ICamera
    {
        private Matrix4x4 _view;

        public StandardCamera(Scene scene, string name) : base(scene, name)
        {
        }

        protected override void OnRotationChanged() => ReCalculateviewMatrix();

        protected override void OnTranslationChanged() => ReCalculateviewMatrix();
        private void ReCalculateviewMatrix()
        {

            //// Create a 4x4 orientation matrix from the right, up, and forward vectors
            //// This is transposed which is equivalent to performing an inverse 
            //// if the matrix is orthonormalized (in this case, it is).
            //var orientation = new Matrix4x4(
            //   Right.X, Up.X, Forward.X, 0,
            //   Right.Y, Up.Y, Forward.Y, 0,
            //   Right.Z, Up.Z, Forward.Z, 0,
            //   0, 0, 0, 1);

            //// Create a 4x4 translation matrix.
            //// The eye position is negated which is equivalent
            //// to the inverse of the translation matrix. 
            //// T(v)^-1 == T(-v)


            //var translation = new Matrix4x4(
            //    1, 0, 0, 0,
            //    0, 1, 0, 0,
            //    0, 0, 1, 0,
            //    -Position.X, -Position.Y, -Position.Z, 0);
            //// Combine the orientation and translation to compute 
            //// the final view matrix. Note that the order of 
            //// multiplication is reversed because the matrices
            //// are already inverted.


            _view = Matrix4x4.CreateLookAt(Position, Forward, Up);
        }

        public override void Render(IRenderContextInternal context)
        {
            context.SetView(_view);
        }

        public void SetActiveCamera() => SceneInternal.SetActiveCamera(this);
    }
}