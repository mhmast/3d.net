using System.Numerics;

namespace _3DNet.Engine.Scene
{
    public interface ISceneObject
    {
        string Name { get; }
        IScene Scene { get; }
        Vector3 Position { get; }
        Vector3 Forward { get; }
        Vector3 Up { get; }
        Vector3 Right { get; }
    }
    public interface ISceneObject<out T> : ISceneObject where T : ISceneObject<T>
    {
        T MoveForward(Vector3 relativeAmount);
        T MoveTo(Vector3 position);
        T LookAt(ISceneObject obj);
        T Rotate(Vector3 axis,float angle);

    }
}
