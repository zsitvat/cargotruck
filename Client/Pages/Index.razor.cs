using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using Cargotruck.Client;
using Cargotruck.Client.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Cargotruck.Client.UtilitiesClasses;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using RestSharp;
using Cargotruck.Shared.Model.Dto;
using Cargotruck.Client.Components;
using Microsoft.AspNetCore.Identity;
using ChartJs.Blazor;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.Common.Handlers;
using ChartJs.Blazor.Common.Time;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.Util;
using ChartJs.Blazor.Interop;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.LineChart;
using Cargotruck.Shared.Resources;
using Microsoft.Extensions.Localization;
using Cargotruck.Shared.Model;

namespace Cargotruck.Client.Pages
{
    public partial class Index
    {
        [CascadingParameter]
        Task<AuthenticationState>? AuthenticationState { get; set; }

        bool greetings = true;
        Dictionary<string, int?> counts = new Dictionary<string, int?>()
        {
            {
                "numberOfTasksUnFinished",
                null
            },
            {
                "numberOfTrucksUnFinished",
                null
            },
            {
                "numberOfCargoesUnFinished",
                null
            },
            {
                "numberOfLogins",
                null
            },
            {
                "numberOfTasks",
                null
            },
            {
                "numberOfTrucks",
                null
            },
            {
                "numberOfCargoes",
                null
            },
            {
                "numberOfUsers",
                null
            }
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
            Dictionary<string, int?> numbersDic = new Dictionary<string, int?>()
            {
                {
                    "numberOfTasksUnFinished",
                    0
                },
                {
                    "numberOfTrucksUnFinished",
                    0
                },
                {
                    "numberOfCargoesUnFinished",
                    0
                },
                {
                    "numberOfLogins",
                    0
                },
                {
                    "numberOfTasks",
                    0
                },
                {
                    "numberOfTrucks",
                    0
                },
                {
                    "numberOfCargoes",
                    0
                },
                {
                    "numberOfUsers",
                    0
                }
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
            int max = 0;
            foreach (var num in MaxNumbers)
            {
                if (max < num.Value)
                {
                    max = (int)num.Value;
                }
            }

            for (int i = 0; i < max; i++)
            {
                if (i <= MaxNumbers["numberOfTasks"])
                {
                    counts["numberOfTasks"] = i;
                }

                if (i <= MaxNumbers["numberOfTrucks"])
                {
                    counts["numberOfTrucks"] = i;
                }

                if (i <= MaxNumbers["numberOfCargoes"])
                {
                    counts["numberOfCargoes"] = i;
                }

                if (i <= MaxNumbers["numberOfTasksUnFinished"])
                {
                    counts["numberOfTasksUnFinished"] = i;
                }

                if (i <= MaxNumbers["numberOfTrucksUnFinished"])
                {
                    counts["numberOfTrucksUnFinished"] = i;
                }

                if (i <= MaxNumbers["numberOfCargoesUnFinished"])
                {
                    counts["numberOfCargoesUnFinished"] = i;
                }

                if (i <= MaxNumbers["numberOfLogins"])
                {
                    counts["numberOfLogins"] = i;
                }

                if (i <= MaxNumbers["numberOfUsers"])
                {
                    counts["numberOfUsers"] = i;
                }

                await Task.Delay(25);
                StateHasChanged();
            }
        }
    }
}