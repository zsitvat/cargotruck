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

        protected async Task ShowPageAsync()
        {
            await GetCurrentPage.InvokeAsync(CurrentPage);
        }

        protected async Task NextPageAsync()
        {
            CurrentPage++;
            await ShowPageAsync();
        }

        protected async Task SetCurrentPageAsync(int i)
        {
            CurrentPage = i;
            await ShowPageAsync();
        }

        protected async Task PrevPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await ShowPageAsync();
            }
        }
    }
}
