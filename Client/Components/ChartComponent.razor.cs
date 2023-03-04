using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Cargotruck.Client;
using Cargotruck.Client.Shared;
using Cargotruck.Shared.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Authorization;
using Cargotruck.Shared.Models;
using Cargotruck.Client.Services;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using RestSharp;
using Cargotruck.Shared.Models.Request;
using Cargotruck.Client.Components;
using Cargotruck.Server.Models;
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

namespace Cargotruck.Client.Components
{
    public partial class ChartComponent
    {
        string lang = CultureInfo.CurrentCulture.Name;
        private BarConfig? _config1;
        private BarConfig? _config2;
        private LineConfig? _config3;
        private LineConfig? _config4;
        protected override async Task OnInitializedAsync()
        {
            await Initialize();
        }

        protected async Task Initialize()
        {
            await TasksChart();
            await CargoesChart();
            await TrucksChart();
            await ExpensesChart();
        }

        protected async Task TasksChart()
        {
            //Tasks
            _config1 = new BarConfig
            {
                Options = new BarOptions
                {
                    Responsive = true,
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
                str = char.ToUpper(str[0]) + str.Substring(1);
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

        protected async Task CargoesChart()
        {
            //Tasks
            _config2 = new BarConfig
            {
                Options = new BarOptions
                {
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
                str = char.ToUpper(str[0]) + str.Substring(1);
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

        protected async Task TrucksChart()
        {
            //Tasks
            _config3 = new LineConfig
            {
                Options = new LineOptions
                {
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
                str = char.ToUpper(str[0]) + str.Substring(1);
                _config3.Data.Labels.Add(str);
            }

            var columnHeights = await client.GetFromJsonAsync<int[]>("api/roads/getchartdata");
            if (columnHeights != null)
            {
                var roads = await client.GetFromJsonAsync<Roads[]?>("api/roads/getroads");
                var trucksVRN = roads?.DistinctBy(x => x.Vehicle_registration_number).ToList();
                if (roads != null && trucksVRN != null && roads.Count() > 0 && trucksVRN.Count > 0)
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
                                Random rnd = new Random();
                                colorChosen.AddRange(new[] { (byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255) });
                                break;
                        //etc
                        }

                        LineDataset<int> datasetNumberOfRoads = new(datalistItems)
                        {
                            Label = lang == "hu" ? "Utak száma (Rendszám: " + trucksVRN?[i].Vehicle_registration_number + ")" : "Number of roads (VRN: " + trucksVRN?[i].Vehicle_registration_number + ")",
                            BorderColor = ColorUtil.ColorHexString(colorChosen[0], colorChosen[1], colorChosen[2]),
                            Fill = false,
                            LineTension = 0
                        };
                        _config3.Data.Datasets.Add(datasetNumberOfRoads);
                    }
                }
            }
        }

        protected async Task ExpensesChart()
        {
            //Tasks
            _config4 = new LineConfig
            {
                Options = new LineOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = (lang == "hu" ? "Bevétel" : "Income") + " " + DateTime.Now.Year + " (HUF)",
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
                str = char.ToUpper(str[0]) + str.Substring(1);
                _config4.Data.Labels.Add(str);
            }

            var columnHeights = await client.GetFromJsonAsync<int[]>("api/monthly_expenses/getchartdata");
            if (columnHeights != null)
            {
                int[] datalistFirstPartItems = columnHeights.Take(12).ToArray();
                LineDataset<int> datasetIncomes = new(datalistFirstPartItems)
                {
                    Label = (lang == "hu" ? "Nyereség" : "Profit"),
                    BorderColor = ColorUtil.ColorHexString(255, 205, 86),
                    Fill = false,
                    LineTension = 0
                };
                int[] datalistSecondPartOfItems = columnHeights.Skip(12).Take(12).ToArray();
                LineDataset<int> datasetExpenses = new(datalistSecondPartOfItems)
                {
                    Label = (lang == "hu" ? "Kiadás" : "Expenses"),
                    BorderColor = ColorUtil.ColorHexString(130, 110, 174),
                    Fill = false,
                    LineTension = 0
                };
                int[] datalistThirdPartOfItems = columnHeights.Skip(24).Take(12).ToArray();
                LineDataset<int> datasetEarning = new(datalistThirdPartOfItems)
                {
                    Label = (lang == "hu" ? "Jövedelem" : "Earning"),
                    BorderColor = ColorUtil.ColorHexString(54, 162, 235),
                    Fill = false,
                    LineTension = 0
                };
                _config4.Data.Datasets.Add(datasetIncomes);
                _config4.Data.Datasets.Add(datasetExpenses);
                _config4.Data.Datasets.Add(datasetEarning);
            }
        }
    }
}