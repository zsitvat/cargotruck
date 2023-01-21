using Cargotruck.Client.Services;
using Cargotruck.Shared.Models;
using Cargotruck.Shared.Models.Request;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Monthly_expenses
{
    public partial class FetchData
    {
        public bool settings = false;
        Cargotruck.Shared.Models.Monthly_expenses[]? Monthly_expenses { get; set; }
        Monthly_expenses_tasks_expenses[]? connection_ids { get; set; }
        int? IdForGetById { get; set; }
        string? getByIdType { get; set; }
        List<bool> showColumns = Enumerable.Repeat(true, 6).ToList();
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
       [CascadingParameter]  Dictionary<string, dynamic>? rates {get;set;}
        DateFilter? dateFilter = new();

        async void DateStartInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter!.StartDate = DateTime.Parse(e?.Value?.ToString()!);
                await OnInitializedAsync();
            }
        }

        async void DateEndInput(ChangeEventArgs e)
        {
            if (e != null && e.Value?.ToString() != "")
            {
                dateFilter!.EndDate = DateTime.Parse(e?.Value?.ToString()!);
                await OnInitializedAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            PageHistoryState.AddPageToHistory("/Monthly_expenses");
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
            await client.GetStringAsync("api/Monthly_expenses/createcontable");
            dataRows = await client.GetFromJsonAsync<int>("api/Monthly_expenses/pagecount");
            var checkData = await client.GetAsync("api/Monthly_expenses/checkdata");
            connection_ids = await client.GetFromJsonAsync<Monthly_expenses_tasks_expenses[]?>
            ("api/Monthly_expenses/getconnectionids");
            if (checkData.IsSuccessStatusCode)
            {
                await ShowPage();
                document_error = "";
            }
            else
            {
                document_error = localizer["CheckFailed"];
            }
        }

        public float? GetCurrency(int? amount)
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

        void OnChangeGetType(ChangeEventArgs e)
        {
            if (e.Value != null)
            {
                currency = e.Value.ToString()!;
            }
        }

        async Task Delete(int Id)
        {
            var data = Monthly_expenses?.First(x => x.Monthly_expense_id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["Delete?"]} {data?.Earning} - {data?.Profit} ({data?.Monthly_expense_id})"))
            {
                await client.DeleteAsync($"api/Monthly_expenses/delete/{Id}");
                var shouldreload = dataRows % ((currentPage == 1 ? currentPage : currentPage - 1) * pageSize);
                if (shouldreload == 1 && dataRows > 0) { navigationManager.NavigateTo("/Monthly_expenses", true); }
                await OnInitializedAsync();
            }
        }

        void GetById(int? id, string idType)
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

        protected async Task GetCurrentPage(int CurrentPage)
        {
            currentPage = CurrentPage;
            await ShowPage();
        }

        protected async Task ShowPage()
        {
            if (pageSize < 1) { pageSize = 10; }
            else if (pageSize >= dataRows) { pageSize = dataRows != 0 ? dataRows : 1; }
            maxPage = (int)Math.Ceiling((decimal)((float)dataRows / (float)pageSize));
            Monthly_expenses = await client.GetFromJsonAsync<Cargotruck.Shared.Models.Monthly_expenses[]>($"api/Monthly_expenses/get?page={currentPage}&pageSize={pageSize}&sortOrder={sortOrder}&desc={desc}&searchString={searchString}&lang={CultureInfo.CurrentCulture.Name.ToString()}&dateFilterStartDate={dateFilter?.StartDate}&dateFilterEndDate={dateFilter?.EndDate}");
            StateHasChanged();
        }

        public void SettingsClosed()
        {
            settings = !settings;
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

        private async Task ExportToPdf()
        {
            //get base64 string from web api call
            var Response = await client.GetAsync($"api/Monthly_expenses/pdf?lang={CultureInfo.CurrentCulture.Name.ToString()}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Monthly_expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf";

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
            var Response = await client.GetAsync($"api/Monthly_expenses/excel?lang={CultureInfo.CurrentCulture.Name.ToString()}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Monthly_expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

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
            var Response = await client.GetAsync($"api/Monthly_expenses/csv?lang={CultureInfo.CurrentCulture.Name.ToString()}");

            if (Response.IsSuccessStatusCode)
            {
                var base64String = await Response.Content.ReadAsStringAsync();

                Random rnd = new();
                int random = rnd.Next(1000000, 9999999);
                string filename = "Monthly_expenses" + random + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "." + format;
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
    }
}
