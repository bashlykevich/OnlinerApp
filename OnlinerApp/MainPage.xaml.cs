using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using OnlinerApp.Rss;
using Microsoft.Phone.Tasks;

namespace OnlinerApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
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
            string WindowsPhoneBlogPosts = "http://tech.onliner.by/feed";
            RssService.GetRssItems(WindowsPhoneBlogPosts,
                                    (items) => { listboxT.ItemsSource = items; },
                                    (exception) => { MessageBox.Show(exception.Message); },
                                    () => {ShowProgress = false;});
        }

        private void RefreshA()
        {
            string WindowsPhoneBlogPosts = "http://auto.onliner.by/feed";
            RssService.GetRssItems(WindowsPhoneBlogPosts,
                                    (items) => { listboxA.ItemsSource = items; },
                                    (exception) => { MessageBox.Show(exception.Message); },
                                    null);
        }

        private void RefreshD()
        {
            string WindowsPhoneBlogPosts = "http://dengi.onliner.by/feed";
            RssService.GetRssItems(WindowsPhoneBlogPosts,
                                    (items) => { listboxD.ItemsSource = items; },
                                    (exception) => { MessageBox.Show(exception.Message); },
                                    null);
        }

        private void RefreshN()
        {
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

        private void grTech_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (App.Current as App).News = (((Grid)sender).DataContext as RssItem);
            ReadNews();
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
        void RateApp()
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
        void GotoAbout()
        {
            NavigationService.Navigate(new Uri(@"/UI/AboutPage.xaml", UriKind.Relative));
        }
    }
}