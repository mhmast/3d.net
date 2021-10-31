namespace _3DNet.Engine.Scene
{
    public interface ICamera : ISceneObject
    {
        void LookAt(ISceneObject obj);
    }
}