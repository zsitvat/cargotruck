using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Cargotruck.Client.Services;

namespace Cargotruck.Client.Pages
{
    public partial class Index
    {
        [CascadingParameter]
        Task<AuthenticationState>? AuthenticationState { get; set; }

        bool greetings = true;
        readonly Dictionary<string, int?> counts = new()
        {
            { "numberOfTasksUnFinished", null },
            { "numberOfTrucksUnFinished", null },
            { "numberOfCargoesUnFinished", null },
            { "numberOfLogins", null },
            { "numberOfTasks", null },
            { "numberOfTrucks", null },
            { "numberOfCargoes", null },
            { "numberOfUsers", null }
        };

        protected override async Task OnInitializedAsync()
        {
            if ((await AuthenticationState!).User.Identity!.IsAuthenticated)
            {
                if (!PageHistoryState.GetPageIsVisited("/"))
                {
                    PageHistoryState.AddPageToHistory("/");
                }
                else
                {
                    greetings = false;
                }

                await GetNumbersAsync();
            }
            else
            {
                PageHistoryState.ResetPageToHistory();
            }
        }

        protected async Task GetNumbersAsync()
        {
            Dictionary<string, int?> numbersDic = new()
            {
                { "numberOfTasksUnFinished", 0 },
                { "numberOfTrucksUnFinished", 0 },
                { "numberOfCargoesUnFinished", 0 },
                { "numberOfLogins", 0 },
                { "numberOfTasks", 0 },
                { "numberOfTrucks", 0 },
                { "numberOfCargoes", 0 },
                { "numberOfUsers", 0 }
            };

            numbersDic["numberOfTasks"] = await client.GetFromJsonAsync<int>($"api/tasks/count?all={true}");
            numbersDic["numberOfTrucks"] = await client.GetFromJsonAsync<int>($"api/trucks/count?all={true}");
            numbersDic["numberOfCargoes"] = await client.GetFromJsonAsync<int>($"api/cargoes/count?all={true}");
            numbersDic["numberOfTasksUnFinished"] = await client.GetFromJsonAsync<int>($"api/tasks/count?all={false}");
            numbersDic["numberOfTrucksUnFinished"] = await client.GetFromJsonAsync<int>($"api/trucks/count?all={false}");
            numbersDic["numberOfCargoesUnFinished"] = await client.GetFromJsonAsync<int>($"api/cargoes/count?all={false}");
            numbersDic["numberOfLogins"] = await client.GetFromJsonAsync<int>("api/admin/loginscount");
            numbersDic["numberOfUsers"] = await client.GetFromJsonAsync<int>("api/admin/count");
            await GetNumbersCounterAsync(numbersDic);
        }

        protected async Task GetNumbersCounterAsync(Dictionary<string, int?> MaxNumbers)
        {
            int max = MaxNumbers.Values.Max() ?? 0;

            for (int i = 0; i <= max; i++) 
            {
                foreach (var key in MaxNumbers) 
                {
                    if (i <= MaxNumbers[key.Key])
                    {
                        counts[key.Key] = i;
                    }
                }

                await Task.Delay(30);
                StateHasChanged();
            }
        }
    }
}