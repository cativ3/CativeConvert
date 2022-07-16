namespace CativeConvert.Commands.Abstractions
{
    internal interface ICommand<T>
    {
        byte[] Convert(List<T> list);
    }
}
