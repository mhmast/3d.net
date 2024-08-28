namespace _3DNet.Engine.Rendering.Model
{
    public interface IModelFactory
    {
        IModel CreateCube(string name, float width, float height, float depth);
        IModel CreatePlane(string name, float width, float height);
    }
}
