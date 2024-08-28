using _3DNet.Engine.Rendering.Buffer;
using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.Buffer;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace _3DNet.Engine.Rendering.Model
{
    public abstract class ModelFactory : IModelFactory
    {
        private readonly IShaderFactory _shaderFactory;

        public ModelFactory(IShaderFactory shaderFactory)
        {
            _shaderFactory = shaderFactory;
        }

        protected IEnumerable<PositionOnlyVertex> CreateCubeVertices(float width, float height, float depth)
        => new PositionOnlyVertex[]
            {
                /*Front
                 *        4-------------5
                 *   0----|-------1     |
                 *   |    |       |     |
                 *   |    6-------|-----7
                 *   2------------3
                 *
                 *Back
                 *        1-------------0
                 *   5----|-------4     |
                 *   |    |       |     |
                 *   |    3-------|-----2
                 *   7------------6
                 *
                 *Left
                 *        5-------------1
                 *   4----|-------0     |
                 *   |    |       |     |
                 *   |    7-------|-----3
                 *   6------------2
                 *Right
                 *        0-------------4
                 *   1----|-------5     |
                 *   |    |       |     |
                 *   |    2-------|-----6
                 *   3------------7
                 */
                
                (-width/2,height/2,-depth/2) , (width/2,height/2,-depth/2),(-width/2,-height/2,-depth/2) , (width/2,-height/2,-depth/2),
                (-width/2,height/2,depth/2) , (width/2,height/2,depth/2),(-width/2,-height/2,depth/2) , (width/2,-height/2,depth/2)
            };
        
        
        protected IEnumerable<PositionOnlyVertex> CreatePlaneVertices(float width, float height)
        => new PositionOnlyVertex[]
            {
                /*Front
                 *        
                 *   0------------1     
                 *   |            |     
                 *   |            |
                 *   2------------3
                 
                 */
                
                (-width/2,height/2,0) , (width/2,height/2,0),(-width/2,-height/2,0) , (width/2,-height/2,0)
            };

        protected uint[] CreatePlaneIndices()
        => new uint[]
            {
                //front
                0,1,3,0,3,2
            };
        
        protected uint[] CreateCubeIndices()
        => new uint[]
            {
                //front
                0,1,3,0,3,2,
                //left
                4,0,2,4,2,6,
                //back
                5,4,6,5,6,7,
                //right
                1,5,7,1,7,3,
                //top
                4,5,1,4,1,0,
                //bottom
                2,3,7,2,7,6
            };
        public IModel CreateCube(string name, float width, float height, float depth)
          => new SimpleModel(
              CreateVertexBuffer(name, CreateCubeVertices(width, height, depth).ToArray()),
              CreateIndexBuffer(name, CreateCubeIndices()),
              _shaderFactory.DefaultShader,
              new Vector3(width, height, depth));
        protected abstract IBuffer CreateIndexBuffer(string name, uint[] indices);
        protected abstract IBuffer CreateVertexBuffer<T>(string name, IEnumerable<T> vertices) where T : struct;
        public IModel CreatePlane(string name, float width, float height)
             => new SimpleModel(
              CreateVertexBuffer(name, CreatePlaneVertices(width, height).ToArray()),
              CreateIndexBuffer(name, CreatePlaneIndices()),
              _shaderFactory.DefaultShader,
              new Vector3(width, height, 0));
    }
}
