using Microsoft.AspNetCore.Components;
using RestSharp;

namespace Cargotruck.Client.Components
{
    public partial class SavedWindow
    {
        [Parameter]
        public EventCallback OnClose { get; set; }

        protected async Task CloseWindow()
        {
            await OnClose.InvokeAsync();
        }
    }
}