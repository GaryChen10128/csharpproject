using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace iTec_uwp
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        int i_pageIndex = 1;

        public HomePage()
        {
            this.InitializeComponent();
            this.pgContent.Navigate(typeof(Sensor1_Page));
        }

        #region Controls
        private void PageChange(int index)
        {
            switch (index)
            {
                case 1:
                    this.pgContent.Navigate(typeof(Sensor2_Page));
                    break;
                case 2:
                    this.pgContent.Navigate(typeof(Sensor1_Page));
                    break;
                case 3:
                    this.pgContent.Navigate(typeof(Sensor3_Page));
                    break;
            }

            btnPres.Visibility = ( index == 1 ? Visibility.Collapsed : Visibility.Visible);
            btnNext.Visibility = (index == 3 ? Visibility.Collapsed : Visibility.Visible);
        }

        private void btnPres_Click(object sender, RoutedEventArgs e)
        {
            i_pageIndex--;

            if (i_pageIndex == 0) i_pageIndex = 1;

            PageChange(i_pageIndex);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            i_pageIndex ++;

            if (i_pageIndex == 3) i_pageIndex = 3;

            PageChange(i_pageIndex);
        }

        #endregion
    }
}
