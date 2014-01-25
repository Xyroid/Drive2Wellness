using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Storage;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Drive2Wellness.Usercontrols
{
    public sealed partial class GoalWeightSetup : UserControl
    {
        int GoalWeight = 0;
        Regex IntegerRegEx = new Regex("^[0-9]*$");
        public GoalWeightSetup()
        {
            this.InitializeComponent();
            Loaded += GoalWeightSetup_Loaded;
        }

        void GoalWeightSetup_Loaded(object sender, RoutedEventArgs e)
        {
            txtGoalWeight.Text = ApplicationData.Current.LocalSettings.Values.ContainsKey("GoalWeight") ?
                       App.GoalWeight.ToString() : string.Empty;
        }

        private void txtGoalWeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!IntegerRegEx.IsMatch(txtGoalWeight.Text))
                {
                    int CursorIndex = txtGoalWeight.SelectionStart - 1;
                    if (CursorIndex != -1)
                    {
                        txtGoalWeight.Text = txtGoalWeight.Text.Remove(CursorIndex, 1);

                        txtGoalWeight.SelectionStart = CursorIndex;
                        txtGoalWeight.SelectionLength = 0;
                    }
                }
                
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private async void grdSetGoalWeight_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtGoalWeight.Text))
                {
                    if (int.TryParse(txtGoalWeight.Text, out GoalWeight))
                    {
                        if (!(GoalWeight >= 100 && GoalWeight <= 400))
                        {
                            txtGoalWeight.Text = string.Empty;
                            await new MessageDialog("Goal weight is not in range.", "Drive2Wellness").ShowAsync();
                        }
                        else
                        {
                            ApplicationData.Current.LocalSettings.Values["GoalWeight"] = GoalWeight;
                            await new MessageDialog("Goal weight is set.").ShowAsync();
                        }
                    }
                    else
                    {
                        txtGoalWeight.Text = string.Empty;
                        await new MessageDialog("Goal weight can not be null.", "Drive2Wellness").ShowAsync();
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
