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
using WindowsPhone.Helpers;

namespace ReadRssItemsSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string WindowsPhoneBlogPosts = "http://windowsteamblog.com/windows_phone/b/wpdev/rss.aspx";

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RssService.GetRssItems(
                WindowsPhoneBlogPosts,
                (items) => { listbox.ItemsSource = items; },
                (exception) => { MessageBox.Show(exception.Message); },
                null
                );
        }
    }
}