using Cargotruck.Client.Services;
using Cargotruck.Shared.Models.Request;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Tasks
{
    public partial class FetchData
    {
        private bool settings = false;
        bool expandExportMenu;
        Cargotruck.Shared.Models.Tasks[]? Tasks { get; set; }
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

        protected override async Task OnInitializedAsync()
        {
            PageHistoryState.AddPageToHistory("/Tasks");
            base.OnInitialized();
            dataRows = await client.GetFromJsonAsync<int>($"api/tasks/pagecount?searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            await ShowPage();
        }

        async Task ChangeCompletion(Cargotruck.Shared.Models.Tasks task)
        {
            await client.PutAsJsonAsync($"api/tasks/changecompletion", task);
            await ShowPage();
        }

        async Task Delete(int Id)
        {
            var t = Tasks?.First(x => x.Id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["Delete?"]} {t?.Partner} ({t?.Id})"))
            {
                await client.DeleteAsync($"api/tasks/delete/{Id}");
                var shouldreload = dataRows % ((currentPage == 1 ? currentPage : currentPage - 1) * pageSize);
                if (shouldreload == 1 && dataRows > 0) { navigationManager.NavigateTo("/Tasks", true); }
                await OnInitializedAsync();
            }
        }

        protected async Task ShowPage()
        {
            pageSize = Page.GetPageSize(pageSize, dataRows);
            maxPage = Page.GetMaxPage(pageSize, dataRows);

            Tasks = await client.GetFromJsonAsync<Cargotruck.Shared.Models.Tasks[]>($"api/tasks/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
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
            pageSize = 10;
            await OnInitializedAsync();
        }

    }
}
