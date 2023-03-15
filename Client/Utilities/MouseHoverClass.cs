namespace Cargotruck.Client.UtilitiesClasses
{
    public static class MouseHoverClass
    {
        public static string? MouseOnHoverClass { get; set; }
        public static string? MouseOnclickClass { get; set; }
        public static void MouseOver(string? id) { MouseOnHoverClass = id?.ToString(); }
        public static void MouseOut() { MouseOnHoverClass = null; }
        public static void MouseOnclick(string? id)
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
