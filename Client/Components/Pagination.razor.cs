using Microsoft.AspNetCore.Components;

namespace Cargotruck.Client.Components
{
    public partial class Pagination
    {
        [Parameter] public int CurrentPage { get; set; }
        [Parameter] public int PageSize { get; set; }
        [Parameter] public int DataRows { get; set; }
        [Parameter] public float MaxPage { get; set; }
        [Parameter] public EventCallback<int> GetCurrentPage { get; set; }

        protected async Task ShowPage()
        {
            await GetCurrentPage.InvokeAsync(CurrentPage);
        }

        protected async Task NextPage()
        {
            CurrentPage++;
            await ShowPage();
        }

        protected async Task SetCurrentPage(int i)
        {
            CurrentPage = i;
            await ShowPage();
        }

        protected async Task PrevPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await ShowPage();
            }
        }
    }
}
