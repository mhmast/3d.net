namespace _3DNet.Engine.Scene
{
    public interface ICamera : ISceneObject
    {
        void SetActiveCamera();
        void LookAt(ISceneObject obj);
    }
}