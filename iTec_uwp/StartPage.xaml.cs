using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
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
using SQLitePCL;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace iTec_uwp
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();

            #region DB Init
            if (!File.Exists(GV._dbName))
            {
                GV.CreateDbConnection();
                if (GV.connection != null)
                      GV.CreateTable(GV.connection);
            } else
                GV.connection = new SQLiteConnection(GV._dbName);

            #region 啟動後, 刪除今日以前記錄  (2019-02-21 Add)
            try
            {
                string sql = string.Format("DELETE FROM itec WHERE substr(EventTime,5,2) <= '{0}' AND substr(EventTime,7,2) < '{1}'",
                                            DateTime.Now.ToString("MM"),
                                            DateTime.Now.ToString("dd")
                                          );
                using (ISQLiteStatement dbState = GV.connection.Prepare(sql)) {
                    dbState.Step();
                }
            }
            catch ( Exception ex)
            {
                string err = ex.Message;
            }

            #endregion Add

            #region 測試 - 輸入隨機壓力值 (2019-02-21 Add)
            /*
            Random rndSensor = new Random();
            for (int i = 0; i < 16; i++)
                GV.SensorRefValue[i] = rndSensor.Next(1000, 1005);
            */
            #endregion

            #endregion

            #region Device init

            #region BLE
            BLE_Switch();
            GV.ITEC_HMI = new GV.HMI();
            GV.III_HMI = new GV.HMI();
            GV.Left_HandPedal = new GV.BLE_DATA();
            GV.Right_HandPedal = new GV.BLE_DATA();
            GV.Crank = new GV.BLE_DATA();
            #endregion

            #region Chancel1
                    GV.i2cChannel1.Sensor1 = new byte[2];
                    GV.i2cChannel1.Sensor2 = new byte[2];
                    GV.i2cChannel1.Sensor3 = new byte[2];
                    GV.i2cChannel1.Sensor4 = new byte[2];
                #endregion

                #region Chancel2
                    GV.i2cChannel2.Sensor1 = new byte[2];
                    GV.i2cChannel2.Sensor2 = new byte[2];
                    GV.i2cChannel2.Sensor3 = new byte[2];
                    GV.i2cChannel2.Sensor4 = new byte[2];
                #endregion

                #region Chancel3
                    GV.i2cChannel3.Sensor1 = new byte[2];
                    GV.i2cChannel3.Sensor2 = new byte[2];
                    GV.i2cChannel3.Sensor3 = new byte[2];
                    GV.i2cChannel3.Sensor4 = new byte[2];
                #endregion

                #region Chancel4
                    GV.i2cChannel4.Sensor1 = new byte[2];
                    GV.i2cChannel4.Sensor2 = new byte[2];
                    GV.i2cChannel4.Sensor3 = new byte[2];
                    GV.i2cChannel4.Sensor4 = new byte[2];
                #endregion

                #region Chancel5
                    GV.i2cChannel5.Sensor1 = new byte[2];
                    GV.i2cChannel5.Sensor2 = new byte[2];
                    GV.i2cChannel5.Sensor3 = new byte[2];
                    GV.i2cChannel5.Sensor4 = new byte[2];
                #endregion

            #endregion
        }

        private async void BLE_Switch()
        {
           // await GV.ChangeBluetoothStateAsync(true);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            GV.newUserID = GV.DeviceID + DateTime.Now.ToString("yyyyMMddssff");
            GV.IsAction = true;
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}

//http://www.blendrocks.com/code-blend/2016/1/3/gif-rendering-on-winrt-and-uwp