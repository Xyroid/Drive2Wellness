using Drive2Wellness.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive2Wellness.Model
{
    public class ChartModel
    {
        public string Date { get; set; }
        public double Value { get; set; }
        public string ValueString { get; set; }
        public ChartType _ChartType { get; set; }
    }
}
