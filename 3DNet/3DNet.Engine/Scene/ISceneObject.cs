using _3DNet.Engine.Rendering;
using System.Numerics;

namespace _3DNet.Engine.Scene
{
    public interface ISceneObject
    {
        string Name { get; }
        IScene Scene { get; }
        Vector3 Position { get;  }
        Vector3 Forward { get;  }
        Vector3 Up { get;  }
        Vector3 Right { get;  }

        void MoveForward(Vector3 relativeAmount);
        void MoveTo(Vector3 position);

        void Rotate(Vector3 axis,float angle);

    }
}
