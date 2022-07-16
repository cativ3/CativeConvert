using CativeConvert.Utilities;

namespace CativeConvert
{
    public interface IConverter
    {
        byte[] Convert<T>(List<T> list, ConvertTo fileType);
    }
}
