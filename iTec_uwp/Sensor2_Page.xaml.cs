﻿using System;
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
    public sealed partial class Sensor2_Page : Page
    {
        DispatcherTimer i2c_timer;

        public Sensor2_Page()
        {
            this.InitializeComponent();

            #region Disable focus
            textBox_Copy.AllowFocusOnInteraction = false;
            textBox_Copy1.AllowFocusOnInteraction = false;
            textBox_Copy2.AllowFocusOnInteraction = false;
            textBox_Copy4.AllowFocusOnInteraction = false;
            txtPowerValue.AllowFocusOnInteraction = false;
            txtCadenceValue.AllowFocusOnInteraction = false;
            txtResistanceValue.AllowFocusOnInteraction = false;
            textCadenceValue.AllowFocusOnInteraction = false;
            textBox_Copy8.AllowFocusOnInteraction = false;
            #endregion

            i2c_timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100) };
            i2c_timer.Tick += i2c_Timer_Tick;
            i2c_timer.Start();
        }

        private async void i2c_Timer_Tick(object sender, object e)
        {
            txtPowerValue.Text = string.Format("{0}", Convert.ToInt16(GV.III_HMI.Power));
            rpcPower.Value = GV.III_HMI.Power;
            txtCadenceValue.Text = string.Format("{0}", Convert.ToInt16(GV.III_HMI.Cadence));
            rpcCadence.Value = GV.III_HMI.Cadence;
            txtResistanceValue.Text = string.Format("{0}", Convert.ToInt16(GV.III_HMI.Resistance));
            rpcResistance.Value = GV.III_HMI.Resistance;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            GV.IsAction = false;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(QRcodePage));
        }
    }
}
