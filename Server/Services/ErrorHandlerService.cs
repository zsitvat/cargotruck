using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Resources;
using Microsoft.Extensions.Localization;
using System.Linq.Dynamic.Core;

namespace Cargotruck.Server.Services
{
    public class ErrorHandlerService : IErrorHandlerService
    {
        private readonly IStringLocalizer<Resource> _localizer;

        public ErrorHandlerService(IStringLocalizer<Resource> localizer)
        {
            _localizer = localizer;
        }

        public string GetErrorMessageAsString(Exception ex)
        {
            if (ex.Message.Contains("datetime to int"))
            {
                return _localizer["Cant_convert_datetime_to_int"];
            }
            else if (ex.Message.Contains("string to int"))
            {
                return _localizer["Cant_convert_string_to_int"];
            }
            else if (ex.Message.Contains("string to int"))
            {
                return _localizer["Cant_convert_string_to_int"];
            }
            else if (ex.Message.Contains("int to datetime"))
            {
                return _localizer["Cant_convert_int_to_datetime"];
            }
            else if (ex.Message.Contains("valid DateTime"))
            {
                return _localizer["Cant_convert_string_to_datetime"];
            }
            else
            {
                return ex.Message;
            }
        }
    }
}
