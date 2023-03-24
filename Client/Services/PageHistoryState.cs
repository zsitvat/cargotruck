namespace Cargotruck.Client.UtilitiesClasses
{
    public class PageHistoryState : IPageHistoryState
    {
        private readonly List<string> previousPages;

        public PageHistoryState()
        {
            previousPages = new List<string>();
        }
        public void AddPageToHistory(string pageName)
        {
            previousPages.Add(pageName);
        }

        public void ResetPageToHistory()
        {
            previousPages.Clear();
        }

        public string? GetGoBackPage()
        {
            if (previousPages.Count > 1)
            {
                // You add a page on initialization, so you need to return the 2nd from the last
                return previousPages.ElementAt(previousPages.Count - 2);
            }

            // Can't go back because you didn't navigate enough
            return previousPages?.FirstOrDefault();
        }

        public bool GetPageIsVisited(string myPage)
        {
            if (previousPages.Count > 0)
            {
                foreach (var page in previousPages)
                {
                    if (page == myPage)
                    {
                        return true;
                    }
                }
                return false;
            }

            // Can't go back because you didn't navigate enough
            return false;
        }

        public bool CanGoBack()
        {
            return previousPages.Count > 1;
        }
    }
}
