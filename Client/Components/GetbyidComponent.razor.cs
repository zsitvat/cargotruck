using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cargotruck.Client.Components
{
    public partial class GetByIdComponent
    {
        string currency = "HUF";
        CargoesDto? idDataCargo;
        TasksDto? idDataTask;
        ExpensesDto? idDataExpense;
        RoadsDto? idDataRoad;
        WarehousesDto? idDataWarehouse;
        TrucksDto? idDataTruck;
        [Parameter] public string? GetById { get; set; }
        [Parameter] public string? GetByIdType { get; set; }
        [Parameter] public EventCallback OnSetToNull { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (GetById != null)
            {
                if (GetByIdType == "cargo" || GetByIdType == "storage")
                {
                    idDataCargo = await client.GetFromJsonAsync<CargoesDto?>($"api/cargoes/getbyid/{GetById}");
                }
                else if (GetByIdType == "task")
                {
                    idDataTask = await client.GetFromJsonAsync<TasksDto?>($"api/tasks/getbyid/{GetById}");
                }
                else if (GetByIdType == "expense")
                {
                    idDataExpense = await client.GetFromJsonAsync<ExpensesDto?>($"api/expenses/getbyid/{GetById}");
                }
                else if (GetByIdType == "road" || GetByIdType == "repair")
                {
                    idDataRoad = await client.GetFromJsonAsync<RoadsDto?>($"api/roads/getbyid/{GetById}");
                }
                else if (GetByIdType == "warehouse")
                {
                    idDataWarehouse = await client.GetFromJsonAsync<WarehousesDto?>($"api/warehouses/getbyid/{GetById}");
                }
                else if (GetByIdType == "truck")
                {
                    idDataTruck = await client.GetFromJsonAsync<TrucksDto?>($"api/trucks/getbyvrn/{GetById}");
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