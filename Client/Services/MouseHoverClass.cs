namespace Cargotruck.Client.Services
{
    public static class MouseHoverClass
    {
        public static String? MouseOnHoverClass { get; set; }
        public static String? MouseOnclickClass { get; set; }
        public static void MouseOver(String? id) { MouseOnHoverClass = id?.ToString(); }
        public static void MouseOut() { MouseOnHoverClass = null; }
        public static void MouseOnclick(String? id)
        {
            if (MouseOnclickClass != id?.ToString())
            {
                MouseOnclickClass = id?.ToString();
            }
            else
            {
                MouseOnclickClass = null;
            }
        }
    }
}
