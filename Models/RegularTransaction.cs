using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetHelperApplication.Models
{
    public partial class RegularTransaction
    {   
        
       
       

        
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public bool Incoming { get; set; }
        public DateTime LPDate { get; set; }
        public long CurrencyAmount { get; set; }
        public int Frequency { get; set; }
        public enum FrequencyIncrement
        {
            Day = 0,
            Week,
            Month
        }
        public FrequencyIncrement increment { get; set; }

        public string VisibleAmount
        {
            get
            {
                return (CurrencyAmount / 100.0).ToString("c");
            }
        }

        public string VisibleIncrement
        {
            get
            {
                if (Frequency == 1)
                {
                    return increment.ToString();
                }
                else return increment.ToString() + 's';
            }
        }

        public string VisibleDate
        {
            get
            {
                return LPDate.ToShortDateString();
            }
        }

    }
}
