namespace Cargotruck.Client.Services.Interfaces
{
    public interface IPageHistoryState
    {
        void AddPageToHistory(string pageName);
        bool CanGoBack();
        string? GetGoBackPage();
        bool GetPageIsVisited(string myPage);
        void ResetPageToHistory();
    }
}