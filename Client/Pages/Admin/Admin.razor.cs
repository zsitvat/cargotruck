using Cargotruck.Server.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Admin
{
    public partial class Admin
    {

        public bool settings = false;
        List<bool> showColumns = Enumerable.Repeat(true, 6).ToList();
        Users[]? users { get; set; }
        Dictionary<string, string>? claims { get; set; }
        Dictionary<string, string>? roles { get; set; }
        string? filter = "";
        private int currentPage = 1;
        int pageSize = 10;
        int dataRows;
        float maxPage;

        protected override async Task OnInitializedAsync()
        {
            await ShowPage();
        }

        protected async Task ShowPage()
        {
            dataRows = await client.GetFromJsonAsync<int>($"api/admin/pagecount?filter={filter}");
            if (pageSize < 1) { pageSize = 10; }
            else if (pageSize >= dataRows) { pageSize = dataRows != 0 ? dataRows : 1; }
            maxPage = (int)Math.Ceiling((decimal)((float)dataRows / (float)pageSize));
            claims = await client.GetFromJsonAsync<Dictionary<string, string>?>("api/admin/claims");
            roles = await client.GetFromJsonAsync<Dictionary<string, string>?>("api/admin/roles");
            users = await client.GetFromJsonAsync<Users[]>($"api/admin/get?page={currentPage}&pageSize={pageSize}&filter={filter}");
            StateHasChanged();
        }

        async Task Delete(string Id)
        {
            var u = users?.First(x => x.Id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["Delete?"]} {u?.UserName} ({u?.Id})"))
            {
                await client.DeleteAsync($"api/admin/delete/{Id}");
                await OnInitializedAsync();
            }
        }

        async void onChangeGetFilter(ChangeEventArgs e)
        {
            filter = e.Value?.ToString();
            await OnInitializedAsync();
        }

        async void onChangeResetFilter()
        {
            filter = "";
            await OnInitializedAsync();
        }

        protected async Task GetCurrentPage(int CurrentPage)
        {
            currentPage = CurrentPage;
            await OnInitializedAsync();
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
    }
}
