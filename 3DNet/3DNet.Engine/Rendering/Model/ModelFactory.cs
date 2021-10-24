using _3DNet.Engine.Rendering.Buffer;

namespace _3DNet.Engine.Rendering.Model
{
    public abstract class ModelFactory : IModelFactory
    {
    
        protected PositionOnlyVertex[] CreateCubeVertices(float width, float height, float depth)
        => new PositionOnlyVertex[]
            {
                (-width/2,height/2,-depth-2) , (width/2,height/2,-depth-2),(-width/2,-height/2,-depth-2) , (width/2,-height/2,-depth-2),
                (-width/2,height/2,depth-2) , (width/2,height/2,depth-2),(-width/2,-height/2,depth-2) , (width/2,-height/2,depth-2)
            };
        
        protected int[] CreateCubeIndices()
        => new []
            {
                //front
                0,1,2,0,2,3,
                //left
                4,0,3,4,3,6,
                //back
                5,4,7,5,7,6,
                //right
                1,5,7,1,7,3,
                //top
                4,1,0,4,5,1,
                //bottom
                2,6,7,2,6,3
            };
        public abstract IModel CreateCube(float width, float height, float depth);
    }
}
