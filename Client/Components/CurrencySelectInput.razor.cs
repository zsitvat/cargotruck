
using Cargotruck.Client.Services;
using Microsoft.AspNetCore.Components;

namespace Cargotruck.Client.Components
{
    public partial class CurrencySelectInput
    {
        
        [Parameter]
        public EventCallback OnCurrencyChanged { get; set; }

        private readonly List<string> items = new()
        {
            ("HUF"),
            ("EUR"),
            ("USD"),
            ("CZK")
        };

        protected async Task CurrencyChangedAsync()
        {
            await OnCurrencyChanged.InvokeAsync();
        }
    }
}