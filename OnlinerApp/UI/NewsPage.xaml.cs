using System;
using System.Net;
using System.Windows;
using HtmlAgilityPack;
using Microsoft.Phone.Controls;
using OnlinerApp.Rss;
using System.Threading;
using System.Text.RegularExpressions;
using OnlinerApp.Onliner;

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
            #region if failed to load
            if (e.Error != null)
            {
                MessageBox.Show("Ошибка подключения к базе вопросов. Проверьте соединение с интернетом.");
                NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
                return;
            }
            #endregion

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(e.Result);
            string htmlnews = FormNewsPage(doc);
            string htmlnewsAscii = ConvertExtendedAscii(htmlnews);
            spNews.NavigateToString(htmlnewsAscii);

            // stop progress bar
            ShowProgress = false;
            spNews.Visibility = System.Windows.Visibility.Visible;

        }
        string ConvertVideoToUrl(string src)
        {
            string res;
            int frameStart = src.IndexOf("<iframe");
            if (frameStart < 0)
                return src;
            string p1 = src.Substring(0, frameStart);

            int frameEnd = src.IndexOf("</iframe>");
            if (frameEnd < 0)
                return src;
            string p2 = src.Substring(frameEnd + 9);

            string iframe = src.Substring(frameStart, frameEnd - frameStart);

            Regex Youtube = new Regex("youtu(?:\\.be|be\\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
            Match youtubeMatch = Youtube.Match(iframe);
            string videoID = string.Empty;

            if (youtubeMatch.Success)

                videoID = youtubeMatch.Groups[1].Value;
            res = p1 + "<a href=\"vnd.youtube:" + videoID + "?vndapp=youtube_mobile&vndclient=mv-google&vndel=watch\">[ Смотреть видео ]</a>" + p2;

            return res;
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
            string p2 = src.Substring(start + 9);

            res = p1 + "[Смотрите видео на сайте]" + p2;
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
        string FormNewsPage(HtmlDocument doc)
        {
            string htmlnews = "<html><head>";
            htmlnews += "<meta charset=\"utf-8\"/>";
            htmlnews += "</head><body>";

            HtmlNode news = doc.DocumentNode.SelectSingleNode("//div[@class='b-posts-1-item__text']");

            if (news != null)
            {
                #region news
                htmlnews += "<h2>" + rssItem.Title + "</h2>";
                htmlnews += "<img src=\"" + rssItem.ImageUrl + "\" width=\"" + 800 + "\" /><br/>";
                htmlnews += news.InnerHtml;
                #endregion

                #region footer
                HtmlNode footer = doc.DocumentNode.SelectSingleNode("//footer[@class='b-inner-pages-footer-1']");
                if (footer != null)
                {
                    htmlnews += "<p><i>";
                    htmlnews += footer.InnerText;
                    htmlnews += "</i></p>";
                }
                #endregion

                #region comments                
                if (OnlinerSettings.CommentsOn)
                {
                    HtmlNodeCollection comments = doc.DocumentNode.SelectNodes("//li[@class='b-comments-1__list-item commentListItem']");
                    htmlnews += "<br/><h2>Комментарии</h2>";
                    htmlnews += "<ul>";
                    foreach (HtmlNode comment in comments)
                    {
                        string author = comment.SelectSingleNode("*/strong[@class='author']/a").InnerText;
                        string time = comment.SelectSingleNode("*/span[@class='date']").InnerText;
                        string messageHtml = comment.SelectSingleNode("div[@class='comment-content']").InnerHtml;
                        htmlnews += "<li>";
                        htmlnews += "<div>";
                        htmlnews += "<strong>";
                        htmlnews += author;
                        htmlnews += "</strong>&emsp;";
                        htmlnews += time;
                        htmlnews += "<div>";
                        htmlnews += messageHtml;
                        htmlnews += "</div>";
                        htmlnews += "</div>";
                        htmlnews += "</li>";
                    }
                    htmlnews += "</ul>";
                }
                #endregion

                #region embedded video processing
                htmlnews = ProcessVideoLinks(htmlnews);
                #endregion
            }
            else
            {
                htmlnews += "Не удалось загрузить страницу.";
            }
            htmlnews += "</body></html>";
            return htmlnews;
        }
        string ProcessVideoLinks(string htmlnews)
        {
            while (htmlnews.Contains("<iframe"))
            {
                htmlnews = ConvertVideoToUrl(htmlnews);
            }
            return htmlnews;
        }
        private void barBtnBack_Click(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}