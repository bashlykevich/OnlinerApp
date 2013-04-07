using System;
using System.Net;
using System.Windows;
using HtmlAgilityPack;
using Microsoft.Phone.Controls;
using OnlinerApp.Rss;
using System.Threading;
using System.Text.RegularExpressions;
using OnlinerApp.Onliner;
using OnlinerApp.Core;
using Microsoft.Phone.Tasks;

namespace OnlinerApp.UI
{
    public partial class NewsPage : PhoneApplicationPage
    {
        public static readonly DependencyProperty ShowProgressProperty = DependencyProperty.Register("ShowProgress", typeof(bool), typeof(NewsPage), new PropertyMetadata(false));
        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }
        private RssItem rssItem;
        private News news;

        public NewsPage()
        {
            InitializeComponent();
        }

        #region event handlers

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNews();
        }

        private void barBtnBack_Click(object sender, EventArgs e)
        {
            ReturnToMainPage();
        }

        #endregion

        void LoadNews()
        {
            StatusStringOn();
            rssItem = (App.Current as App).News;
            news = new News(rssItem);
            news.Loaded += new NewsLoaded(news_Loaded);
        }

        void FinishLoading()
        {
            this.wbNews.NavigateToString(news.NewsPage);           
            StatusStringOff();
        }
        void news_Loaded(News news)
        {
            ThreadPool.QueueUserWorkItem(
              (o) =>
              {
                  this.Dispatcher.BeginInvoke(FinishLoading);
              });
        }

        void ReturnToMainPage()
        {
            this.NavigationService.GoBack();
        }

        void StatusStringOn()
        {
            ShowProgress = true;
            wbNews.Visibility = System.Windows.Visibility.Collapsed;            
        }
        void StatusStringOff()
        {
            ShowProgress = false;
            wbNews.Visibility = System.Windows.Visibility.Visible;            
        }
        void GotoBrowser()
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(rssItem.Url);
            wbt.Show();
        }
        private void barBtnGotoBrowser_Click(object sender, EventArgs e)
        {
            GotoBrowser();
        }

        private void barBtnGotoBrowser_Click_1(object sender, EventArgs e)
        {
            GotoBrowser();
        }
    }
}