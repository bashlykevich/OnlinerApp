using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;

namespace OnlinerApp.Rss
{
    /// <summary>
    /// Provides facade for accessing RSS service
    /// </summary>
    public static class RssService
    {

        /// <summary>
        /// Gets the RSS items.
        /// </summary>
        /// <param name="rssFeed">The RSS feed.</param>
        /// <param name="onGetRssItemsCompleted">The on get RSS items completed.</param>
        /// <param name="onError">The on error.</param>
        public static void GetRssItems(string rssFeed, Action<IEnumerable<RssItem>> onGetRssItemsCompleted = null, Action<Exception> onError = null, Action onFinally = null)
        {
            WebClient webClient = new WebClient();

            // register on download complete event
            webClient.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                try
                {
                    // report error
                    if (e.Error != null)
                    {
                        if (onError != null)
                        {
                            onError(e.Error);
                        }
                        return;
                    }

                    // convert rss result to model
                    List<RssItem> rssItems = new List<RssItem>();
                    Stream stream = e.Result;
                    XmlReader response = XmlReader.Create(stream);
                    SyndicationFeed feeds = SyndicationFeed.Load(response);
                    foreach (SyndicationItem f in feeds.Items)
                    {
                        RssItem rssItem = new RssItem(f.Title.Text, f.Summary.Text, f.PublishDate.ToString(), f.Links[0].Uri.AbsoluteUri);
                        rssItems.Add(rssItem);
                    }

                    // notify completed callback
                    if (onGetRssItemsCompleted != null)
                    {
                        onGetRssItemsCompleted(rssItems);
                    }
                }
                finally
                {
                    // notify finally callback
                    if (onFinally != null)
                    {
                        onFinally();
                    }
                }
            };

            webClient.OpenReadAsync(new Uri(rssFeed));
        }
    }
}