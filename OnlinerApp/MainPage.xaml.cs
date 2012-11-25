using System;
using System.Windows;
using Microsoft.Phone.Controls;
using OnlinerApp.Rss;

namespace OnlinerApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        void Refresh()
        {
            RefreshT();
            RefreshA();
            RefreshD();
            RefreshN();
        }

        private void RefreshT()
        {
            string WindowsPhoneBlogPosts = "http://tech.onliner.by/feed";
            RssService.GetRssItems(WindowsPhoneBlogPosts,
                                    (items) => { listboxT.ItemsSource = items; },
                                    (exception) => { MessageBox.Show(exception.Message); },
                                    null);
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}