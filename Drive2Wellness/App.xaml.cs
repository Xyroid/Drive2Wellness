using Callisto.Controls;
using Drive2Wellness.Model;
using Drive2Wellness.Usercontrols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRtUtility;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Drive2Wellness
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.Content.GetType() != typeof(Login))
            {
                #region Logout Module

                args.Request.ApplicationCommands.Add
                (
                    new SettingsCommand("Drive2Wellness", "Logout",
                        async a =>
                        {
                            var fileToDelete = await ApplicationData.Current.LocalFolder.GetFileAsync("Drive2Wellness.Model.objStorageHelperDrive2Wellness.Model.objStorageHelper");
                            await fileToDelete.DeleteAsync();
                            rootFrame.Navigate(typeof(Login));
                            //await ApplicationData.ApplicationData.Current.LocalFolder.DeleteAsync("Drive2Wellness.Model.objStorageHelperDrive2Wellness.Model.objStorageHelper");
                        }
                 ));

                #endregion

                #region Upload Module

                args.Request.ApplicationCommands.Add
                (
                    new SettingsCommand("Drive2Wellness", "Upload Data",
                        async a =>
                        {
                            await UploadDatabase();
                        }
                 ));

                #endregion

                #region Download Module

                args.Request.ApplicationCommands.Add
                (
                    new SettingsCommand("Drive2Wellness", "Download Data",
                        async a =>
                        {
                            await DownloadDatabase();
                        }
                 ));

                #endregion

                #region Goal weight setup

                args.Request.ApplicationCommands.Add
                (
                    new SettingsCommand("Drive2Wellness", "Set Goal Weight",
                        a =>
                        {
                            var GoalWeightFlyout = new SettingsFlyout();
                            GoalWeightFlyout.ContentBackgroundBrush = App.Current.Resources["AppBlueColor"] as SolidColorBrush;
                            GoalWeightFlyout.FlyoutWidth = SettingsFlyout.SettingsFlyoutWidth.Narrow;
                            GoalWeightFlyout.HeaderBrush = App.Current.Resources["AppBlueColor"] as SolidColorBrush;
                            GoalWeightFlyout.HeaderText = "Settings";
                            BitmapImage bmp = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"));
                            GoalWeightFlyout.SmallLogoImageSource = bmp;
                            GoalWeightFlyout.Content = new GoalWeightSetup();
                            GoalWeightFlyout.IsOpen = true;
                        }
                 ));

                #endregion
            }

            args.Request.ApplicationCommands.Add(new SettingsCommand("Drive2Wellness", "Privacy Policy", (x) => rootFrame.Navigate(typeof(PrivacyPolicy))));
        }

        public static int GoalWeight
        {
            set { ApplicationData.Current.LocalSettings.Values["GoalWeight"] = value; }
            get { return (int)ApplicationData.Current.LocalSettings.Values["GoalWeight"]; }
        }

        public static string userName;
        public static bool check;

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // code to check whether user was logged in or not

                check = await DoesFileExistAsync("Drive2Wellness.Model.objStorageHelperDrive2Wellness.Model.objStorageHelper");
                if (check)
                {
                    var helper_objStorage = new ObjectStorageHelper<objStorageHelper>(StorageType.Local);
                    objStorageHelper retrivedObject = await helper_objStorage.LoadAsync();

                    App.userName = retrivedObject.username;
                    rootFrame.Navigate(typeof(MainPage));
                }
                else
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    if (!rootFrame.Navigate(typeof(Login), args.Arguments))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }
            }

            SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


        #region myMethods

        HttpClient http_client = new HttpClient();
        public async Task UploadDatabase()
        {
            try
            {
                var dbFile = await ApplicationData.Current.LocalFolder.GetFileAsync("chartdb.db");
                var fileBytes = await GetBytesAsync(dbFile);

                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(App.userName), "userid");
                form.Add(new ByteArrayContent(fileBytes), "file", "chartdb.db");

                //http_client = new HttpClient();
                Uri uri = new Uri("http://drive2wellness.com/ws/ws_get_file.php");
                //http_client.DefaultRequestHeaders.Add("Content-Type", "multipart/form-data");
                HttpResponseMessage response = await http_client.PostAsync(uri, form);

                if (response.IsSuccessStatusCode)
                {
                    var a = await response.Content.ReadAsStringAsync();
                    if (a == "Success")
                    {
                        MessageDialog ms = new MessageDialog("Upload successful");
                        await ms.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog ms = new MessageDialog(ex.StackTrace);
                ms.ShowAsync();
            }
        }

        private async Task<byte[]> GetBytesAsync(StorageFile file)
        {
            byte[] fileBytes = null;
            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            return fileBytes;
        }

        public async Task DownloadDatabase()
        {
            try
            {
                HttpResponseMessage response = await http_client.GetAsync(string.Format("http://drive2wellness.com/ws/ws_get_file_res.php?userid={0}&filename={1}", App.userName, "chartdb.db"));

                if (response.IsSuccessStatusCode)
                {
                    var resultedFileAsStream = await response.Content.ReadAsStreamAsync();

                    if (response.Content.Headers.ContentLength == 8 && await response.Content.ReadAsStringAsync() == "Failure.")
                    {
                        await new MessageDialog("You have no backup on server.").ShowAsync();
                    }

                    else
                    {
                        byte[] fileAsBytes = App.ToByteArray(resultedFileAsStream);

                        StorageFolder folder = ApplicationData.Current.LocalFolder;

                        //if (await DoesFileExistAsync("chartdb.db"))
                        //{
                        //    StorageFile abc = await ApplicationData.Current.LocalFolder.GetFileAsync("chartdb.db");
                        //    await abc.DeleteAsync();
                        //}
                        
                        StorageFile file = await folder.CreateFileAsync("chartdb.db", CreationCollisionOption.ReplaceExisting);

                        await FileIO.WriteBytesAsync(file, fileAsBytes);
                        await new MessageDialog("Backup restored successfully from server.").ShowAsync();
                    }

                    #region alternative code for saving file by writing Bytes       // commented

                    //using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    //{
                    //    using (IOutputStream outputStream = fileStream.GetOutputStreamAt(0))
                    //    {
                    //        using (DataWriter dataWriter = new DataWriter(outputStream))
                    //        {
                    //            //TODO: Replace "Bytes" with the type you want to write.
                    //            dataWriter.WriteBytes(bytes);
                    //            await dataWriter.StoreAsync();
                    //            dataWriter.DetachStream();
                    //        }

                    //        await outputStream.FlushAsync();
                    //    }
                    //}

                    #endregion

                    //var status = await FileIO.wri
                }
                else
                {
                    await new MessageDialog("Error occurred while restoring backup from server. Error : " + response.StatusCode).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                MessageDialog ms = new MessageDialog(ex.ToString());
                ms.ShowAsync();
            }
        }

        public static byte[] ToByteArray(Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length; )
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            return buffer;
        }

        public static async Task<bool> DoesFileExistAsync(string fileName)
        {
            try
            {
                //using (await ApplicationData.Current.LocalFolder.GetFileAsync(fileName))
                //{
                    
                //}
                StorageFile abc = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                abc = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }
}
