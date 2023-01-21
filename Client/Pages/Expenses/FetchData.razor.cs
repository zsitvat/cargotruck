using Cargotruck.Client.Services;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Expenses
{
    public partial class FetchData
    {
        public bool settings = false;
        Cargotruck.Shared.Models.Expenses[]? expenses { get; set; }
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
        string? currency_api_error;
        bool showError = false;
        string currency = "HUF";
        Dictionary<string, dynamic>? rates;
        Cargotruck.Shared.Models.Type? filter;
        DateFilter dateFilter = new();

        async void DateStartInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter.StartDate = DateTime.Parse(e.Value?.ToString());
                await OnInitializedAsync();
            }
        }

        async void DateEndInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter.EndDate = DateTime.Parse(e.Value?.ToString());
                await OnInitializedAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            PageHistoryState.AddPageToHistory("/Expenses");
            base.OnInitialized();
            if (rates == null)
            {
                try
                {
                    rates = await CurrencyExchange.GetRatesAsync(client);
                }
                catch (Exception ex)
                {

                    currency_api_error = $"Error - Type: {ex.GetType()}, Message: {ex.Message}";
                    if (ex.GetType().ToString() == "Microsoft.CSharp.RuntimeBinder.RuntimeBinderException")
                    {
                        currency_api_error = "currency_api_is_exceeded";
                    }
                }
            }
            dataRows = await client.GetFromJsonAsync<int>("api/expenses/pagecount");
            await ShowPage();
        }

        public async Task<float?> GetCurrencyAsync(int? amount)
        {
            float? conversionNum = amount;

            if (rates != null && currency != "HUF")
            {
                if (currency != "EUR")
                {
                    conversionNum = (float)((amount / rates["HUF"]) * rates[currency]);
                }
                else
                {
                    conversionNum = (float)(amount / rates["HUF"]);
                }
            }

            return conversionNum;
        }

        async void OnChangeGetType(ChangeEventArgs e)
        {
            currency = e.Value?.ToString();
        }

        async Task Delete(int Id)
        {
            var data = expenses.First(x => x.Id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["Delete?"]} {data.Type} - {data.Type_id} - {data.Date}"))
            {
                await client.DeleteAsync($"api/expenses/delete/{Id}");
                var shouldreload = dataRows % ((currentPage == 1 ? currentPage : currentPage - 1) * pageSize);
                if (shouldreload == 1 && dataRows > 0) { navigationManager.NavigateTo("/Expenses", true); }
                await OnInitializedAsync();
            }
        }

        async Task GetById(int? id, string idType)
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

        async void OnChangeGetFilter(ChangeEventArgs e)
        {
            switch (e.Value)
            {
                case "salary":
                    filter = Cargotruck.Shared.Models.Type.salary;
                    break;
                case "task":
                    filter = Cargotruck.Shared.Models.Type.task;
                    break;
                case "storage":
                    filter = Cargotruck.Shared.Models.Type.storage;
                    break;
                case "repair":
                    filter = Cargotruck.Shared.Models.Type.repair;
                    break;
                case "other":
                    filter = Cargotruck.Shared.Models.Type.other;
                    break;
                default:
                    filter = null;
                    break;
            }

            await OnInitializedAsync();
        }

        async void OnChangeResetFilter()
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
            searchString = args.Value?.ToString();
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

            expenses = await client.GetFromJsonAsync<Cargotruck.Shared.Models.Expenses[]>($"api/expenses/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&filter={filter}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            StateHasChanged();
        }

        private async Task ExportToPdf()
        {
            //get base64 string from web api call
            var Response = await client.GetAsync($"api/expenses/pdf?lang={CultureInfo.CurrentCulture.Name.ToString()}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf";

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
            var Response = await client.GetAsync($"api/expenses/excel?lang={CultureInfo.CurrentCulture.Name.ToString()}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

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
            var Response = await client.GetAsync($"api/expenses/csv?lang={CultureInfo.CurrentCulture.Name.ToString()}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "." + format;
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
