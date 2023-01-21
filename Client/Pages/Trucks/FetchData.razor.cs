using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Trucks
{
    public partial class FetchData
    {
        public bool settings = false;
        Cargotruck.Shared.Models.Trucks[]? trucks { get; set; }
        int? IdForGetById { get; set; }
        string? getByIdType { get; set; }
        List<bool> showColumns = Enumerable.Repeat(true, 16).ToList();
        private int currentPage = 1;
        int pageSize = 10;
        int dataRows;
        float maxPage;
        private string sortOrder = "Date";
        private bool desc = true;
        private string? searchString = "";
        string? document_error;
        string? filterIcon;
        Status? filter;
        DateFilter? dateFilter = new();

        async void DateStartInput(ChangeEventArgs e)
        {
            if (e != null && e.Value.ToString() != "")
            {
                dateFilter.StartDate = DateTime.Parse(e.Value.ToString());
                await OnInitializedAsync();
            }
        }

        async void DateEndInput(ChangeEventArgs e)
        {
            if (e != null && e.Value.ToString() != "")
            {
                dateFilter.EndDate = DateTime.Parse(e.Value.ToString());
                await OnInitializedAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            PageHistoryState.AddPageToHistory("/Trucks");
            base.OnInitialized();
            dataRows = await client.GetFromJsonAsync<int>("api/trucks/pagecount");
            await ShowPage();
        }

        async Task Delete(int Id)
        {
            var data = trucks.First(x => x.Id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["Delete?"]} {data.Vehicle_registration_number} - {data.Status} ({data.Id})"))
            {
                await client.DeleteAsync($"api/trucks/delete/{Id}");
                var shouldreload = dataRows % ((currentPage == 1 ? currentPage : currentPage - 1) * pageSize);
                if (shouldreload == 1 && dataRows > 0) { navigationManager.NavigateTo("/Trucks", true); }
                await OnInitializedAsync();
            }
        }

        async Task GetById(int? id, string? idType)
        {
            IdForGetById = id;
            getByIdType = idType;
            StateHasChanged();
        }

        public void SetToNull()
        {
            IdForGetById = null;
            getByIdType = null;
        }

        public void SettingsClosed()
        {
            settings = !settings;
        }

        async void onChangeGetFilter(ChangeEventArgs e)
        {
            switch (e.Value)
            {
                case "loaned":
                    filter = Status.loaned;
                    break;
                case "on_road":
                    filter = Status.on_road;
                    break;
                case "garage":
                    filter = Status.garage;
                    break;
                case "rented":
                    filter = Status.rented;
                    break;
                case "under_repair":
                    filter = Status.under_repair;
                    break;
                case "delivering":
                    filter = Status.delivering;
                    break;
                default:
                    filter = null;
                    break;
            }

            await OnInitializedAsync();
        }

        async void onChangeResetFilter()
        {
            filter = null;
            await OnInitializedAsync();
        }

        public void SettingsChanged() { }

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
            searchString = args.Value.ToString();
            await ShowPage();
        }

        protected async Task GetCurrentPage(int CurrentPage)
        {
            currentPage = CurrentPage;
            await ShowPage();
        }

        protected async Task ShowPage()
        {
            if (pageSize < 1 || pageSize == null) { pageSize = 10; }
            else if (pageSize >= dataRows) { pageSize = dataRows != 0 ? dataRows : 1; }
            maxPage = (int)Math.Ceiling((decimal)((float)dataRows / (float)pageSize));

            trucks = await client.GetFromJsonAsync<Cargotruck.Shared.Models.Trucks[]>($"api/trucks/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            StateHasChanged();
        }

        private async Task ExportToPdf()
        {
            //get base64 string from web api call
            var Response = await client.GetAsync($"api/trucks/pdf?lang={CultureInfo.CurrentCulture.Name.ToString()}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Trucks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf";

                //call javascript function to download the file
                await js.InvokeVoidAsync(
                    "downloadFile",
                    "application/pdf",
                    base64String,
                    filename);
            }
            else
            {
                document_error = localizer["Document_failder_to_create"];
            }
        }

        private async Task ExportToExcel()
        {
            //get base64 string from web api call
            var Response = await client.GetAsync($"api/trucks/excel?lang={CultureInfo.CurrentCulture.Name.ToString()}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Trucks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

                //call javascript function to download the file
                await js.InvokeVoidAsync(
                    "downloadFile",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    base64String,
                    filename);
            }
            else
            {
                document_error = localizer["Document_failder_to_create"];
            }
        }

        private async Task ExportToCSV(string format)
        {
            //get base64 string from web api call
            var Response = await client.GetAsync($"api/trucks/csv?lang={CultureInfo.CurrentCulture.Name.ToString()}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Trucks" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "." + format;
                //call javascript function to download the file
                await js.InvokeVoidAsync(
                    "downloadFile",
                    "text/" + format + ";charset=utf-8",
                    base64String,
                    filename);
            }
            else
            {
                document_error = localizer["Document_failder_to_create"];
            }
        }

        private async void StateChanged()
        {
            await OnInitializedAsync();
        }
    }
}
