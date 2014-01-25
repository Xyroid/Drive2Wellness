using Drive2Wellness.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Drive2Wellness
{
    public sealed partial class PrivacyPolicy : LayoutAwarePage
    {
        public PrivacyPolicy()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private async void richTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var richTB = sender as RichTextBlock;
            var textPointer = richTB.GetPositionFromPoint(e.GetPosition(richTB));

            var element = textPointer.Parent as TextElement;
            while (element != null && !(element is Underline))
            {
                if (element.ContentStart != null
                    && element != element.ElementStart.Parent)
                {
                    element = element.ElementStart.Parent as TextElement;
                }
                else
                {
                    element = null;
                }
            }

            if (element == null) return;

            var underline = element as Underline;
            if (underline.Name == "urlIntuit")
            {
                await Launcher.LaunchUriAsync(new Uri("http://security.intuit.com/privacy"));
            }
            else if (underline.Name == "urlDrive2Wellness" || underline.Name == "urlDrive2Wellness1")
            {
                await Launcher.LaunchUriAsync(new Uri("http://www.drive2wellness.com/"));
            }
            else if (underline.Name == "urlEmailD2W")
            {
                await Launcher.LaunchUriAsync(new Uri("mailto:drive2wellness@gmail.com?subject=Drive2Wellness%20%E2%80%94%20Windows%208"));
            }
        }

        private void ScrollViewer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void richTextBlock_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {

        }

        private async void btnUrl_Clicked(object sender, RoutedEventArgs e)
        {
            var TextButton = (Button)sender;
            if (TextButton.Name == "urlIntuit")
            {
                await Launcher.LaunchUriAsync(new Uri("http://security.intuit.com/privacy"));
            }
            else if (TextButton.Name == "urlDrive2Wellness" || TextButton.Name == "urlDrive2Wellness1")
            {
                await Launcher.LaunchUriAsync(new Uri("http://www.drive2wellness.com/"));
            }
            else if (TextButton.Name == "urlEmailD2W")
            {
                await Launcher.LaunchUriAsync(new Uri("mailto:drive2wellness@gmail.com?subject=Drive2Wellness%20%E2%80%94%20Windows%208"));
            }
        }
    }
}
