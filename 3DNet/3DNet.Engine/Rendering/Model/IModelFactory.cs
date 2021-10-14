namespace _3DNet.Engine.Rendering.Model
{
    public interface IModelFactory
    {
        IModel CreateCube(float width, float height, float depth);
    }
}
