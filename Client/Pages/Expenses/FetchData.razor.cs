using Cargotruck.Client.Services;
using Cargotruck.Shared.Models.Request;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Expenses
{
    public partial class FetchData
    {
        public bool settings = false;
        bool expandExportMenu;
        Cargotruck.Shared.Models.Expenses[]? expenses;
        int? IdForGetById { get; set; }
        string? GetByIdType { get; set; }
        readonly List<bool> showColumns = Enumerable.Repeat(true, 12).ToList();
        private int currentPage = 1;
        int pageSize = 10;
        int dataRows;
        float maxPage;
        private string sortOrder = "Date";
        private bool desc = true;
        private string? searchString = "";
        Cargotruck.Shared.Models.Type? filter;
        DateFilter dateFilter = new();

        protected override async Task OnInitializedAsync()
        {
            PageHistoryState.AddPageToHistory("/Expenses");
            base.OnInitialized();

            dataRows = await client.GetFromJsonAsync<int>($"api/expenses/pagecount?searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            await ShowPage();
        }

        protected async Task ShowPage()
        {
            pageSize = Page.GetPageSize(pageSize, dataRows);
            maxPage = Page.GetMaxPage(pageSize, dataRows);

            expenses = await client.GetFromJsonAsync<Cargotruck.Shared.Models.Expenses[]>($"api/expenses/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            StateHasChanged();
        }

        async Task Delete(int Id)
        {
            var data = expenses?.First(x => x.Id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["Delete?"]} {data?.Type} - {data?.Type_id} - {data?.Date}"))
            {
                await client.DeleteAsync($"api/expenses/delete/{Id}");
                var shouldreload = dataRows % ((currentPage == 1 ? currentPage : currentPage - 1) * pageSize);
                if (shouldreload == 1 && dataRows > 0) { navigationManager.NavigateTo("/Expenses", true); }
                await OnInitializedAsync();
            }
        }

        async void DateStartInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter.StartDate = DateTime.Parse(e?.Value?.ToString()!);
                await OnInitializedAsync();
            }
        }

        async void DateEndInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter.EndDate = DateTime.Parse(e?.Value?.ToString()!);
                await OnInitializedAsync();
            }
        }

        void GetById(int? id, string idType)
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

        async void OnChangeGetFilter(ChangeEventArgs e)
        {
            filter = e.Value switch
            {
                "salary" => (Cargotruck.Shared.Models.Type?)Cargotruck.Shared.Models.Type.salary,
                "task" => (Cargotruck.Shared.Models.Type?)Cargotruck.Shared.Models.Type.task,
                "storage" => (Cargotruck.Shared.Models.Type?)Cargotruck.Shared.Models.Type.storage,
                "repair" => (Cargotruck.Shared.Models.Type?)Cargotruck.Shared.Models.Type.repair,
                "other" => (Cargotruck.Shared.Models.Type?)Cargotruck.Shared.Models.Type.other,
                _ => null,
            };
            await OnInitializedAsync();
        }

        async void OnChangeResetFilter()
        {
            filter = null;
            await OnInitializedAsync();
        }

        public static void SettingsChanged() { }

        public async void InputChanged(int ChangedPageSize)
        {
            pageSize = ChangedPageSize;
            currentPage = 1;
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

        protected async Task GetCurrentPage(int CurrentPage)
        {
            currentPage = CurrentPage;
            await ShowPage();
        }

        private async void StateChanged()
        {
            await OnInitializedAsync();
        }

    }
}
