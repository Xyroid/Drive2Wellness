using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive2Wellness.Model
{
    public class TableModel
    {
        public string categories { get; set; }
        //public string Date { get; set; }
        public DateTime Date { get; set; }

        public string DayOfWeek { get; set; }
        public string DayOfMonth { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
    }
}
