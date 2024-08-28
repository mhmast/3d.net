namespace _3DNet.Engine.Input;

public interface IKeyBoard
{
    void Update();
    bool IsButtonPressed(Key key);
}