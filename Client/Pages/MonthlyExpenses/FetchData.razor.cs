using Cargotruck.Client.Services;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.MonthlyExpenses
{
    public partial class FetchData
    {
        public bool settings = false;
        bool expandExportMenu;
        public bool fullYearProfitWindow = true;
        MonthlyExpenseDto[]? Monthly_expenses { get; set; }
        MonthlyExpense_task_expenseDto[]? Connection_ids { get; set; }
        int? IdForGetById { get; set; }
        string? GetByIdType { get; set; }
        readonly List<bool> showColumns = Enumerable.Repeat(true, 6).ToList();
        private int currentPage = 1;
        int pageSize = 10;
        int dataRows;
        float maxPage;
        private string sortOrder = "Date";
        private bool desc = false;
        private string? searchString = "";
        readonly DateFilter? dateFilter = new();
        private bool showDeleteConfirmationWindow = false;
        private string? idForDelete;
        private readonly string controller = "monthlyexpenses";

        protected override async Task OnInitializedAsync()
        {
            dateFilter!.StartDate = new DateTime(DateTime.Today.Year, 1, 1, 0, 0, 0);

            PageHistoryState.AddPageToHistory("/MonthlyExpenses");
            base.OnInitialized();

            await client.PostAsync("api/monthlyexpenses/createcontable", null);
            dataRows = await client.GetFromJsonAsync<int>($"api/monthlyexpenses/pagecount?searchString={searchString}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            var checkData = await client.PostAsync("api/monthlyexpenses/checkdata", null);
            Connection_ids = await client.GetFromJsonAsync<MonthlyExpense_task_expenseDto[]?>("api/monthlyexpenses/getconnectionids");

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

            Monthly_expenses = await client.GetFromJsonAsync<MonthlyExpenseDto[]>($"api/monthlyexpenses/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            StateHasChanged();
        }

        void DeleteAsync(int Id)
        {
           idForDelete = Id.ToString();
           showDeleteConfirmationWindow = true;
        }

        public void CloseDeleteConfirmationWindow()
        {
            idForDelete = null;
            showDeleteConfirmationWindow = false;
        }

        public async Task RowIsDeleted(bool deleted)
        {
            if (deleted)
            {
                await ShowPageAsync();
            }
            else
            {
                throw new Exception(localizer["Error_on_delete"]);
            }
        }

        async void DateStartInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter!.StartDate = DateTime.Parse(e?.Value?.ToString()!);
                pageSize = 10;
                await ShowPageAsync();
            }
        }

        async void DateEndInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter!.EndDate = DateTime.Parse(e?.Value?.ToString()!);
                pageSize = 10;
                await ShowPageAsync();
            }
        }

        void GetById(int? id, string idType)
        {
            IdForGetById = id;
            GetByIdType = idType;
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
