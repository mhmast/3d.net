using _3DNet.Math.Extensions;
using _3DNet.Engine.Rendering;
using System;
using System.Numerics;
using System.Diagnostics;

namespace _3DNet.Engine.Scene
{
    internal abstract class BaseSceneObject : ISceneObject, IRenderable
    {
        private Vector3 _position = new Vector3(0, 0, 0);
        private Vector3 _forward = new Vector3(0, 0, 1);
        private Vector3 _up = new Vector3(0, 1, 0);
        private Vector3 _right = new Vector3(1, 0, 0);
        private Quaternion _rotationQuat = Quaternion.Identity;

        protected BaseSceneObject(Scene scene, string name)
        {
            SceneInternal = scene;
            Name = name;
            ReCalculateWorld();
        }

        protected virtual void OnTranslationChanged() { }
        protected virtual void OnRotationChanged()
        {
            var eulers = _rotationQuat.ToEulerAngles();
            Debug.WriteLine($"pitch: {eulers.X} yaw:{eulers.Y} roll:{eulers.Z}");
        }

        protected virtual void OnScaleChanged() { }

        protected virtual void OnWorldRecalculated() { }

        public IScene Scene => SceneInternal;
        internal protected Scene SceneInternal { get; }
        public Matrix4x4 World { get; protected set; }
        public string Name { get; }

        public Vector3 Position => _position;

        public Vector3 Forward => _forward;

        public Vector3 Up => _up;

        public Vector3 Right => _right;


        protected void ReCalculateWorld()
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
            };
            OnWorldRecalculated();
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


        public void Rotate(Vector3 axis, float degrees)
        {
            var rads = degrees * (MathF.PI / 180f);
            Debug.WriteLine($"Rotate {degrees} = rads {rads}");
            _rotationQuat = Quaternion.Concatenate(_rotationQuat, Quaternion.CreateFromAxisAngle(axis, rads)).Normalize();
            _up = Vector3.Transform(new Vector3(0, 1, 0), _rotationQuat);
            _forward = Vector3.Transform(new Vector3(0, 0, 1), _rotationQuat);
            _right = Vector3.Transform(new Vector3(1, 0, 0), _rotationQuat);
            ReCalculateWorld();
            OnRotationChanged();
        }


    }
}