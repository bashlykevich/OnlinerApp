using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using OnlinerApp.Onliner;
using OnlinerApp.Rss;

namespace OnlinerApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set
            {
                SetValue(ShowProgressProperty, value);
                if (value)
                {
                    pMain.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    pMain.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        // Using a DependencyProperty as the backing store for ShowProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowProgressProperty =
            DependencyProperty.Register("ShowProgress", typeof(bool), typeof(MainPage), new PropertyMetadata(false));

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            pMain.Items.Clear();
            foreach (OnlinerSection section in OnlinerSettings.Sections)
            {
                if (section.IsEnabled)
                {
                    PivotItem pi = new PivotItem();
                    pi.Header = section.Header;

                    ListBox lb = new ListBox();
                    lb.ItemTemplate = GenerateNewsTemplate();
                    lb.SelectionChanged += new SelectionChangedEventHandler(listbox_SelectionChanged);

                    pi.Content = lb;
                    pMain.Items.Add(pi);

                    RssService.GetRssItems(section.FeedUrl,
                                    (items) => { lb.ItemsSource = items; },
                                    (exception) => { MessageBox.Show(exception.Message); },
                                    () => { ShowProgress = false; });
                }
            }
            if (pMain.Items.Count == 0)
            {
                PivotItem pi = new PivotItem();

                TextBlock tb = new TextBlock();
                tb.Text = "Нет разделов для отображения";
                pi.Content = tb;
                pMain.Items.Add(pi);

                ShowProgress = false;                
            }
        }

        private void StartLoading()
        {
            ShowProgress = true;
            ThreadPool.QueueUserWorkItem(
                (o) =>
                {
                    this.Dispatcher.BeginInvoke(Refresh);
                });
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            StartLoading();
        }

        private void NavigateToNewsPage()
        {
            NavigationService.Navigate(new Uri(@"/UI/NewsPage.xaml", UriKind.Relative));
        }

        private void btnBarIconRefresh_Click(object sender, EventArgs e)
        {
            StartLoading();
        }

        private void btnBarEval_Click(object sender, EventArgs e)
        {
            RateApp();
        }

        private void btnBarAbout_Click(object sender, EventArgs e)
        {
            GotoAbout();
        }

        private void RateApp()
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void GotoAbout()
        {
            NavigationService.Navigate(new Uri(@"/UI/AboutPage.xaml", UriKind.Relative));
        }

        private void GotoSettings()
        {
            NavigationService.Navigate(new Uri(@"/UI/SettingsPage.xaml", UriKind.Relative));
        }  

        private void btnBarIconSettings_Click(object sender, EventArgs e)
        {
            GotoSettings();
        }

        private DataTemplate GenerateNewsTemplate()
        {
            string xaml = "";
            xaml += @"<DataTemplate "

                //+ @"x:Class=""OnlinerApp.MainPage"" "
                + @"xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" "
                + @"xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" >";
            xaml += @"<Grid >";

            xaml += @"<Grid.RowDefinitions>";
            xaml += @"<RowDefinition Height=""Auto"" />";
            if (OnlinerSettings.PicsInStripOn)
                xaml += @"<RowDefinition Height=""Auto"" />";
            xaml += @"<RowDefinition Height=""Auto"" />";
            xaml += @"<RowDefinition Height=""Auto"" />";
            xaml += @"</Grid.RowDefinitions>";

            int rowIndex = 0;
            xaml += @"<TextBlock Grid.Row=""0"" Margin=""2"" TextWrapping=""Wrap"" Text=""{Binding Title}"" FontWeight=""Bold"" FontSize=""24"" />";
            if (OnlinerSettings.PicsInStripOn)
                xaml += @"<Image Grid.Row=""" + (++rowIndex) + @""" Margin=""1"" Source=""{Binding ImageUrl}"" Stretch=""Fill""/>";
            xaml += @"<TextBlock Grid.Row=""" + (++rowIndex) + @""" Margin=""1"" TextWrapping=""Wrap"" Text=""{Binding PlainSummary}"" />";
            xaml += @"<TextBlock Grid.Row=""" + (++rowIndex) + @""" Margin=""2,2,2,30"" TextWrapping=""Wrap"" Text=""{Binding NewsFooter}""/>";

            xaml += @"</Grid>";
            xaml += @"</DataTemplate>";
            DataTemplate dt = (DataTemplate)XamlReader.Load(xaml);
            return dt;
        }

        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem == null)
                return;
            (App.Current as App).News = (sender as ListBox).SelectedItem as RssItem;
            NavigateToNewsPage();            
        }
    }
}