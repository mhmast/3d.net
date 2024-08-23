using System.Numerics;

namespace _3DNet.Engine.Scene;

public interface IStandardSceneObject : ISceneObject
{
    void Resize(Vector3 boundingBox);
}