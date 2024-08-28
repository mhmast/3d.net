namespace _3DNet.Engine.Scene
{
    public interface ICamera : ISceneObject<ICamera>
    {
        ICamera SetActiveCamera();
    }
}