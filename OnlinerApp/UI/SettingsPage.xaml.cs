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
using OnlinerApp.Onliner;

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
            edtComments.IsChecked = OnlinerSettings.CommentsOn;
            this.edtComments.Content = OnlinerSettings.CommentsOn ? "Загружать" : "Не загружать";
            #endregion

            #region PicsInStrip
            edtPicsInStrip.Checked += new EventHandler<RoutedEventArgs>(picsInStrip_On);
            edtPicsInStrip.Unchecked += new EventHandler<RoutedEventArgs>(picsInStrip_Off);                        
            edtPicsInStrip.IsChecked = OnlinerSettings.PicsInStripOn;
            this.edtPicsInStrip.Content = OnlinerSettings.PicsInStripOn ? "Показывать" : "Не показывать";
            #endregion

            #region sections
            foreach (OnlinerSection s in OnlinerSettings.Sections)
            {
                ToggleSwitch ts = new ToggleSwitch();
                ts.DataContext = s;
                //ts.Name = "";
                //ts.Header = s.Header;
                ts.IsChecked = s.IsEnabled;
                ts.Content = s.Header;
                ts.Checked += new EventHandler<RoutedEventArgs>(section_on);
                ts.Unchecked += new EventHandler<RoutedEventArgs>(section_off);
                spSections.Children.Add(ts);
            }
            #endregion
        }
        
        void section_on(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;
            OnlinerSection s = (OnlinerSection)ts.DataContext;
            s.IsEnabled = true;
        }
        void section_off(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;
            OnlinerSection s = (OnlinerSection)ts.DataContext;
            s.IsEnabled = false;
        }
        void comments_On(object sender, RoutedEventArgs e)
        {
            this.edtComments.Content = "Загружать";
            OnlinerSettings.CommentsOn = true;
        }

        void comments_Off(object sender, RoutedEventArgs e)
        {           
            this.edtComments.Content = "Не загружать";
            OnlinerSettings.CommentsOn = false;
        }
        void picsInStrip_On(object sender, RoutedEventArgs e)
        {
            this.edtPicsInStrip.Content = "Показывать";
            OnlinerSettings.PicsInStripOn = true;
        }

        void picsInStrip_Off(object sender, RoutedEventArgs e)
        {
            this.edtPicsInStrip.Content = "Не показывать";
            OnlinerSettings.PicsInStripOn = false;
        }
        private void barBtnBack_Click(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}