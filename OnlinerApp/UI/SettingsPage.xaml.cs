using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace OnlinerApp.UI
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            
            #region Comments
            edtComments.Checked += new EventHandler<RoutedEventArgs>(comments_On);
            edtComments.Unchecked += new EventHandler<RoutedEventArgs>(comments_Off);
            bool CommentsOn = true;
            AppSettings.TryGetSetting<bool>("CommentsOn", out CommentsOn);
            edtComments.IsChecked = CommentsOn;            
            this.edtComments.Content = CommentsOn?"Загружать":"Не загружать";
            #endregion

            #region PicsInStrip
            edtPicsInStrip.Checked += new EventHandler<RoutedEventArgs>(picsInStrip_On);
            edtPicsInStrip.Unchecked += new EventHandler<RoutedEventArgs>(picsInStrip_Off);            
            bool PicsInStripOn = true;
            AppSettings.TryGetSetting<bool>("PicsInStripOn", out PicsInStripOn);
            edtPicsInStrip.IsChecked = PicsInStripOn;
            this.edtPicsInStrip.Content = PicsInStripOn ? "Показывать" : "Не показывать";
            #endregion
        }
        void comments_On(object sender, RoutedEventArgs e)
        {
            this.edtComments.Content = "Загружать";
            AppSettings.StoreSetting<bool>("CommentsOn", true);
        }

        void comments_Off(object sender, RoutedEventArgs e)
        {           
            this.edtComments.Content = "Не загружать";
            AppSettings.StoreSetting<bool>("CommentsOn", false);
        }
        void picsInStrip_On(object sender, RoutedEventArgs e)
        {
            this.edtPicsInStrip.Content = "Показывать";
            AppSettings.StoreSetting<bool>("PicsInStripOn", true);
        }

        void picsInStrip_Off(object sender, RoutedEventArgs e)
        {
            this.edtPicsInStrip.Content = "Не показывать";
            AppSettings.StoreSetting<bool>("PicsInStripOn", false);
        }
        private void barBtnBack_Click(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}