using System;
using System.Net;
using System.Windows;
using HtmlAgilityPack;
using Microsoft.Phone.Controls;
using OnlinerApp.Rss;
using System.Threading;

namespace OnlinerApp.UI
{
    public partial class NewsPage : PhoneApplicationPage
    {
        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowProgressProperty =
            DependencyProperty.Register("ShowProgress", typeof(bool), typeof(NewsPage), new PropertyMetadata(false));



        private RssItem rssItem;

        public NewsPage()
        {
            InitializeComponent();
        }
        void StartLoading()
        {
            ShowProgress = true;
            spNews.Visibility = System.Windows.Visibility.Collapsed;

            ThreadPool.QueueUserWorkItem(
                (o) =>
                {
                    this.Dispatcher.BeginInvoke(LoadNews);
                });

        }
        void LoadNews()
        {
            WebClient wc = new WebClient();
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(QuestionsDownloadStringCompleted);
            client.DownloadStringAsync(new Uri(rssItem.Url));
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            rssItem = (App.Current as App).News;
            StartLoading();
        }

        private void QuestionsDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Ошибка подключения к базе вопросов. Проверьте соединение с интернетом.");
                NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
                return;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(e.Result);
            HtmlNode news = doc.DocumentNode.SelectSingleNode("//div[@class='b-posts-1-item__text']");
            
            if (news != null)
            {
                string htmlnews = "<html><head>";
                htmlnews += "<meta charset=\"utf-8\"/>";

                //htmlnews += "<meta name=\"viewport\" content=\"width=device-width\" />";
                htmlnews += "</head><body>";
                htmlnews += "<h2>" + rssItem.Title + "</h2>";
                htmlnews += "<img src=\"" + rssItem.ImageUrl + "\" width=\"" + 800 + "\" /><br/>";

                htmlnews += news.InnerHtml;

                HtmlNode footer = doc.DocumentNode.SelectSingleNode("//footer[@class='b-inner-pages-footer-1']");
                if (footer != null)
                {
                    htmlnews += "<p><i>";
                    htmlnews += footer.InnerText;
                    htmlnews += "</i></p>";
                }
                htmlnews += "</body></html>";

                htmlnews = DeleteVideo(htmlnews);
                spNews.NavigateToString(ConvertExtendedAscii(htmlnews));
                
                // stop progress bar
                ShowProgress = false;
                spNews.Visibility = System.Windows.Visibility.Visible;

            }
        }
        string DeleteVideo(string src)
        {
            string res;
            int start = src.IndexOf("<iframe");
            if (start < 0)
                return src;
            string p1 = src.Substring(0, start);

            start = src.IndexOf("</iframe>");
            if (start < 0)
                return src;
            string p2 = src.Substring(start+9);

            res = p1 + "Смотрите видео на сайте." + p2;
            //
            return res;
        }
        public static string ConvertExtendedAscii(string html)
        {
            var retVal = "";
            var s = html.ToCharArray();

            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    retVal += "&#" + Convert.ToInt32(c) + ";";
                else
                    retVal += c;
            }

            return retVal;
        }

        private string GetFirstImg(string src)
        {
            string res = "";
            int start = src.IndexOf("src=");
            if (start < 0)
                return "";
            res = src.Substring(start + 1);

            int length = res.IndexOf("\"");
            if (length < 0)
                return "";
            res = res.Substring(0, length);
            return res;
        }

        private string GetTagP(string src)
        {
            string res = "";
            int start = src.IndexOf("<p");
            if (start < 0)
                return "";
            res = src.Substring(start + 1);

            int length = res.IndexOf("</p>");
            if (length < 0)
                return "";
            res = res.Substring(0, length);
            return res;
        }
    }
}