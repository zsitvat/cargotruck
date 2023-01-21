using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cargotruck.Shared.Models
{
    public class DateFilter
    {
        public DateTime? StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0);
        public DateTime? EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month), 23, 59, 59);
    }
}
