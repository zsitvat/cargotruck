using Cargotruck.Client.Services;
using Cargotruck.Client.UtilitiesClasses;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Monthly_expenses
{
    public partial class FetchData
    {
        public bool settings = false;
        bool expandExportMenu;
        public bool fullYearProfitWindow = true;
        Cargotruck.Shared.Model.Monthly_expenses[]? Monthly_expenses { get; set; }
        Monthly_expenses_tasks_expenses[]? Connection_ids { get; set; }
        int? IdForGetById { get; set; }
        string? GetByIdType { get; set; }
        readonly List<bool> showColumns = Enumerable.Repeat(true, 6).ToList();
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
            PageHistoryState.AddPageToHistory("/Monthly_expenses");
            base.OnInitialized();

            await client.GetStringAsync("api/Monthly_expenses/createcontable");
            dataRows = await client.GetFromJsonAsync<int>($"api/Monthly_expenses/pagecount?searchString={searchString}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            var checkData = await client.GetAsync("api/Monthly_expenses/checkdata");
            Connection_ids = await client.GetFromJsonAsync<Monthly_expenses_tasks_expenses[]?>("api/Monthly_expenses/getconnectionids");

            if (checkData.IsSuccessStatusCode)
            {
                await ShowPageAsync();
                fileDownload.DocumentError = "";
            }
            else
            {
                fileDownload.DocumentError = localizer["CheckFailed"];
            }
        }

        protected async Task ShowPageAsync()
        {
            pageSize = Page.GetPageSize(pageSize, dataRows);
            maxPage = Page.GetMaxPage(pageSize, dataRows);

            Monthly_expenses = await client.GetFromJsonAsync<Cargotruck.Shared.Model.Monthly_expenses[]>($"api/Monthly_expenses/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            StateHasChanged();
        }

        async Task Delete(int Id)
        {
            var data = Monthly_expenses?.First(x => x.Monthly_expense_id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["DeleteAsync?"]} {data?.Earning} - {data?.Profit} ({data?.Monthly_expense_id})"))
            {
                await client.DeleteAsync($"api/Monthly_expenses/delete/{Id}");
                var shouldreload = dataRows % ((currentPage == 1 ? currentPage : currentPage - 1) * pageSize);
                if (shouldreload == 1 && dataRows > 0) { navigationManager.NavigateTo("/Monthly_expenses", true); }
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
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter!.EndDate = DateTime.Parse(e?.Value?.ToString()!);
                pageSize = 10;
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

        protected async Task GetCurrentPage(int CurrentPage)
        {
            currentPage = CurrentPage;
            await ShowPageAsync();
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

    }
}
