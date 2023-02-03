namespace Cargotruck.Client.Services
{
    public static class Page
    {
        public static int GetPageSize(int pageSize, int dataRows)
        {
            if (pageSize < 1) { pageSize = 10; }
            else if (pageSize >= dataRows) { pageSize = dataRows != 0 ? dataRows : 1; }
            return pageSize;
        }

        public static int GetMaxPage(int pageSize, int dataRows)
        {
            return (int)Math.Ceiling((decimal)((float)dataRows / (float)pageSize));
        }
    }
}
