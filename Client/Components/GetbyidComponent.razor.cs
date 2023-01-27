using Cargotruck.Client.Services;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace Cargotruck.Client.Components
{
    public partial class GetbyidComponent
    {
        string currency = "HUF";
        Cargoes? idDataCargo;
        Tasks? idDataTask;
        Expenses? idDataExpense;
        Roads? idDataRoad;
        Warehouses? idDataWarehouse;
        Trucks? idDataTruck;
        [Parameter] public string? GetById { get; set; }
        [Parameter] public string? GetByIdType { get; set; }
        [Parameter] public EventCallback OnSetToNull { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (GetById != null)
            {
                if (GetByIdType == "cargo" || GetByIdType == "storage")
                {
                    idDataCargo = await client.GetFromJsonAsync<Cargoes?>($"api/cargoes/getbyid/{GetById}");
                }
                else if (GetByIdType == "task")
                {
                    idDataTask = await client.GetFromJsonAsync<Tasks?>($"api/tasks/getbyid/{GetById}");
                }
                else if (GetByIdType == "expense")
                {
                    idDataExpense = await client.GetFromJsonAsync<Expenses?>($"api/expenses/getbyid/{GetById}");
                }
                else if (GetByIdType == "road" || GetByIdType == "repair")
                {
                    idDataRoad = await client.GetFromJsonAsync<Roads?>($"api/roads/getbyid/{GetById}");
                }
                else if (GetByIdType == "warehouse")
                {
                    idDataWarehouse = await client.GetFromJsonAsync<Warehouses?>($"api/warehouses/getbyid/{GetById}");
                }
                else if (GetByIdType == "truck")
                {
                    idDataTruck = await client.GetFromJsonAsync<Trucks?>($"api/trucks/getbyid/{GetById}");
                }
                else
                {
                    idDataCargo = null;
                    idDataTask = null;
                    idDataExpense = null;
                    idDataRoad = null;
                    idDataWarehouse = null;
                    idDataTruck = null;
                    GetById = null;
                }
            }
        }

        protected async Task SetToNull()
        {
            await OnSetToNull.InvokeAsync();
        }

        void OnChangeGetType(ChangeEventArgs e)
        {
            currency = e.Value?.ToString()!;
        }
    }
}