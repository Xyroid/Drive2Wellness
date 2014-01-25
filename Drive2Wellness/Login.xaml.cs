using Drive2Wellness.Common;
using Drive2Wellness.Helper;
using Drive2Wellness.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRtUtility;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Drive2Wellness
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : LayoutAwarePage
    {
        public Login()
        {
            this.InitializeComponent();

            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
            
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        public static objStorageHelper oStorage = new objStorageHelper();

        public string uID;
        public string pass;
        private async void LoginOnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(uID_tb.Text) || string.IsNullOrWhiteSpace(Pass_tb.Password))
            {
                MessageDialog ms = new MessageDialog("Username and/or password is empty.", "Drive2Wellness");
                await ms.ShowAsync();
            }
            else
            {
                uID = uID_tb.Text;
                pass = Pass_tb.Password;

                //var response = WebRequest.Create(string.Format("http://drive2wellness.com/ws/ws_login.php?uname={0}&pwd={1}", uID, pass));

                HttpClient hc = new HttpClient();
                var req = await hc.GetAsync(string.Format("http://drive2wellness.com/ws/ws_login.php?uname={0}&pwd={1}", uID, pass));
                LoginModel objLoginModel;

                if (req.IsSuccessStatusCode)
                {
                    var res = await req.Content.ReadAsStringAsync();
                    objLoginModel = JsonHelper.Deserialize<LoginModel>(res);

                    if (objLoginModel.status == "true")
                    {
                        App.userName = objLoginModel.username;

                        #region saving state

                        oStorage.username = App.userName;
                        var objectStorageHelper = new ObjectStorageHelper<objStorageHelper>(StorageType.Local);
                        await objectStorageHelper.SaveAsync(oStorage);

                        #endregion

                        this.Frame.Navigate(typeof(MainPage));
                    }
                    else if (objLoginModel.status == "expire")
                    {
                        MessageDialog ms = new MessageDialog("Your Subscription is expired.", "Drive2Wellness");
                        await ms.ShowAsync();
                    }
                    else
                    {
                        MessageDialog ms = new MessageDialog("Username or password incorrect.", "Drive2Wellness");
                        await ms.ShowAsync();
                    }
                }
                else
                {
                    MessageDialog ms = new MessageDialog("Something went wrong, please try again.", "Drive2Wellness");
                    await ms.ShowAsync();
                }
            }
        }

        public async Task<bool> forgetPassword(string eMail)
        {
            HttpClient hc = new HttpClient();
            var req = await hc.GetAsync(string.Format("http://drive2wellness.com/ws/ws_forgetpass.php?email={0}", eMail));
            forgetPasswordModel fpm;

            if (req.IsSuccessStatusCode)
            {
                var res = await req.Content.ReadAsStringAsync();
                fpm = JsonHelper.Deserialize<forgetPasswordModel>(res);

                if (fpm.status == "true")
                {
                    return true;
                }
                else if (fpm.status == "false")
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void tb_forgotPassword_onTapped(object sender, TappedRoutedEventArgs e)
        {
            emailInputDialog.IsOpen = true;
        }

        private void InputDialog_BackButtonClicked(object sender, RoutedEventArgs e)
        {
            tBox_eMail.Text = "";
            emailInputDialog.IsOpen = false;
        }

        private async void submitEmailAddress(object sender, TappedRoutedEventArgs e)
        {
            if (EmailValidator.EmailIsValid(tBox_eMail.Text))
            {
                var email = tBox_eMail.Text;

                bool status = await forgetPassword(email);
                if (status)
                {
                    MessageDialog ms = new MessageDialog("Password Submited to the email address you have entered.", "Drive2Wellness");
                    await ms.ShowAsync();
                }
                else if (!status)
                {
                    MessageDialog ms = new MessageDialog("Email address you have entered does not exist.", "Drive2Wellness");
                    await ms.ShowAsync();
                }
            }
            else
            {
                MessageDialog ms = new MessageDialog("Email address you have entered is not valid.", "Drive2Wellness");
                await ms.ShowAsync();
            }

        }

        private async void btnSignup_Click(object sender, TappedRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.drive2wellness.com/subscription/"));
        }
    }
}
