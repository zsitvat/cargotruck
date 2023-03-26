﻿using Cargotruck.Client.Services;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Cargotruck.Client.Components
{
    public partial class GetByIdComponent
    {   
        Cargoes? idDataCargo;
        TasksDto? idDataTask;
        Expenses? idDataExpense;
        Roads? idDataRoad;
        Warehouses? idDataWarehouse;
        Trucks? idDataTruck;
        [Parameter] public string? GetById { get; set; }
        [Parameter] public string? GetByIdType { get; set; }
        [Parameter] public EventCallback OnSetToNull { get; set; }
        private string? _previousId;
        private string? _previousIdType;

        protected override async Task OnParametersSetAsync()
        {
            if (GetById != null && GetById != _previousId && GetByIdType != _previousIdType)
            {
                _previousId = GetById;
                _previousIdType = GetByIdType;
                if (GetByIdType == "cargo" || GetByIdType == "storage")
                {
                    idDataCargo = await client.GetFromJsonAsync<Cargoes?>($"api/cargoes/getbyid/{GetById}");
                }
                else if (GetByIdType == "task")
                {
                    idDataTask = await client.GetFromJsonAsync<TasksDto?>($"api/tasks/getbyid/{GetById}");
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
                    idDataTruck = await client.GetFromJsonAsync<Trucks?>($"api/trucks/getbyvrn/{GetById}");
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

        protected async Task SetToNullAsync()
        {
            await OnSetToNull.InvokeAsync();
        }

        public static void SettingsChanged() { }
    }
}