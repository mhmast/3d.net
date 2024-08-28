using System.Numerics;

namespace _3DNet.Engine.Scene;

public interface IStandardSceneObject : ISceneObject<IStandardSceneObject>
{
    IStandardSceneObject Resize(Vector3 boundingBox);
}