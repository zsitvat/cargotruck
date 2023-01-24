using Microsoft.AspNetCore.Components.Web;

namespace Cargotruck.Client.Services
{
    public static class Pages
    {
        public static String? MouseOnHoverClass { get; set; }
        public static String? MouseOnclickClass { get; set; }
        public static void MouseOver(MouseEventArgs e, String? id) { MouseOnHoverClass = id?.ToString(); }
        public static void MouseOut(MouseEventArgs e) { MouseOnHoverClass = null; }
        public static void MouseOnclick(MouseEventArgs e, String? id) 
        {
            if (MouseOnclickClass != id?.ToString()) { 
                MouseOnclickClass = id?.ToString();
            }
            else
            {
                MouseOnclickClass= null;
            }
        }
    }
}
