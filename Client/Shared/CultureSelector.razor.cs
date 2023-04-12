using Microsoft.JSInterop;
using System.Globalization;

namespace Cargotruck.Client.Shared
{
    public partial class CultureSelector
    {
        private CultureInfo[] supportedCultures = new[]
        {
            new CultureInfo("hu"),
            new CultureInfo("en"),
        };
        private CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var js = (IJSInProcessRuntime)JS;
                    js.InvokeVoid("blazorCulture.set", value.Name);
                    Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
                }
            }
        }
    }
}