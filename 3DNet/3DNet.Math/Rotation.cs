using System;
using System.Numerics;

namespace _3DNet.Math;
public struct Rotation
{
    public Rotation()
    {
        Axis = new Vector3();
        Degrees = 0f;
    }
    public Rotation(Vector3 axis, float degrees) : this()
    {
        Axis = axis;
        Degrees = degrees;
    }

    public Vector3 Axis { get; set; }


    public float Degrees { get; set; }
    public readonly float Radians => Degrees * (MathF.PI / 180f); 
}
