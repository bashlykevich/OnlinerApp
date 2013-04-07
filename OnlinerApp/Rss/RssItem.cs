using System.Net;
using System.Text.RegularExpressions;
using System;

namespace OnlinerApp.Rss
{
    /// <summary>
    /// Model for RSS item
    /// </summary>
    public class RssItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RssItem"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="summary">The summary.</param>
        /// <param name="publishedDate">The published date.</param>
        /// <param name="url">The URL.</param>
        public RssItem(string title, string summary, string publishedDate, string url, string categories)
        {
            Title = title.Replace("&nbsp;", "") ;
            Categories = categories;
            Summary = summary;
            //PublishedDate = publishedDate;
            Url = url;

            DateTime d = DateTime.Parse(publishedDate);
            PublishedDate = d.ToString("HH:mm dd/MM/yyyy");
            // Get plain text from html
            PlainSummary = HttpUtility.HtmlDecode(Regex.Replace(summary, "<[^>]+?>", ""));
            PlainSummary = PlainSummary.Replace("Читать далее…", "");
            
            var match = Regex.Match(summary, "<img(.*?) src=\"(.*?)\"[^>]*>");
            ImageUrl = match.Groups[2].Value; ;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Categories.
        /// </summary>
        /// <value>The title.</value>
        public string Categories { get; set; }

        public string NewsFooter 
        { 
            get
            {
                return PublishedDate + " " + Categories;
            }
        }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the published date.
        /// </summary>
        /// <value>The published date.</value>
        public string PublishedDate { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the Image URL.
        /// </summary>
        /// <value>The Image URL.</value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the plain summary.
        /// </summary>
        /// <value>The plain summary.</value>
        public string PlainSummary { get; set; }
    }
}