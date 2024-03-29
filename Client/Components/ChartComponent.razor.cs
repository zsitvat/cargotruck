﻿using System.Net.Http.Json;
using System.Globalization;
using Cargotruck.Shared.Model;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.Util;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.LineChart;
using Cargotruck.Client.Services;

namespace Cargotruck.Client.Components
{
    public partial class ChartComponent
    {
        readonly string lang = CultureInfo.CurrentCulture.Name;
        private BarConfig? _config1;
        private BarConfig? _config2;
        private LineConfig? _config3;
        private LineConfig? _config4;
        protected override async Task OnInitializedAsync()
        {
            await CreateChartsAsync();
        }

        protected async Task CreateChartsAsync()
        {
            await TasksChartAsync();
            await CargoesChartAsync();
            await TrucksChartAsync();
            await ExpensesChartAsync();
        }

        protected async Task TasksChartAsync()
        {
            //Tasks
            _config1 = new BarConfig
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    MaintainAspectRatio = false,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = (lang == "hu" ? "Megbízások" : "Tasks") + " " + DateTime.Now.Year,
                        FontSize = 16
                    },
                    Tooltips = new Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false,
                        TitleFontColor = "#bab3b3",
                        BodyFontColor = "#bab3b3"
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis>
                        {
                            new BarCategoryAxis
                            {
                                Stacked = true
                            }
                        },
                        YAxes = new List<CartesianAxis>
                        {
                            new BarLinearCartesianAxis
                            {
                                Stacked = true
                            }
                        }
                    }
                }
            };
            string[] columns = DateTimeFormatInfo.CurrentInfo.MonthNames;
            foreach (string column in columns.Where(x => x != null && x != ""))
            {
                string str = column;
                str = char.ToUpper(str[0]) + str[1..];
                _config1.Data.Labels.Add(str);
            }

            var columnHeights = await client.GetFromJsonAsync<int[]>("api/tasks/getchartdata");
            if (columnHeights != null)
            {
                int[] datalistAllItems = columnHeights.Take(12).ToArray();
                BarDataset<int> datasetNotCompleted = new(datalistAllItems)
                {
                    Label = lang == "hu" ? "Nem teljesített" : "Not completed",
                    BackgroundColor = new[]
                    {
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235)
                    }
                };
                int[] datalistPartOfItems = columnHeights.Skip(12).Take(12).ToArray();
                BarDataset<int> datasetCompleted = new(datalistPartOfItems)
                {
                    Label = lang == "hu" ? "Teljesített" : "Completed",
                    BackgroundColor = new[]
                    {
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86)
                    }
                };
                _config1.Data.Datasets.Add(datasetCompleted);
                _config1.Data.Datasets.Add(datasetNotCompleted);
            }
        }

        protected async Task CargoesChartAsync()
        {
            //Tasks
            _config2 = new BarConfig
            {
                Options = new BarOptions
                {
                    MaintainAspectRatio = false,
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = (lang == "hu" ? "Szállítmányok" : "Cargoes") + " " + DateTime.Now.Year,
                        FontSize = 16
                    },
                    Tooltips = new Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false,
                        TitleFontColor = "#bab3b3",
                        BodyFontColor = "#bab3b3"
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis>
                        {
                            new BarCategoryAxis
                            {
                                Stacked = true
                            }
                        },
                        YAxes = new List<CartesianAxis>
                        {
                            new BarLinearCartesianAxis
                            {
                                Stacked = true
                            }
                        }
                    }
                }
            };
            string[] columns = DateTimeFormatInfo.CurrentInfo.MonthNames;
            foreach (string column in columns.Where(x => x != null && x != ""))
            {
                string str = column;
                str = char.ToUpper(str[0]) + str[1..];
                _config2.Data.Labels.Add(str);
            }

            var columnHeights = await client.GetFromJsonAsync<int[]>("api/cargoes/getchartdata");
            if (columnHeights != null)
            {
                int[] datalistFirstPartOfItems = columnHeights.Take(12).ToArray();
                BarDataset<int> datasetNotDelivered = new(datalistFirstPartOfItems)
                {
                    Label = lang == "hu" ? "Nincs elszállítva" : "Not delivered",
                    BackgroundColor = new[]
                    {
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235),
                        ColorUtil.ColorHexString(54, 162, 235)
                    }
                };
                int[] datalistSecondPartOfItems = columnHeights.Skip(12).Take(12).ToArray();
                BarDataset<int> datasetDelivered = new(datalistSecondPartOfItems)
                {
                    Label = lang == "hu" ? "Elszállítva" : "Delivered",
                    BackgroundColor = new[]
                    {
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86),
                        ColorUtil.ColorHexString(255, 205, 86)
                    }
                };
                int[] datalistThirdPartOfItems = columnHeights.Skip(24).Take(12).ToArray();
                BarDataset<int> datasetInWarehouse = new(datalistThirdPartOfItems)
                {
                    Label = lang == "hu" ? "Raktárban" : "In warehouse",
                    BackgroundColor = new[]
                    {
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174),
                        ColorUtil.ColorHexString(130, 110, 174)
                    }
                };
                _config2.Data.Datasets.Add(datasetDelivered);
                _config2.Data.Datasets.Add(datasetInWarehouse);
                _config2.Data.Datasets.Add(datasetNotDelivered);
            }
        }

        protected async Task TrucksChartAsync()
        {
            //Tasks
            _config3 = new LineConfig
            {
                Options = new LineOptions
                {
                    MaintainAspectRatio = false,
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = (lang == "hu" ? "Járművek használata" : "Uses of trucks") + " " + DateTime.Now.Year,
                        FontSize = 16
                    },
                    Tooltips = new Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false,
                        TitleFontColor = "#bab3b3",
                        BodyFontColor = "#bab3b3"
                    },
                    Hover = new Hover
                    {
                        Mode = InteractionMode.Index,
                        Intersect = true
                    },
                    Scales = new Scales
                    {
                        XAxes = new List<CartesianAxis>
                        {
                            new CategoryAxis
                            {
                                ScaleLabel = new ScaleLabel
                                {
                                    LabelString = "Month"
                                }
                            }
                        },
                        YAxes = new List<CartesianAxis>
                        {
                            new LinearCartesianAxis
                            {
                                ScaleLabel = new ScaleLabel
                                {
                                    LabelString = "Value"
                                }
                            }
                        }
                    }
                }
            };
            string[] columns = DateTimeFormatInfo.CurrentInfo.MonthNames;
            foreach (string column in columns.Where(x => x != null && x != ""))
            {
                string str = column;
                str = char.ToUpper(str[0]) + str[1..];
                _config3.Data.Labels.Add(str);
            }

            var columnHeights = await client.GetFromJsonAsync<int[]>("api/roads/getchartdata");
            if (columnHeights != null)
            {
                var roads = await client.GetFromJsonAsync<Road[]?>("api/roads/getroads");
                var trucksVRN = roads?.DistinctBy(x => x.VehicleRegistrationNumber).ToList();
                if (roads != null && trucksVRN != null && roads.Length > 0 && trucksVRN.Count > 0)
                {
                    int NumberOfTrucks = columnHeights.Length / 12;
                    for (int i = 0; i < NumberOfTrucks; i++)
                    {
                        int[] datalistItems = columnHeights.Skip(12 * i).Take(12).ToArray();
                        List<byte> colorChosen = new();
                        switch (i)
                        {
                            case 0:
                                colorChosen.AddRange(new[] { (byte)130, (byte)110, (byte)174 });
                                break;
                            case 1:
                                colorChosen.AddRange(new[] { (byte)54, (byte)162, (byte)235 });
                                break;
                            case 2:
                                colorChosen.AddRange(new[] { (byte)255, (byte)205, (byte)86 });
                                break;
                            case 3:
                                colorChosen.AddRange(new[] { (byte)252, (byte)3, (byte)90 });
                                break;
                            case 4:
                                colorChosen.AddRange(new[] { (byte)25, (byte)135, (byte)84 });
                                break;
                            default:
                                //color picker
                                Random rnd = new();
                                colorChosen.AddRange(new[] { (byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255) });
                                break;
                        //etc
                        }

                        LineDataset<int> datasetNumberOfRoads = new(datalistItems)
                        {
                            Label = lang == "hu" ? "Utak száma (Rendszám: " + trucksVRN?[i].VehicleRegistrationNumber + ")" : "Number of roads (VRN: " + trucksVRN?[i].VehicleRegistrationNumber + ")",
                            BorderColor = ColorUtil.ColorHexString(colorChosen[0], colorChosen[1], colorChosen[2]),
                            Fill = false,
                            LineTension = 0
                        };
                        _config3.Data.Datasets.Add(datasetNumberOfRoads);
                    }
                }
            }
        }

        protected async Task ExpensesChartAsync()
        {
            //Tasks
            _config4 = new LineConfig
            {
                Options = new LineOptions
                {
                    MaintainAspectRatio = false,
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = (lang == "hu" ? "Bevétel" : "Income") + " " + DateTime.Now.Year,
                        FontSize = 16
                    },
                    Tooltips = new Tooltips
                    {
                        Mode = InteractionMode.Index,
                        Intersect = false,
                        TitleFontColor = "#bab3b3",
                        BodyFontColor = "#bab3b3"
                    },
                    Hover = new Hover
                    {
                        Mode = InteractionMode.Index,
                        Intersect = true
                    },
                    Scales = new Scales
                    {
                        XAxes = new List<CartesianAxis>
                        {
                            new CategoryAxis
                            {
                                ScaleLabel = new ScaleLabel
                                {
                                    LabelString = "Month"
                                }
                            }
                        },
                        YAxes = new List<CartesianAxis>
                        {
                            new LinearCartesianAxis
                            {
                                ScaleLabel = new ScaleLabel
                                {
                                    LabelString = "Value"
                                }
                            }
                        }
                    }
                }
            };
            string[] columns = DateTimeFormatInfo.CurrentInfo.MonthNames;
            foreach (string column in columns.Where(x => x != null && x != ""))
            {
                string str = column;
                str = char.ToUpper(str[0]) + str[1..];
                _config4.Data.Labels.Add(str);
            }

            await GetExpenseChartData();
        }

        private async Task GetExpenseChartData()
        {
            var columnHeights = await client.GetFromJsonAsync<float?[]>("api/monthlyexpenses/getchartdata");

            for (int i = 0; i < columnHeights?.Length; i++)
            {     
                columnHeights[i] = currencyExchange.GetCurrencyAmount((int)columnHeights[i]!, currencyExchange.GetCurrencyType());
            }

            if (columnHeights != null)
            {
                float?[] datalistFirstPartItems = columnHeights.Take(12).ToArray();
                LineDataset<float?> datasetIncomes = new(datalistFirstPartItems)
                {
                    Label = (lang == "hu" ? "Nyereség" : "Profit"),
                    BorderColor = ColorUtil.ColorHexString(255, 205, 86),
                    Fill = false,
                    LineTension = 0
                };
                float?[] datalistSecondPartOfItems = columnHeights.Skip(12).Take(12).ToArray();
                LineDataset<float?> datasetExpenses = new(datalistSecondPartOfItems)
                {
                    Label = (lang == "hu" ? "Kiadás" : "Expenses"),
                    BorderColor = ColorUtil.ColorHexString(130, 110, 174),
                    Fill = false,
                    LineTension = 0
                };
                float?[] datalistThirdPartOfItems = columnHeights.Skip(24).Take(12).ToArray();
                LineDataset<float?> datasetEarning = new(datalistThirdPartOfItems)
                {
                    Label = (lang == "hu" ? "Jövedelem" : "Earning"),
                    BorderColor = ColorUtil.ColorHexString(54, 162, 235),
                    Fill = false,
                    LineTension = 0
                };
                _config4?.Data.Datasets.Add(datasetIncomes);
                _config4?.Data.Datasets.Add(datasetExpenses);
                _config4?.Data.Datasets.Add(datasetEarning);
            }
        }

        public async void CurrencyChanged()
        {
            _config4?.Data.Datasets.Clear();
            await GetExpenseChartData();
            StateHasChanged();
        }
    }
}