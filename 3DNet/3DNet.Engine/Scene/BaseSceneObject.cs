using _3DNet.Engine.Rendering;
using _3DNet.Math;
using System;

namespace _3DNet.Engine.Scene
{
    internal abstract class BaseSceneObject : ISceneObject
    {
        private Vector3F _position = (0, 0, 0);
        private Vector3F _forward = (0, 0, 1);
        private Vector3F _up = (0, 1, 0);
        private Vector3F _right = (1, 0, 0);
        private Quaternion _rotationQuat = Quaternion.Identity;
        private Matrix4x4 _rotation = Matrix4x4.Identity;
        private Matrix4x4 _translation = Matrix4x4.Identity;
        private Matrix4x4 _scale = Matrix4x4.Identity;

        protected BaseSceneObject(IScene scene, string name)
        {
            Scene = scene;
            Name = name;
            ReCalculateTranslation();
            ReCalculateRotation();
            ReCalculateScale();
        }

        private void ReCalculateRotation()
        {
            _up = ((0, 1, 0, 0) * _rotationQuat).ToVector3F().Normalize();
            _forward = ((0, 0, 1, 0) * _rotationQuat).ToVector3F().Normalize();
            _right = ((1, 0, 0, 0) * _rotationQuat).ToVector3F().Normalize();
            _rotation = _rotationQuat.ToMatrix4x4();
        }

        private void ReCalculateScale()
        {
            _scale = Matrix4x4.Scale((1, 1, 1));
        }

        public IScene Scene { get; }
        public Matrix4x4 World { get; private set; }
        public string Name { get; }

        public Vector3F Position => _position;

        public Vector3F Forward => _forward;

        public Vector3F Up => _up;

        public Vector3F Right => _right;

        private void ReCalculateTranslation()
        {
            _translation = Matrix4x4.Translate(_position);
            ReCalculateWorld();
        }

        private void ReCalculateWorld()
        {
            World = _scale * _rotation * _translation;
        }

        public abstract void Render(IRenderEngine engine);

        public void MoveForward(Vector3F relativeAmount)
        {
            _position += relativeAmount;
            ReCalculateTranslation();
        }

        public void MoveTo(Vector3F position)
        {
            _position = position;
            ReCalculateTranslation();
        }

        public void Rotate(Vector3F axis, float angle)
        {
            _rotationQuat += Quaternion.CreateFromAxisAngle(axis, angle);
            ReCalculateRotation();
        }
    }
}