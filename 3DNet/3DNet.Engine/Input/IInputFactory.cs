namespace _3DNet.Engine.Input;
public interface IInputFactory
{
    IKeyBoard GetKeyBoard();
    IMouse GetMouse();
}
