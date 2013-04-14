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
using System.Collections.Generic;

namespace OnlinerApp.Onliner
{
    public static class OnlinerSettings
    {
        public static int[] Fonts = {16, 20, 24, 28, 32, 36, 40};
        public static bool PicsInStripOn
        {
            get
            {
                bool PicsInStrip;
                if (AppSettings.TryGetSetting<bool>("PicsInStripOn", out PicsInStrip))
                    return PicsInStrip;
                else
                    return true;
            }
            set
            {
                AppSettings.StoreSetting<bool>("PicsInStripOn", value);
            }
        }

        public static bool CommentsOn
        {
            get
            {
                bool CommentsOn;
                if (AppSettings.TryGetSetting<bool>("CommentsOn", out CommentsOn))
                    return CommentsOn;
                else
                    return true;
            }
            set
            {
                AppSettings.StoreSetting<bool>("CommentsOn", value);
            }
        }

        public static int TitleFontSize
        {
            get
            {
                int TitleFontSize;
                if (AppSettings.TryGetSetting<int>("TitleFontSize", out TitleFontSize))
                    return TitleFontSize;
                else
                    return 24;
            }
            set
            {
                AppSettings.StoreSetting<int>("TitleFontSize", value);
            }
        }

        public static int SummaryFontSize
        {
            get
            {
                int SummaryFontSize;
                if (AppSettings.TryGetSetting<int>("SummaryFontSize", out SummaryFontSize))
                    return SummaryFontSize;
                else
                    return 16;
            }
            set
            {
                AppSettings.StoreSetting<int>("SummaryFontSize", value);
            }
        }

        private static List<OnlinerSection> sections = null;
        
        public static List<OnlinerSection> Sections
        {
            get
            {
                if (sections == null)
                {
                    sections = new List<OnlinerSection>();
                    sections.Add(new OnlinerSection
                    {
                        Header = "техника",
                        FeedUrl = "http://tech.onliner.by/feed"
                    });
                    sections.Add(new OnlinerSection
                    {
                        Header = "авто",
                        FeedUrl = "http://auto.onliner.by/feed"
                    });
                    sections.Add(new OnlinerSection
                    {
                        Header = "деньги",
                        FeedUrl = "http://dengi.onliner.by/feed"
                    });
                    sections.Add(new OnlinerSection
                    {
                        Header = "недвижимость",
                        FeedUrl = "http://realt.onliner.by/feed"
                    });
                }
                return sections;
            }
        }
    }
}
