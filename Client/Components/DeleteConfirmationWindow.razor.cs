using Microsoft.AspNetCore.Components;
using RestSharp;

namespace Cargotruck.Client.Components
{
    public partial class DeleteConfirmationWindow
    {
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string? Controller { get; set; }

        [Parameter]
        public EventCallback OnCloseDeleteConfirmationWindow { get; set; }

        [Parameter]
        public EventCallback<bool> OnRenderParent { get; set; }

        protected async Task CloseWindow()
        {
            await OnCloseDeleteConfirmationWindow.InvokeAsync();
        }

        private async Task Delete()
        {
            var delete = await client.DeleteAsync($"api/{Controller}/delete/{Id}");
            await CloseWindow();
            await OnRenderParent.InvokeAsync(delete.IsSuccessStatusCode);
        }
    }
}