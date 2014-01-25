using Drive2Wellness.Helper;
using Drive2Wellness.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Drive2Wellness.Converter
{
    public class ChartValueToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var Obj = (ChartModel)value;
            var chartType = Obj._ChartType;
            var Value = Obj.Value;
            SolidColorBrush color = null;
            SolidColorBrush RedColor = new SolidColorBrush(Colors.Red);
            SolidColorBrush GreenColor = new SolidColorBrush(Colors.Green);
            SolidColorBrush YellowColor = new SolidColorBrush(Colors.Yellow);

            switch (chartType)
            {
                case ChartType.Weight:
                    {
                        if ((Value >= App.GoalWeight && Value <= App.GoalWeight + 12.49) || (Value <= App.GoalWeight && Value >= App.GoalWeight - 12.49)) color = GreenColor;
                        else if ((Value >= App.GoalWeight - 24.9 && Value <= App.GoalWeight - 12.5) || (Value >= App.GoalWeight + 12.5 && Value <= App.GoalWeight - 24.9)) color = YellowColor;
                        else color = RedColor;
                        break;

                        //if (Value >= App.GoalWeight && Value <= App.GoalWeight + 10) color = GreenColor;
                        //else if (Value >= App.GoalWeight + 11 && Value <= App.GoalWeight + 25) color = YellowColor;
                        //else color = RedColor;
                        //break;
                    }

                case ChartType.BMI:
                    {
                        if (Value <= 29.9) color = GreenColor;
                        else if (Value >= 30.0 && Value <= 34.9) color = YellowColor;
                        else if (Value >= 35.0) color = RedColor;
                        break;
                        

                        //if (Value <= 18.4) color = RedColor;
                        //else if (Value >= 18.5 && Value <= 29.9) color = GreenColor;
                        //else if (Value >= 30 && Value <= 30.4) color = YellowColor;
                        //else color = RedColor;
                        //break;
                    }

                case ChartType.blood_pressure:
                    {
                        if (Value >= 110 && Value <= 129) color = GreenColor;
                        else if ((Value >= 130 && Value <= 159) || (Value >= 100 && Value <= 109)) color = YellowColor;
                        else color = RedColor;
                        break;

                        //if (Value >= 110 && Value <= 120) color = GreenColor;
                        //else if ((Value >= 121 && Value <= 139) || (Value >= 100 && Value <= 109)) color = YellowColor;
                        //else color = RedColor;
                        //break;
                    }

                case ChartType.oxygen_level:
                    {
                        if (Value <= 92) color = RedColor;
                        else if (Value >= 97 && Value <= 100) color = GreenColor;
                        else if (Value >= 93 && Value <= 96) color = YellowColor;
                        break;

                        //if (Value <= 89) color = RedColor;
                        //else if (Value >= 95 && Value <= 100) color = GreenColor;
                        //else if (Value >= 90 && Value <= 94) color = YellowColor;
                        //else color = RedColor;
                        //break;
                    }

                case ChartType.glucose_level:
                    {
                        if (Value <= 69) color = RedColor;
                        else if (Value >= 85 && Value <= 115) color = GreenColor;
                        else if ((Value >= 70 && Value <= 84) || (Value >= 116 && Value <= 130)) color = YellowColor;
                        else color = RedColor;
                        break;
                    }

                case ChartType.Pedometer:
                    {
                        if (Value >= 0 && Value <= 30.0) color = RedColor;
                        else if (Value >= 31 && Value <= 90) color = YellowColor;
                        else color = GreenColor;
                        break;

                        //if (Value >= 0 && Value <= 60) color = RedColor;
                        //else if (Value >= 61 && Value <= 120) color = YellowColor;
                        //else color = GreenColor;
                        //break;
                    }
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
