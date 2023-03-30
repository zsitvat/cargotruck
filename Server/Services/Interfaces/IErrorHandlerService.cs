namespace Cargotruck.Server.Services.Interfaces
{
    public interface IErrorHandlerService
    {
        string GetErrorMessageAsString(Exception ex);
    }
}