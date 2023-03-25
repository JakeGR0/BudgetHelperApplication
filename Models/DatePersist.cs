using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetHelperApplication.Models
{
    public class DatePersist
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }    
        public DateTime LastUpdated { get; set; }
    }
}
