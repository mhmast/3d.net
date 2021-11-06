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
            _rotation = _rotationQuat.ToMatrix4x4();
            OnRotationChanged();
            ReCalculateWorld();
        }

        protected virtual void OnRotationChanged() { }

        private void ReCalculateScale()
        {
            _scale = Matrix4x4.Scale((1, 1, 1));
            OnScaleChanged();
            ReCalculateWorld();
        }

        protected virtual void OnScaleChanged() { }

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
            OnTranslationChanged();
            ReCalculateWorld();
        }

        protected virtual void OnTranslationChanged() { }

        private void ReCalculateWorld()
        {
            World = _scale * _rotation * _translation;
        }

        public abstract void Render(IRenderWindowContext context);

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

        public void LookAt(ISceneObject obj)
        {
            _forward = (Position - obj.Position).Normalize();
            _right = _up.Cross(_forward).Normalize();
            _up = _forward.Cross(_right);
            _rotationQuat = Quaternion.CreateFromAxisAngle(_forward, 0) + Quaternion.CreateFromAxisAngle(_right, 0) + Quaternion.CreateFromAxisAngle(_up, 0);
            ReCalculateRotation();
        }


        public void Rotate(Vector3F axis, float angle)
        {
            _rotationQuat += Quaternion.CreateFromAxisAngle(axis, angle);
            _up = ((0, 1, 0, 0) * _rotationQuat).ToVector3F().Normalize();
            _forward = ((0, 0, 1, 0) * _rotationQuat).ToVector3F().Normalize();
            _right = ((1, 0, 0, 0) * _rotationQuat).ToVector3F().Normalize();
            ReCalculateRotation();
        }
    }
}