using CativeConvert.Commands;
using CativeConvert.Commands.Abstractions;
using CativeConvert.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CativeConvert
{
    public class ModelConverter : IConverter
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ModelConverter()
        {

        }

        public ModelConverter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public byte[] Convert<T>(List<T> list, ConvertTo fileType)
        {
            ICommand<T>? command;

            if(_httpContextAccessor == null)
            {
                switch (fileType)
                {
                    case ConvertTo.PDF:
                        command = new ConvertToPdfCommand<T>();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                switch (fileType)
                {
                    case ConvertTo.PDF:
                        command = _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<IConvertToPdfCommand<T>>();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            var bytes = command?.Convert(list);

            return bytes;
        }
    }
}
