using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive2Wellness.Helper
{
    /// <summary>
    /// Type of chart
    /// </summary>
    public enum ChartType
    {
        [Display(Name="Weight")]
        Weight,

        [Display(Name="BMI")]
        BMI,

        [Display(Name="Blood Pressure")]
        blood_pressure,

        [Display(Name="Oxygen Level")]
        oxygen_level,

        [Display(Name="Glucose Level")]
        glucose_level,

        [Display(Name="Activity Time")]
        Pedometer,
    }

    /// <summary>
    /// Duration of chart
    /// </summary>
    public enum ChartDuration
    {
        [Display(Name="Last Week")]
        LastWeek,

        [Display(Name="Last Month")]
        LastMonth,

        [Display(Name="Last Year")]
        LastYear
    }
}
