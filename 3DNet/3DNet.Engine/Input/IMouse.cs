namespace _3DNet.Engine.Input;

public interface IMouse
{
    void Update();
    int X { get; }
    int Y { get; }
    float DeltaX { get; }
    float DeltaY { get; }
    float DeltaZ { get; }
    bool IsLeftButtonDown { get; }
    bool IsRightButtonDown { get; }
    bool IsMiddleButtonDown { get; }
}