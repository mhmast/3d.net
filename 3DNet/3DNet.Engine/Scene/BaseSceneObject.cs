﻿using _3DNet.Math.Extensions;
using _3DNet.Engine.Rendering;
using System;
using System.Numerics;
using System.Diagnostics;
using _3DNet.Math;
using System.Linq;

namespace _3DNet.Engine.Scene
{
    internal abstract class BaseSceneObject<T> : ISceneObject<T>, IRenderable
        where T : ISceneObject<T>
    {
        private Vector3 _position = new Vector3(0, 0, 0);
        private Vector3 _forward = new Vector3(0, 0, 1);
        private Vector3 _up = new Vector3(0, 1, 0);
        private Vector3 _right = new Vector3(1, 0, 0);


        protected BaseSceneObject(Scene scene, string name)
        {
            SceneInternal = scene;
            Name = name;
            ReCalculateWorld();
        }

        protected virtual void OnTranslationChanged() { }
        protected virtual void OnRotationChanged()
        {

            // Debug.WriteLine($"pitch: {eulers.X} yaw:{eulers.Y} roll:{eulers.Z}");
        }

        protected virtual void OnScaleChanged() { }

        protected virtual void OnWorldRecalculated() { }

        protected abstract T Instance { get; }

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

        public T MoveForward(Vector3 relativeAmount)
        {
            _position += relativeAmount;
            ReCalculateWorld();
            OnTranslationChanged();
            return Instance;
        }

        public T MoveTo(Vector3 position)
        {
            _position = position;
            ReCalculateWorld();
            OnTranslationChanged();
            return Instance;
        }

        public T LookAt(ISceneObject obj)
        {
            _forward = (Position - obj.Position).Normalize();
            _right = _up.Cross(_forward).Normalize();
            _up = _forward.Cross(_right);
            ReCalculateWorld();
            OnRotationChanged();
            return Instance;
        }


        public T Rotate(params Rotation[] rotations)
        {
            var epsilon = 0.01f;

            var rotationQuat = rotations.Select(rotation => Quaternion.CreateFromAxisAngle(rotation.Axis, rotation.Radians)).Aggregate(Quaternion.Concatenate).Normalize();
            _up = Vector3.Transform(_up, rotationQuat).Normalize().ClampEpsilon(epsilon);
            _forward = Vector3.Transform(_forward, rotationQuat).Normalize().ClampEpsilon(epsilon);
            _right = Vector3.Transform(_right, rotationQuat).Normalize().ClampEpsilon(epsilon);

            //Debug.Assert(_right.Y == 0);

            ReCalculateWorld();
            OnRotationChanged();
            return Instance;
        }


    }
}