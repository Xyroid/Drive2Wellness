using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Drive2Wellness.Helper
{
    public class PasswordBoxHelper
    {
        static bool isInistialised = false;

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(null, WatermarkChanged));



        public static bool GetShowWatermark(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowWatermarkProperty);
        }

        public static void SetShowWatermark(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowWatermarkProperty, value);
        }

        public static readonly DependencyProperty ShowWatermarkProperty =
            DependencyProperty.RegisterAttached("ShowWatermark", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false));



        static void WatermarkChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var pwd = obj as PasswordBox;
            CheckShowWatermark(pwd);
          
                pwd.GotFocus += new RoutedEventHandler(pwd_PasswordChanged);
                pwd.LostFocus += new RoutedEventHandler(pwd_LostFocus);
                pwd.Unloaded += new RoutedEventHandler(pwd_Unloaded);
              //  isInistialised = true;
           
        }

        private static void CheckShowWatermark(PasswordBox pwd)
        {
            pwd.SetValue(PasswordBoxHelper.ShowWatermarkProperty, pwd.Password == string.Empty);
        }

        static void pwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var p = sender as PasswordBox;
            p.SetValue(PasswordBoxHelper.ShowWatermarkProperty,false);        
        }

        static void pwd_LostFocus(object sender, RoutedEventArgs e)
        {
            var p = sender as PasswordBox;
            if (p.Password == string.Empty)
            {
                p.SetValue(PasswordBoxHelper.ShowWatermarkProperty, true);
            }
        }
        static void pwd_Unloaded(object sender, RoutedEventArgs e)
        {
            var pwd = sender as PasswordBox;
            pwd.PasswordChanged -= new RoutedEventHandler(pwd_PasswordChanged);
        }

    }
}
