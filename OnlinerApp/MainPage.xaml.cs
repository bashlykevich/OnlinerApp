using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using OnlinerApp.Rss;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Windows.Media;

namespace OnlinerApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        bool PicsInStripeOn
        {
            get
            {
                bool PicsInStrip = true;
                AppSettings.TryGetSetting<bool>("PicsInStripOn", out PicsInStrip);
                return PicsInStrip;
            }
        }
        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set 
            { 
                SetValue(ShowProgressProperty, value);
                if (value)
                {
                    grMain.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    grMain.Visibility = System.Windows.Visibility.Visible;
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
            RefreshT();
            RefreshA();
            RefreshD();
            RefreshN();

            // stop progress bar
        }

        private void RefreshT()
        {
            listboxT.ItemTemplate = GenerateNewsTemplate();            
            string WindowsPhoneBlogPosts = "http://tech.onliner.by/feed";
            RssService.GetRssItems(WindowsPhoneBlogPosts,
                                    (items) => { listboxT.ItemsSource = items; },
                                    (exception) => { MessageBox.Show(exception.Message); },
                                    () => { ShowProgress = false; });            
        }

        private void RefreshA()
        {
            listboxA.ItemTemplate = GenerateNewsTemplate();
            string WindowsPhoneBlogPosts = "http://auto.onliner.by/feed";
            RssService.GetRssItems(WindowsPhoneBlogPosts,
                                    (items) => { listboxA.ItemsSource = items; },
                                    (exception) => { MessageBox.Show(exception.Message); },
                                    null);
        }

        private void RefreshD()
        {
            listboxD.ItemTemplate = GenerateNewsTemplate();
            string WindowsPhoneBlogPosts = "http://dengi.onliner.by/feed";
            RssService.GetRssItems(WindowsPhoneBlogPosts,
                                    (items) => { listboxD.ItemsSource = items; },
                                    (exception) => { MessageBox.Show(exception.Message); },
                                    null);
        }

        private void RefreshN()
        {
            listboxN.ItemTemplate = GenerateNewsTemplate();
            string WindowsPhoneBlogPosts = "http://realt.onliner.by/feed";
            RssService.GetRssItems(WindowsPhoneBlogPosts,
                                    (items) => { listboxN.ItemsSource = items; },
                                    (exception) => { MessageBox.Show(exception.Message); },
                                    null);
        }

        private void StartLoading()
        {
            ShowProgress = true;

            //spNews.Visibility = System.Windows.Visibility.Collapsed;

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

        private void ReadNews()
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

        private void DownloadigFinished()
        {
            ShowProgress = false;
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
            if (this.PicsInStripeOn)
                xaml += @"<RowDefinition Height=""Auto"" />";            
            xaml += @"<RowDefinition Height=""Auto"" />";
            xaml += @"<RowDefinition Height=""Auto"" />";
            xaml += @"</Grid.RowDefinitions>";

            int rowIndex = 0;
            xaml += @"<TextBlock Grid.Row=""0"" Margin=""2"" TextWrapping=""Wrap"" Text=""{Binding Title}"" FontWeight=""Bold"" FontSize=""24"" />";
            if (this.PicsInStripeOn)
                xaml += @"<Image Grid.Row=""" + (++rowIndex) + @""" Margin=""1"" Source=""{Binding ImageUrl}"" Stretch=""Fill""/>";
            xaml += @"<TextBlock Grid.Row=""" + (++rowIndex) + @""" Margin=""1"" TextWrapping=""Wrap"" Text=""{Binding PlainSummary}"" />";
            xaml += @"<TextBlock Grid.Row=""" + (++rowIndex) + @""" Margin=""2,2,2,20"" TextWrapping=""Wrap"" Text=""{Binding PublishedDate}"" FontStyle=""Italic"" FontSize=""16"" />";
            
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
            ReadNews();
        }
    }
}