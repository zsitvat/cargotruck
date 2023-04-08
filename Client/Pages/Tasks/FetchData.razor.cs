using Cargotruck.Client.Components;
using Cargotruck.Client.Services;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Data.Entity.Infrastructure;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;

namespace Cargotruck.Client.Pages.Tasks
{
    public partial class FetchData
    {
        private bool settings = false;
        bool expandExportMenu;
        List<DeliveryTaskDto>? Tasks { get; set; }
        int? IdForGetById { get; set; }
        string? GetByIdType { get; set; }
        private readonly List<bool> showColumns = Enumerable.Repeat(true, 16).ToList();
        private int currentPage = 1;
        int pageSize = 10;
        int dataRows;
        float maxPage;
        private string sortOrder = "Date";
        private bool desc = true;
        private string? searchString = "";
        string? filter = "";
        DateFilter? dateFilter = new();
        private bool showDeleteConfirmationWindow = false;
        private string? idForDelete;
        private readonly string controller = "tasks";
        protected override async Task OnInitializedAsync()
        {
            PageHistoryState.AddPageToHistory("/Tasks");
            base.OnInitialized();
            dataRows = await client.GetFromJsonAsync<int>($"api/tasks/pagecount?searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            await ShowPageAsync();
        }

        async Task ChangeCompletionAsync(DeliveryTaskDto task)
        {
            await client.PutAsJsonAsync($"api/tasks/changecompletion", task);
            await ShowPageAsync();
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

        protected async Task ShowPageAsync()
        {
            pageSize = Page.GetPageSize(pageSize, dataRows);
            maxPage = Page.GetMaxPage(pageSize, dataRows);

            Tasks = await client.GetFromJsonAsync<List<DeliveryTaskDto>>($"api/tasks/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            StateHasChanged();
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

        async void OnChangeResetFilter()
        {
            filter = "";
            pageSize = 10;
            await OnInitializedAsync();
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
