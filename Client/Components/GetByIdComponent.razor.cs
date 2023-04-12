using Cargotruck.Client.Services;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cargotruck.Client.Components
{
    public partial class GetByIdComponent
    {   
        Cargo? idDataCargo;
        DeliveryTaskDto? idDataTask;
        Expense? idDataExpense;
        Road? idDataRoad;
        Warehouse? idDataWarehouse;
        Truck? idDataTruck;
        [Parameter] public string? GetById { get; set; }
        [Parameter] public string? GetByIdType { get; set; }
        [Parameter] public EventCallback OnSetToNull { get; set; }
        private string? _previousId;
        private string? _previousIdType;

        protected override async Task OnParametersSetAsync()
        {
            await GetDataByIdAndType();
        }

        protected async Task SetToNullAsync()
        {
            await OnSetToNull.InvokeAsync();
        }

        protected async Task GetDataByIdAndType() 
        {
            if (GetById != null && ( GetById != _previousId || GetByIdType != _previousIdType))
            {
                _previousId = GetById;
                _previousIdType = GetByIdType;

                // Retrieve data from web API based on GetByIdType value
                if (GetByIdType == "cargo" || GetByIdType == "storage")
                {
                    idDataCargo = await client.GetFromJsonAsync<Cargo?>($"api/cargoes/getbyid/{GetById}");
                }
                else if (GetByIdType == "task")
                {
                    idDataTask = await client.GetFromJsonAsync<DeliveryTaskDto?>($"api/tasks/getbyid/{GetById}");
                }
                else if (GetByIdType == "expense")
                {
                    idDataExpense = await client.GetFromJsonAsync<Expense?>($"api/expenses/getbyid/{GetById}");
                }
                else if (GetByIdType == "road" || GetByIdType == "repair")
                {
                    idDataRoad = await client.GetFromJsonAsync<Road?>($"api/roads/getbyid/{GetById}");
                }
                else if (GetByIdType == "warehouse")
                {
                    idDataWarehouse = await client.GetFromJsonAsync<Warehouse?>($"api/warehouses/getbyid/{GetById}");
                }
                else if (GetByIdType == "truck")
                {
                    idDataTruck = await client.GetFromJsonAsync<Truck?>($"api/trucks/getbyvrn/{GetById}");
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

        public static void SettingsChanged() { }
    }
}