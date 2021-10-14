using _3DNet.Engine.Rendering.Buffer;

namespace _3DNet.Engine.Rendering.Model
{
    class ModelFactory : IModelFactory
    {
        public IModel CreateCube(float width, float height, float depth)
        {
            var vertexBuffer = new VertexBuffer<PositionOnlyVertex> {
            (-width/2,height/2,-depth-2) , (width/2,height/2,-depth-2),(-width/2,-height/2,-depth-2) , (width/2,-height/2,-depth-2),
            (-width/2,height/2,depth-2) , (width/2,height/2,depth-2),(-width/2,-height/2,depth-2) , (width/2,-height/2,depth-2)
            };
            var indexBuffer = new IndexBuffer
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
            return new SimpleModel(vertexBuffer, indexBuffer);
        }
    }
}
