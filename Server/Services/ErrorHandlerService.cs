using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Resources;
using Microsoft.Data.SqlClient;
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
            else if (ex.Message.Contains("duplicate"))
            {
                return _localizer["Cant_save_duplicate"];
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
            else if (ex is IndexOutOfRangeException)
            {
                return _localizer["IndexOutOfRangeException"];
            }
            else if (ex is NullReferenceException)
            {
                return _localizer["NullReferenceException"];
            }
            else if (ex is DivideByZeroException)
            {
                return _localizer["DivideByZeroException"];
            }
            else if (ex is InvalidCastException)
            {
                return _localizer["InvalidCastException"];
            }
            else if (ex is OutOfMemoryException)
            {
                return _localizer["OutOfMemoryException"];
            }
            else if (ex is StackOverflowException)
            {
                return _localizer["StackOverflowException"];
            }
            else if (ex is ArrayTypeMismatchException)
            {
                return _localizer["ArrayTypeMismatchException"];
            }
            else if (ex is IOException)
            {
                return _localizer["IOException"];
            }
            else if (ex is SqlException)
            {
                return _localizer["SqlException"];
            }
            else
            {
                return ex.Message;
            }
        }
    }
}
