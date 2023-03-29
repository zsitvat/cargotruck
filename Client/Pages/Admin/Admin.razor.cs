using Cargotruck.Shared.Model.Dto;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Cargotruck.Client.Pages.Admin
{
    public partial class Admin
    {

        public bool settings = false;
        readonly List<bool> showColumns = Enumerable.Repeat(true, 6).ToList();
        UserDto[]? Users { get; set; }
        Dictionary<string, string>? Claims { get; set; }
        Dictionary<string, string>? Roles { get; set; }
        string? filter = "";
        private int currentPage = 1;
        int pageSize = 10;
        int dataRows;
        float maxPage;

        protected override async Task OnInitializedAsync()
        {
            await ShowPageAsync();
        }

        protected async Task ShowPageAsync()
        {
            dataRows = await client.GetFromJsonAsync<int>("api/admin/pagecount");
            if (pageSize < 1) { pageSize = 10; }
            else if (pageSize >= dataRows) { pageSize = dataRows != 0 ? dataRows : 1; }
            maxPage = (int)Math.Ceiling((decimal)((float)dataRows / (float)pageSize));
            Claims = await client.GetFromJsonAsync<Dictionary<string, string>?>("api/admin/claims");
            Roles = await client.GetFromJsonAsync<Dictionary<string, string>?>("api/admin/roles");
            Users = await client.GetFromJsonAsync<UserDto[]>($"api/admin/get?page={currentPage}&pageSize={pageSize}&filter={filter}");
            StateHasChanged();
        }

        async Task DeleteAsync(string Id)
        {
            var u = Users?.First(x => x.Id == Id);
            if (await js.InvokeAsync<bool>("confirm", $"{@localizer["DeleteAsync?"]} {u?.UserName} ({u?.Id})"))
            {
                await client.DeleteAsync($"api/admin/delete/{Id}");
                await OnInitializedAsync();
            }
        }

        async void OnChangeResetFilter()
        {
            filter = "";
            await OnInitializedAsync();
        }

        protected async Task GetCurrentPageAsync(int CurrentPage)
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
            await ShowPageAsync();
        }
        private async void StateChanged()
        {
            pageSize = 10;
            await OnInitializedAsync();
        }
    }
}
