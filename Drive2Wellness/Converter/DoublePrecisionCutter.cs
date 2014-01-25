using Drive2Wellness.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Drive2Wellness.Converter
{
    public class DoublePrecisionCutter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var Obj = (ChartModel)value;
            if (Obj.ValueString != null)
            {
                return Obj.ValueString;
            }
            else
            {
                return System.Convert.ToInt32(Obj.Value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
