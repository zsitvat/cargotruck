using Cargotruck.Client.Services;
using Cargotruck.Client.UtilitiesClasses;
using Cargotruck.Shared.Models.Request;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Cargoes
{
    public partial class FetchData
    {
        public bool settings = false;
        bool expandExportMenu;
        Cargotruck.Shared.Models.Cargoes[]? Cargoes { get; set; }
        string? IdForGetById { get; set; }
        string? GetByIdType { get; set; }
        readonly List<bool> showColumns = Enumerable.Repeat(true, 9).ToList();
        private int currentPage = 1;
        int pageSize = 10;
        int dataRows;
        float maxPage;
        private string sortOrder = "Date";
        private bool desc = true;
        private string? searchString = "";
        string? filter = "";
        DateFilter? dateFilter = new();

        protected override async Task OnInitializedAsync()
        {
            PageHistoryState.AddPageToHistory("/Cargoes");
            base.OnInitialized();
            dataRows = await client.GetFromJsonAsync<int>($"api/cargoes/pagecount?searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            await ShowPageAsync();
        }
        protected async Task ShowPageAsync()
        {
            pageSize = Page.GetPageSize(pageSize, dataRows);
            maxPage = Page.GetMaxPage(pageSize, dataRows);

            Cargoes = await client.GetFromJsonAsync<Cargotruck.Shared.Models.Cargoes[]>($"api/cargoes/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            StateHasChanged();
        }

        async Task DeleteAsync(int Id)
        {
            var data = Cargoes?.First(x => x.Id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["DeleteAsync?"]} {data?.Task_id} - {data?.Description} ({data?.Id})"))
            {
                await client.DeleteAsync($"api/cargoes/delete/{Id}");
                var shouldreload = dataRows % ((currentPage == 1 ? currentPage : currentPage - 1) * pageSize);
                if (shouldreload == 1 && dataRows > 0) { navigationManager.NavigateTo("/Cargoes", true); }
                await OnInitializedAsync();
            }
        }

        async void DateStartInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter!.StartDate = DateTime.Parse(e?.Value?.ToString()!);
                pageSize = 10;
                await OnInitializedAsync();
            }
        }

        async void DateEndInput(ChangeEventArgs e)
        {
            if (e != null && e?.Value?.ToString() != "")
            {
                dateFilter!.EndDate = DateTime.Parse(e?.Value?.ToString()!);
                pageSize = 10;
                await OnInitializedAsync();
            }
        }

        void GetById(string? VRN, string idType)
        {
            IdForGetById = VRN;
            GetByIdType = idType;
            StateHasChanged();
        }

        public void SetToNull()
        {
            IdForGetById = null;
            GetByIdType = null;
        }

        public void SettingsClosed()
        {
            settings = !settings;
        }

        public static void SettingsChanged() { }

        public async void InputChanged(int ChangedPageSize)
        {
            pageSize = ChangedPageSize;
            currentPage = 1;
            await ShowPageAsync();
        }

        async void OnChangeResetFilter()
        {
            filter = "";
            pageSize = 10;
            await OnInitializedAsync();
        }

        protected async void Sorting(string column)
        {
            if (sortOrder == column)
            {
                desc = !desc;
            }
            else
            {
                sortOrder = column;
            }
            await ShowPageAsync();
        }

        protected async Task SearchAsync(ChangeEventArgs args)
        {
            searchString = args.Value?.ToString();
            await ShowPageAsync();
        }

        protected async Task GetCurrentPageAsync(int CurrentPage)
        {
            currentPage = CurrentPage;
            await ShowPageAsync();
        }

        private async void StateChanged()
        {
            pageSize = 10;
            await OnInitializedAsync();
        }
    }
}
