using _3DNet.Engine.Rendering;
using _3DNet.Math;

namespace _3DNet.Engine.Scene
{
    public interface ISceneObject
    {
        string Name { get; }
        IScene Scene { get; }
        Vector3F Position { get;  }
        Vector3F Forward { get;  }
        Vector3F Up { get;  }
        Vector3F Right { get;  }

        void MoveForward(Vector3F relativeAmount);
        void MoveTo(Vector3F position);

        void Rotate(Vector3F axis,float angle);

        void Render(IRenderEngine engine);
    }
}
