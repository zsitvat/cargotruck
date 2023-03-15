namespace Cargotruck.Client.UtilitiesClasses
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