namespace Cargotruck.Shared.Models.Request
{
    public class DateFilter
    {
        public DateTime? StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0);
        public DateTime? EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);
    }
}
