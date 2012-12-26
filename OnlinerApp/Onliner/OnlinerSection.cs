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
    public struct OnlinerSection
    {
        public string FeedUrl;
        public string Header;        
        public bool IsEnabled
        {
            get
            {
                bool enabled = true;
                if (AppSettings.TryGetSetting<bool>("раздел_" + Header, out enabled))
                    return enabled;
                else
                    return true;
            }
            set
            {
                AppSettings.StoreSetting<bool>("раздел_" + Header, value);
            }
        }
    }
}
