using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using OnlinerApp.Onliner;
using OnlinerApp.Rss;
using System.Text.RegularExpressions;
using System.Threading;

namespace OnlinerApp.Core
{
    public delegate void NewsLoaded(News news);

    public class News
    {
        #region data

        private RssItem rssItem;
        private string newsPage = "";

        #endregion

        #region public

        public News(RssItem rssItem)
        {
            this.rssItem = rssItem;
            LoadNews();
        }

        public string NewsPage
        {
            get
            {
                return newsPage;
            }
        }

        public event NewsLoaded Loaded;

        #endregion

        void LoadNews()
        {
            WebClient wc = new WebClient();
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(QuestionsDownloadStringCompleted);
            client.DownloadStringAsync(new Uri(rssItem.Url));
        }


        private void QuestionsDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                string Message = "Ошибка подключения. Проверьте соединение с интернетом.";
                throw new NoConnectionException(Message);
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(e.Result);

            this.newsPage = GetNewsPage(doc);

            Loaded(this);
        }
        string GetNewsPage(HtmlDocument doc)
        {
            string html = "<html><head>";
            html += "<meta charset=\"utf-8\"/>";
            html += "</head><body>";

            string news = GetNewsTextHtml(doc);
            html += news;

            if (OnlinerSettings.CommentsOn)
            {
                string comments = GetCommentsHtml(doc);
                html += comments;
            }

            html += "</body></html>";
            return html;
        }
        string GetNewsTextHtml(HtmlDocument doc)
        {
            string html = "";
            HtmlNode news = doc.DocumentNode.SelectSingleNode("//div[@class='b-posts-1-item__text']");

            if (news != null)
            {
                #region news
                html += "<h2>" + rssItem.Title + "</h2>";
                html += "<img src=\"" + rssItem.ImageUrl + "\" width=\"" + 800 + "\" /><br/>";
                html += news.InnerHtml;
                #endregion

                #region footer
                HtmlNode footer = doc.DocumentNode.SelectSingleNode("//footer[@class='b-inner-pages-footer-1']");
                if (footer != null)
                {
                    html += "<p><i>";
                    html += footer.InnerText;
                    html += "</i></p>";
                }
                #endregion
                #region embedded video processing
                html = ProcessVideoLinks(html);
                #endregion
            }
            else
            {
                html += "Не удалось загрузить страницу.";
            }
            html += "</body></html>";
            string converted = ConvertExtendedAscii(html);
            return converted;
        }

        string GetCommentsHtml(HtmlDocument doc)
        {
            string html = "";
            #region comments
            if (OnlinerSettings.CommentsOn)
            {
                HtmlNodeCollection comments = doc.DocumentNode.SelectNodes("//li[@class='b-comments-1__list-item commentListItem']");
                html += "<br/><h2>Комментарии</h2>";
                html += "<ul>";
                foreach (HtmlNode comment in comments)
                {
                    string author = comment.SelectSingleNode("*/strong[@class='author']/a").InnerText;
                    string time = comment.SelectSingleNode("*/span[@class='date']").InnerText;
                    string messageHtml = comment.SelectSingleNode("div[@class='comment-content']").InnerHtml;
                    html += "<li>";
                    html += "<div>";
                    html += "<strong>";
                    html += author;
                    html += "</strong>&emsp;";
                    html += time;
                    html += "<div>";
                    html += messageHtml;
                    html += "</div>";
                    html += "</div>";
                    html += "</li>";
                }
                html += "</ul>";
            }
            #endregion
            string converted = ConvertExtendedAscii(html);
            return converted;
        }


        string ProcessVideoLinks(string htmlnews)
        {
            while (htmlnews.Contains("<iframe"))
            {
                htmlnews = ConvertVideoToUrl(htmlnews);
            }
            return htmlnews;
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
            {
                videoID = youtubeMatch.Groups[1].Value;
                string videoLink = "http://www.youtube.com/watch?v=" + videoID;
                string a = "<a href=\"" + videoLink + "\">[ Смотреть видео ]</a>";
                res = p1 + a + p2;
                return res;
            }   else
            {
                return src;
            }
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

    }
}
