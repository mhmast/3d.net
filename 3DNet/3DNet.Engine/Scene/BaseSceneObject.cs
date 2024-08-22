using _3DNet.Math.Extensions;
using _3DNet.Engine.Rendering;
using System;
using System.Numerics;

namespace _3DNet.Engine.Scene
{
    internal abstract class BaseSceneObject : ISceneObject, IRenderable
    {
        private Vector3 _position = new Vector3(0, 0, 0);
        private Vector3 _forward = new Vector3(0, 0, 1);
        private Vector3 _up = new Vector3(0, 1, 0);
        private Vector3 _right = new Vector3(1, 0, 0);
        private Quaternion _rotationQuat = Quaternion.Identity;
        private Matrix4x4 _rotation = Matrix4x4.Identity;
        private Matrix4x4 _scale = Matrix4x4.Identity;

        protected BaseSceneObject(Scene scene, string name)
        {
            SceneInternal = scene;
            Name = name;
            ReCalculateWorld();
        }

        protected virtual void OnTranslationChanged() { }
        protected virtual void OnRotationChanged() { }

        protected virtual void OnScaleChanged() { }

        public IScene Scene => SceneInternal;
        internal protected Scene SceneInternal { get; }
        public Matrix4x4 World { get; private set; }
        public string Name { get; }

        public Vector3 Position => _position;

        public Vector3 Forward => _forward;

        public Vector3 Up => _up;

        public Vector3 Right => _right;


        private void ReCalculateWorld()
        {
            World = new Matrix4x4
            {
                M11 = _right.X,
                M12 = _right.Y,
                M13 = _right.Z,
                M14 = 0,
                M21 = _up.X,
                M22 = _up.Y,
                M23 = _up.Z,
                M24 = 0,
                M31 = _forward.X,
                M32 = _forward.Y,
                M33 = _forward.Z,
                M34 = 0,
                M41 = _position.X,
                M42 = _position.Y,
                M43 = _position.Z,
                M44 = 1
            } ;
        }

        public abstract void Render(IRenderContextInternal context);

        public void MoveForward(Vector3 relativeAmount)
        {
            _position += relativeAmount;
            ReCalculateWorld();
            OnTranslationChanged();
        }

        public void MoveTo(Vector3 position)
        {
            _position = position;
            ReCalculateWorld();
            OnTranslationChanged();
        }

        public void LookAt(ISceneObject obj)
        {
            _forward = (Position - obj.Position).Normalize();
            _right = _up.Cross(_forward).Normalize();
            _up = _forward.Cross(_right);
            ReCalculateWorld();
            OnRotationChanged();
        }


        public void Rotate(Vector3 axis, float angle)
        {
            _rotationQuat += Quaternion.CreateFromAxisAngle(axis, angle);
            _up = (new Quaternion(0, 1, 0, 0) * _rotationQuat).ToVector3().Normalize();
            _forward = (new Quaternion(0, 0, 1, 0) * _rotationQuat).ToVector3().Normalize();
            _right = (new Quaternion(1, 0, 0, 0) * _rotationQuat).ToVector3().Normalize();
            ReCalculateWorld();
            OnRotationChanged();
        }
    }
}