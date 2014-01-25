using Drive2Wellness.Common;
using Drive2Wellness.Helper;
using Drive2Wellness.Model;
using Drive2Wellness.Repository;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Drive2Wellness
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public class NameValueItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public sealed partial class MainPage : LayoutAwarePage
    {
        ChartDuration SelectedChartDuration;
        string SelectedChartType, DBPATH = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "chartdb.db");

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        /// 

        public ObservableCollection<string> question;
        public ObservableCollection<string> answer;
        QuestionModel[] objectQuestionModel;
        TipsModel[] objTipsModel;
        DateTime dateToInsert;

        private Random _random = new Random();
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            FillChartTypeComboBox();

            //DateTime date = DateTime.Now;
            //var currentMonth = date.Month;
            //var daysOfCurrentMonth = DateTime.DaysInMonth(date.Year, date.Month);

            //CB_categorySelection.ItemsSource = CategoriesList;
            //CB_categorySelection.SelectedItem = CategoriesList[0];
            selectedCategory = ((ComboBoxItem)CB_categorySelection.SelectedItem).Content.ToString();
            tb_secondColumn.Text = selectedCategory;

            dateToInsert = myDatePicker.SelectedDate;

            //lv_measurmentTable.ItemsSource = CategoriesList;
            //myDatePicker.SelectedDateChanged += selectedDate_onChange;

            var check = await App.DoesFileExistAsync("chartdb.db");
            if (!check)
            {
                CreateDatabase();
            }

            generateTable();

            #region Charting Dummy Data
            List<NameValueItem> items = new List<NameValueItem>();
            items.Add(new NameValueItem { Name = "Test1", Value = _random.Next(10, 100) });
            items.Add(new NameValueItem { Name = "Test2", Value = _random.Next(10, 100) });
            items.Add(new NameValueItem { Name = "Test3", Value = _random.Next(10, 100) });
            items.Add(new NameValueItem { Name = "Test4", Value = _random.Next(10, 100) });
            items.Add(new NameValueItem { Name = "Test5", Value = _random.Next(10, 100) });

            //((ColumnSeries)this.Chart.Series[0]).ItemsSource = items;

            //((StackedColumnSeries)this.StackedColumn.Series[0]).SeriesDefinitions[0].ItemsSource =
            //    items;
            //((StackedColumnSeries)this.StackedColumn.Series[0]).SeriesDefinitions[1].ItemsSource =
            //    items;
            //((StackedColumnSeries)this.StackedColumn.Series[0]).SeriesDefinitions[2].ItemsSource =
            //    items;
            #endregion

            #region fetching tips

            HttpClient hc_forTips = new HttpClient();
            var req_forTips = await hc_forTips.GetAsync("http://drive2wellness.com/ws/ws_tips.php");

            if (req_forTips.IsSuccessStatusCode)
            {
                var res = await req_forTips.Content.ReadAsStringAsync();
                objTipsModel = JsonHelper.Deserialize<TipsModel[]>(res);

                foreach (var a in objTipsModel)
                {
                    switch (a.id)
                    {
                        case "1":
                            a.headings = "First Tip";
                            break;

                        case "2":
                            a.headings = "Second Tip";
                            break;

                        case "3":
                            a.headings = "Third Tip";
                            break;

                        case "4":
                            a.headings = "Fourth Tip";
                            break;

                        case "5":
                            a.headings = "Fifth Tip";
                            break;

                        case "6":
                            a.headings = "Sixth Tip";
                            break;

                        case "7":
                            a.headings = "Seventh Tip";
                            break;

                        case "8":
                            a.headings = "Eighth Tip";
                            break;
                    }
                }

                lv_tips.ItemsSource = objTipsModel;
            }
            else
            {
                MessageDialog ms = new MessageDialog("Something went wrong in fetching the Tips, please try again.", "Drive2Wellness");
                await ms.ShowAsync();
            }

            #endregion

            #region fetching Previous Q & A

            HttpClient hc = new HttpClient();
            var req = await hc.GetAsync(string.Format("http://drive2wellness.com/ws/ws_question.php?uname={0}", App.userName));

            if (req.IsSuccessStatusCode)
            {
                var res = await req.Content.ReadAsStringAsync();
                objectQuestionModel = JsonHelper.Deserialize<QuestionModel[]>(res);

                listView_QandA.ItemsSource = objectQuestionModel;

                #region commented
                //question.Add(objectQuestionModel.question);

                //CreateDatabase();
                //SQLiteAsyncConnection conn = new SQLiteAsyncConnection("QuestionAnswers");

                //QuestionModel qm = new QuestionModel
                //{
                //    question = objectQuestionModel.question,
                //    answer = objectQuestionModel.answer
                //};

                //await conn.InsertAsync(qm);
                #endregion
            }
            else
            {
                MessageDialog ms = new MessageDialog("Something went wrong in fetching the previous Qestions & Answers, please try again.", "Drive2Wellness");
                await ms.ShowAsync();
            }

            #endregion



        }

        private void CreateDatabase()
        {
            using (var db = new SQLiteConnection(System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "chartdb.db")))
            {
                db.CreateTable<CategoriesTable>();
            }

        }

        public string query;
        private async void SendQuery_onTapped(object sender, TappedRoutedEventArgs e)
        {
            query = QueryBox_tBox.Text;
            if (string.IsNullOrWhiteSpace(query) || query == "Enter Your Question")
            {
                SolidColorBrush colorBrush = new SolidColorBrush();
                colorBrush.Color = Colors.Red;

                QueryBox_tBox.BorderBrush = colorBrush;
                QueryBox_tBox.BorderThickness = new Thickness(3);
                return;
                //MessageDialog ms = new MessageDialog("Write your query.","Drive2Wellness");
                //await ms.ShowAsync();
            }

            try
            {
                HttpClient hc = new HttpClient();
                var req = await hc.GetAsync(string.Format("http://drive2wellness.com/ws/ws_question.php?uname={0}&que={1}", App.userName, query));
                SendQueryModel objectSendQueryModel;

                if (req.IsSuccessStatusCode)
                {
                    var res = await req.Content.ReadAsStringAsync();
                    objectSendQueryModel = JsonHelper.Deserialize<SendQueryModel>(res);

                    if (objectSendQueryModel.status == "true")
                    {

                        MessageDialog ms = new MessageDialog("Your question is posted.", "Drive2Wellness");
                        await ms.ShowAsync();

                        QueryBox_tBox.Text = "";
                    }
                    else if (objectSendQueryModel.status == "false")
                    {
                        MessageDialog ms = new MessageDialog("Your question is not posted, please try again.", "Drive2Wellness");
                        await ms.ShowAsync();
                    }
                }
                else
                {
                    MessageDialog ms = new MessageDialog("Something went wrong, please try again.", "Drive2Wellness");
                    await ms.ShowAsync();
                }
            }
            catch (Exception exc)
            {

            }
            finally
            {
                QueryBox_tBox.BorderBrush = new SolidColorBrush(Color.FromArgb(163, 0, 0, 0));
                QueryBox_tBox.BorderThickness = new Thickness(2);
            }
        }

        public int activityTime;
        public int converted_weight;
        public int converted_oxyzenLevel;
        public int converted_glucoze;
        public int converted_BMI;
        public int minBloodPressure;
        public int maxBloodPressure;

        public async Task<bool> isValidData()
        {
            if (string.IsNullOrWhiteSpace(tb_weight.Text) || string.IsNullOrWhiteSpace(tb_boodPressure.Text) ||
                   string.IsNullOrWhiteSpace(tb_oxyzenLevel.Text) || //string.IsNullOrWhiteSpace(tb_glucozeLevel.Text) ||
                   string.IsNullOrWhiteSpace(tb_BMI.Text))
            {
                //MessageDialog ms = new MessageDialog("Every field is compulsory.", "Drive2Wellness");
                //await ms.ShowAsync();
                await new MessageDialog("Every field is compulsory.", "Drive2Wellness").ShowAsync();
                return false;
            }
            else
            {
                if (dateToInsert > DateTime.Now)
                {
                    await new MessageDialog("You are not allowed to enter data for future date.", "Drive2Wellness").ShowAsync();
                    return false;
                }

                using (var db = new SQLiteConnection(System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "chartdb.db")))
                {
                    var UniqueEntryFlag = (db.Query<CategoriesTable>("select * from CategoriesTable where date = ? and username = ?",
                        dateToInsert, App.userName)).Count > 0;
                    if (UniqueEntryFlag)
                    {
                        await new MessageDialog("You have already entered value for " + dateToInsert.ToString("MM-dd-yyyy") + ".", "Drive2Wellness").ShowAsync();

                        return false;
                    }
                }

                if (int.TryParse(tb_weight.Text, out converted_weight))
                {
                    if (!(converted_weight >= 100 && converted_weight <= 400))
                    {
                        await new MessageDialog("Weight is no in it's range.", "Drive2Wellness").ShowAsync();
                        return false;
                    }
                }
                else { return false; }

                if (int.TryParse(tb_oxyzenLevel.Text, out converted_oxyzenLevel))
                {
                    if (!(converted_oxyzenLevel >= 70 && converted_oxyzenLevel <= 100))
                    {
                        await new MessageDialog("Oxyzen Level is not in it's range.", "Drive2Wellness").ShowAsync();
                        return false;
                    }
                }
                else { return false; }

                if (!string.IsNullOrWhiteSpace(tb_glucozeLevel.Text))
                {
                    if (int.TryParse(tb_glucozeLevel.Text, out converted_glucoze))
                    {
                        if (!(converted_glucoze >= 30 && converted_glucoze <= 375))
                        {
                            await new MessageDialog("Glucoze Level is not in it's range.", "Drive2Wellness").ShowAsync();
                            return false;
                        }
                    }
                    else { return false; }
                }

                if (int.TryParse(tb_BMI.Text, out converted_BMI))
                {
                    if (!(converted_BMI >= 15 && converted_BMI <= 55))
                    {
                        await new MessageDialog("BMI is not in it's range.", "Drive2Wellness").ShowAsync();
                        return false;
                    }
                }
                else { return false; }

                var bloodPressure = tb_boodPressure.Text;
                if (bloodPressure.Contains("/"))
                {
                    string[] strings = bloodPressure.Split('/');
                    if (strings.Count() > 2)
                    {
                        await new MessageDialog("Blood Pressure value is not valid.", "Drive2Wellness").ShowAsync();
                        return false;
                    }
                    else
                    {
                        if (int.TryParse(strings[0], out minBloodPressure) && int.TryParse(strings[1], out maxBloodPressure))
                        {
                            if ((minBloodPressure >= 60 && minBloodPressure <= 240) && (maxBloodPressure >= 40 && maxBloodPressure <= 120))
                            {
                                // great.. everything's okay..
                                return true;
                            }
                            else
                            {
                                await new MessageDialog("Blood Pressure is not in it's range.", "Drive2Wellness").ShowAsync();
                                return false;
                            }
                        }
                        else
                        {
                            await new MessageDialog("Blood Pressure is not in it's range.", "Drive2Wellness").ShowAsync();
                            return false;
                        }
                    }
                }
                else
                {
                    await new MessageDialog("Blood Pressure value is not valid.", "Drive2Wellness").ShowAsync();
                    return false;
                }

            }

        }

        private async void generateGraph_onTapped(object sender, TappedRoutedEventArgs e)
        {
            // check whether DB file already there or not..
            var check = await App.DoesFileExistAsync("chartdb.db");
            if (!check)
            {
                CreateDatabase();
            }

            if (await isValidData())
            {
                #region activity time => integer converter

                var a = cb_activityTime.SelectedIndex;
                switch (a)
                {
                    case 0:
                        activityTime = 0;
                        break;

                    case 1:
                        activityTime = 15;
                        break;

                    case 2:
                        activityTime = 30;
                        break;

                    case 3:
                        activityTime = 45;
                        break;

                    case 4:
                        activityTime = 60;
                        break;

                    case 5:
                        activityTime = 75;
                        break;

                    case 6:
                        activityTime = 90;
                        break;

                    case 7:
                        activityTime = 105;
                        break;

                    case 8:
                        activityTime = 120;
                        break;

                    case 9:
                        activityTime = 135;
                        break;

                    case 10:
                        activityTime = 150;
                        break;

                    case 11:
                        activityTime = 165;
                        break;

                    case 12:
                        activityTime = 180;
                        break;
                }

                #endregion

                // connect to DB and insert current values in DB

                SQLiteAsyncConnection con = new SQLiteAsyncConnection("chartdb.db");
                CategoriesTable data = new CategoriesTable
                {
                    username = App.userName,
                    weight = converted_weight,
                    blood_pressure = tb_boodPressure.Text,
                    oxygen_level = converted_oxyzenLevel,
                    glucose_level = converted_glucoze,
                    bmi = double.Parse(converted_BMI.ToString()),
                    pedometer = activityTime,
                    //pedometer = int.Parse(cb_activityTime.SelectedItem.ToString()),
                    date = dateToInsert
                };
                await con.InsertAsync(data);
                
                generateTable();
                await CreateChartAsync();

                SQLiteConnectionPool.Shared.Reset();

                tb_weight.Text = string.Empty;
                tb_boodPressure.Text = string.Empty;
                tb_oxyzenLevel.Text = string.Empty;
                tb_glucozeLevel.Text = string.Empty;
                tb_BMI.Text = string.Empty;
            }
            //else
            //{
            //    MessageDialog ms = new MessageDialog("Data you entered is not in it's range.", "Drive2Wellness");
            //    await ms.ShowAsync();
            //}
        }

        //public ObservableCollection<string> measurmentTypeFilter;
        //public ObservableCollection<string> measurmentDurationFilter;
        public List<TableModel> list_TableModel = new List<TableModel>();
        public void generateTable()
        {
            if (selectedTimePeriod == "Last Week")
            {
                using (var db = new SQLiteConnection(System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "chartdb.db")))
                {
                    list_TableModel = db.Query<TableModel>("SELECT " + selectedCategory + " as categories, date as Date FROM CategoriesTable WHERE Date BETWEEN datetime('now', '-8 days') AND datetime('now', 'localtime') and username = ?", App.userName);
                }
            }
            else if (selectedTimePeriod == "Last Month")
            {
                using (var db = new SQLiteConnection(System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "chartdb.db")))
                {
                    list_TableModel = db.Query<TableModel>("SELECT " + selectedCategory + " as categories, date as Date FROM CategoriesTable WHERE Date BETWEEN datetime('now', '-1 month') AND datetime('now', 'localtime') and username = ?", App.userName);
                }
            }

            foreach (var a in list_TableModel)
            {
                a.DayOfWeek = (a.Date.DayOfWeek.ToString()).Substring(0, 3);
                a.DayOfMonth = a.Date.Day.ToString();
                a.Month = a.Date.Month.ToString();
                a.Year = a.Date.Year.ToString();
            }

            lv_measurmentTable.ItemsSource = list_TableModel;
        }



        public string selectedCategory = "Weight";
        public string selectedTimePeriod = "Last Week";
        private void CB_categorySelection_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var temp_selectedCategory = ((ComboBoxItem)CB_categorySelection.SelectedItem).Content.ToString();
                tb_secondColumn.Text = temp_selectedCategory;

                switch (temp_selectedCategory)
                {
                    case "Weight":
                        selectedCategory = "weight";
                        break;

                    case "BMI":
                        selectedCategory = "bmi";
                        break;

                    case "Blood Pressure":
                        selectedCategory = "blood_pressure";
                        break;

                    case "Oxygen Level":
                        selectedCategory = "oxygen_level";
                        break;

                    case "Glucose Level":
                        selectedCategory = "glucose_level";
                        break;

                    case "Activity Time":
                        selectedCategory = "pedometer";
                        break;
                }

                generateTable();
            }
            catch (NullReferenceException ex)
            { }
        }

        private void CB_timePeriodSelection_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                selectedTimePeriod = ((ComboBoxItem)CB_timePeriodSelection.SelectedItem).Content.ToString();
                generateTable();
            }
            catch (Exception ex)
            {

            }
        }

        private void selectedDate_onChange(object sender, WinRTDatePicker.SelectedDateChangedEventArgs e)
        {
            dateToInsert = e.NewDate;
        }

        #region Graph

        private async void elipChartDuration_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var chartDuration = ((Ellipse)sender).Tag.ToString();
            switch (chartDuration)
            {
                case "Last Week":
                    {
                        tbLastMonth.Foreground = App.Current.Resources["LightGrayTextColor"] as SolidColorBrush;
                        tbLastYear.Foreground = App.Current.Resources["LightGrayTextColor"] as SolidColorBrush;
                        tbLastWeek.Foreground = App.Current.Resources["OrangeTextColor"] as SolidColorBrush;

                        elipLastMonth.Visibility = Visibility.Collapsed;
                        elipLastYear.Visibility = Visibility.Collapsed;
                        elipLastWeek.Visibility = Visibility.Visible;

                        SelectedChartDuration = ChartDuration.LastWeek;
                        break;
                    }

                case "Last Month":
                    {
                        tbLastWeek.Foreground = App.Current.Resources["LightGrayTextColor"] as SolidColorBrush;
                        tbLastYear.Foreground = App.Current.Resources["LightGrayTextColor"] as SolidColorBrush;
                        tbLastMonth.Foreground = App.Current.Resources["OrangeTextColor"] as SolidColorBrush;

                        elipLastYear.Visibility = Visibility.Collapsed;
                        elipLastWeek.Visibility = Visibility.Collapsed;
                        elipLastMonth.Visibility = Visibility.Visible;

                        SelectedChartDuration = ChartDuration.LastMonth;
                        break;
                    }

                case "Last Year":
                    {
                        tbLastWeek.Foreground = App.Current.Resources["LightGrayTextColor"] as SolidColorBrush;
                        tbLastMonth.Foreground = App.Current.Resources["LightGrayTextColor"] as SolidColorBrush;
                        tbLastYear.Foreground = App.Current.Resources["OrangeTextColor"] as SolidColorBrush;

                        elipLastWeek.Visibility = Visibility.Collapsed;
                        elipLastMonth.Visibility = Visibility.Collapsed;
                        elipLastYear.Visibility = Visibility.Visible;

                        SelectedChartDuration = ChartDuration.LastYear;
                        break;
                    }
            }

            await CreateChartAsync();
        }

        private async Task CreateChartAsync()
        {
            try
            {
                if (await App.DoesFileExistAsync("chartdb.db"))
                {
                    #region No goal weight no chart

                    if (SelectedChartType == "Weight" && !ApplicationData.Current.LocalSettings.Values.ContainsKey("GoalWeight"))
                    {
                        //await new MessageDialog("Set your goal weight to see how close you are from goal weight.").ShowAsync();
                        //await new MessageDialog("You can't see weight chart until you set goal weight from the settings.").ShowAsync();
                        await new MessageDialog("Set goal weight in settings.").ShowAsync();
                        return;
                    }

                    #endregion

                    await Task.Delay(100);

                    #region Get data from database & assign to chart

                    List<ChartModel> lstChartData = new List<ChartModel>();

                    if (SelectedChartDuration == ChartDuration.LastWeek)
                    {
                        using (var db = new SQLiteConnection(DBPATH)) //strftime('%d-%m-%Y', date)
                        {
                            lstChartData = db.Query<ChartModel>("SELECT " + SelectedChartType.ToString() + " as Value," + SelectedChartType.ToString() + " as ValueString, date as Date FROM CategoriesTable WHERE Date BETWEEN datetime('now', '-7 days') AND datetime('now', 'localtime') and username = ?", App.userName);
                            foreach (var data in lstChartData)
                            {
                                data.Date = DateTime.Parse(data.Date, System.Globalization.CultureInfo.InvariantCulture).ToString("ddd MM-dd-yyyy");
                                data._ChartType = (ChartType)Enum.Parse(typeof(ChartType), SelectedChartType);
                            }
                        }
                    }

                    else if (SelectedChartDuration == ChartDuration.LastMonth)
                    {
                        using (var db = new SQLiteConnection(DBPATH))
                        {
                            lstChartData = db.Query<ChartModel>("SELECT avg(" + SelectedChartType.ToString() + ") as Value FROM CategoriesTable WHERE date BETWEEN datetime('now', '-1 month') AND datetime('now', 'localtime') group by strftime('%W', date) and username = ?", App.userName);
                            string[] WeekOrdinalNumber = new string[] { "1st week", "2nd Week", "3rd Week", "4th Week", "5th Week" };
                            for (int i = 0; i < lstChartData.Count; i++)
                            {
                                lstChartData[i].Date = WeekOrdinalNumber[i];
                                lstChartData[i]._ChartType = (ChartType)Enum.Parse(typeof(ChartType), SelectedChartType);
                            }
                        }
                    }

                    else if (SelectedChartDuration == ChartDuration.LastYear)
                    {
                        using (var db = new SQLiteConnection(DBPATH))
                        {
                            lstChartData = db.Query<ChartModel>("SELECT avg(" + SelectedChartType.ToString() + ") as Value, date as Date FROM CategoriesTable WHERE Date BETWEEN datetime('now', '-1 year') AND datetime('now', 'localtime') group by strftime('%m-%Y', Date) and username = ?", App.userName);
                            foreach (var data in lstChartData)
                            {
                                data.Date = DateTime.ParseExact(data.Date, "yyyy-MM-dd hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToString("MMM");
                                data._ChartType = (ChartType)Enum.Parse(typeof(ChartType), SelectedChartType);
                            }
                        }
                    }

                    ((ColumnSeries)Chart.Series[0]).ItemsSource = lstChartData;

                    #endregion

                    await Task.Delay(100);

                    #region Get dependent axis (Y-Axis)

                    ((ColumnSeries)Chart.Series[0]).DependentRangeAxis = GetDependentAxis((ChartType)Enum.Parse(typeof(ChartType), SelectedChartType));

                    #endregion

                    await Task.Delay(100);

                    #region Chart's goal value setup

                    switch ((ChartType)Enum.Parse(typeof(ChartType), SelectedChartType))
                    {
                        case ChartType.oxygen_level:
                            {
                                tbReferenceText.Text = "* DOT Medical Compliance Guideline.";
                                Chart.GoalValue = 92;
                                Chart.IsGoalLineVisible = Visibility.Visible;
                                break;
                            }
                        case ChartType.glucose_level:
                            {
                                Chart.IsGoalLineVisible = Visibility.Collapsed;
                                tbReferenceText.Text = string.Empty;
                                break;
                            }
                        case ChartType.Weight:
                            {
                                tbReferenceText.Text = string.Empty;
                                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("GoalWeight"))
                                {
                                    Chart.GoalValue = App.GoalWeight;
                                    Chart.IsGoalLineVisible = Visibility.Visible;
                                }
                                else
                                {
                                    Chart.IsGoalLineVisible = Visibility.Collapsed;
                                }
                                break;
                            }

                        case ChartType.BMI:
                            {
                                tbReferenceText.Text = "* DOT Medical Regulation Requirement.";
                                Chart.GoalValue = 35.0;
                                Chart.IsGoalLineVisible = Visibility.Visible;
                                break;
                            }

                        case ChartType.blood_pressure:
                            {
                                tbReferenceText.Text = "* DOT Medical Regulation Requirement.";
                                Chart.GoalValue = 160.0;
                                Chart.IsGoalLineVisible = Visibility.Visible;
                                break;
                            }

                        case ChartType.Pedometer:
                            {
                                tbReferenceText.Text = "* Recommended Amount of Daily Exercise.";
                                Chart.GoalValue = 30;
                                Chart.IsGoalLineVisible = Visibility.Visible;
                                break;
                            }
                    }

                    #endregion
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private LinearAxis GetDependentAxis(ChartType chartType)
        {
            LinearAxis linearAxis = new LinearAxis
            {
                Orientation = AxisOrientation.Y,
                ShowGridLines = true
            };
            switch (chartType)
            {
                case ChartType.Weight:
                    {
                        linearAxis.Minimum = 100;
                        linearAxis.Maximum = 480;
                        linearAxis.Interval = 50;
                        break;
                    }
                case ChartType.BMI:
                    {
                        linearAxis.Minimum = 15;
                        linearAxis.Maximum = 80;
                        linearAxis.Interval = 10;
                        break;
                    }
                case ChartType.blood_pressure:
                    {
                        linearAxis.Minimum = 60;
                        linearAxis.Maximum = 300;
                        linearAxis.Interval = 30;
                        break;
                    }
                case ChartType.oxygen_level:
                    {
                        linearAxis.Minimum = 70;
                        linearAxis.Maximum = 130;
                        linearAxis.Interval = 10;
                        break;
                    }
                case ChartType.glucose_level:
                    {
                        linearAxis.Minimum = 30;
                        linearAxis.Maximum = 440;
                        linearAxis.Interval = 50;
                        break;
                    }
                case ChartType.Pedometer:
                    {
                        linearAxis.Minimum = 15;
                        linearAxis.Maximum = 370;
                        linearAxis.Interval = 40;
                        break;
                    }
            }

            return linearAxis;
        }

        private void FillChartTypeComboBox()
        {
            cmbChartType.ItemsSource = GetEnumWithDescription(typeof(ChartType));
            cmbChartType.SelectedIndex = 0;
        }

        public Dictionary<Enum, string> GetEnumWithDescription(Type type)
        {
            Dictionary<Enum, string> list = new Dictionary<Enum, string>();

            try
            {
                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }

                Array enumValues = Enum.GetValues(type);

                foreach (Enum value in enumValues)
                {
                    list.Add(value, GetDescription(value));
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return list;
        }

        public string GetDescription(Enum value)
        {
            try
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                string description = value.ToString();
                FieldInfo fieldInfo = value.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == description.ToString());
                DisplayAttribute[] attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

                if (attributes != null && attributes.Length > 0)
                {
                    description = attributes[0].Name;
                }
                return description;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private async void cmbChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbChartType.SelectedValue != null)
            {
                SelectedChartType = cmbChartType.SelectedValue.ToString();
                await CreateChartAsync();
            }
        }

        #endregion

        private void grdSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (ApplicationView.TryUnsnap() || ApplicationView.Value != ApplicationViewState.Snapped)
            {
                Windows.UI.ApplicationSettings.SettingsPane.Show();
            }
        }

        public VirtualKey[] keySet = { VirtualKey.Number0, VirtualKey.Number1, VirtualKey.Number2, VirtualKey.Number3, VirtualKey.Number4,
                                       VirtualKey.Number5, VirtualKey.Number6, VirtualKey.Number7, VirtualKey.Number8, VirtualKey.Number9,
                                       VirtualKey.NumberPad0, VirtualKey.NumberPad1, VirtualKey.NumberPad2, VirtualKey.NumberPad3,
                                       VirtualKey.NumberPad4, VirtualKey.NumberPad5, VirtualKey.NumberPad6, VirtualKey.NumberPad7,
                                       VirtualKey.NumberPad8, VirtualKey.NumberPad9};

        //List<VirtualKey> keySet = new List<VirtualKey>();
        private void tb_weight_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (((TextBox)sender).Name == "tb_boodPressure")
            {
                if (!(keySet.Contains(e.Key) || e.Key == VirtualKey.Divide || e.KeyStatus.ScanCode == 53 ))
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (!keySet.Contains(e.Key))
                {
                    e.Handled = true;
                }
            }
        }
    }
}
