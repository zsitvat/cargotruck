using Cargotruck.Client.Services;
using Cargotruck.Shared.Models.Request;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Warehouses
{
    public partial class FetchData
    {
        public bool settings = false;
        bool expandExportMenu;
        Cargotruck.Shared.Models.Warehouses[]? Warehouses { get; set; }
        Cargotruck.Shared.Models.Cargoes[]? Cargoes { get; set; }
        int? IdForGetById { get; set; }
        string? GetByIdType { get; set; }
        readonly List<bool> showColumns = Enumerable.Repeat(true, 4).ToList();
        private int currentPage = 1;
        int pageSize = 10;
        int dataRows;
        float maxPage;
        private string sortOrder = "Date";
        private bool desc = true;
        private string? searchString = "";
        DateFilter? dateFilter = new();

        protected override async Task OnInitializedAsync()
        {
            PageHistoryState.AddPageToHistory("/Warehouses");
            base.OnInitialized();
            dataRows = await client.GetFromJsonAsync<int>("api/warehouses/pagecount");
            Cargoes = await client.GetFromJsonAsync<Cargotruck.Shared.Models.Cargoes[]?>("api/cargoes/getcargoes");
            await ShowPage();
        }

        protected async Task ShowPage()
        {
            pageSize = Page.GetPageSize(pageSize, dataRows);
            maxPage = Page.GetMaxPage(pageSize, dataRows);

            Warehouses = await client.GetFromJsonAsync<Cargotruck.Shared.Models.Warehouses[]>($"api/warehouses/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            StateHasChanged();
        }
        async Task Delete(int Id)
        {
            var data = Warehouses?.First(x => x.Id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["Delete?"]} {data?.Address} - {data?.Owner} ({data?.Id})"))
            {
                await client.DeleteAsync($"api/warehouses/delete/{Id}");
                var shouldreload = dataRows % ((currentPage == 1 ? currentPage : currentPage - 1) * pageSize);
                if (shouldreload == 1 && dataRows > 0) { navigationManager.NavigateTo("/Warehouses", true); }
                await OnInitializedAsync();
            }
        }

        async void DateStartInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter!.StartDate = DateTime.Parse(e.Value?.ToString()!);
                await OnInitializedAsync();
            }
        }

        async void DateEndInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter!.EndDate = DateTime.Parse(e.Value?.ToString()!);
                await OnInitializedAsync();
            }
        }

        void GetById(int? id, string? idType)
        {
            IdForGetById = id;
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
            await ShowPage();
        }

        protected async Task GetCurrentPage(int CurrentPage)
        {
            currentPage = CurrentPage;
            await ShowPage();
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
            await ShowPage();
        }

        protected async Task Search(ChangeEventArgs args)
        {
            searchString = args.Value?.ToString();
            await ShowPage();
        }

        private async void StateChanged()
        {
            await OnInitializedAsync();
        }
    }
}
