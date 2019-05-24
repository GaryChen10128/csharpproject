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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Text;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using QRCoder;
using SQLitePCL;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace iTec_uwp
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class QRcodePage : Page
    {
        public QRcodePage()
        {
            this.InitializeComponent();

            #region Disable focus
            textBox_Copy.AllowFocusOnInteraction = false;
            txtUserID.AllowFocusOnInteraction = false;
            #endregion

            //if (GV.iSHAPE != null)
            //{
            //    GV.iSHAPE = null;

            //}

            //if (GV.i2c_timer != null)
            //{
            //    GV.i2c_timer.Stop();
            //    GV.i2c_timer = null;
            //}

            //if (GV.db_timer != null)
            //{
            //    GV.db_timer.Stop();
            //    GV.db_timer = null;
            //}

			if (GV.mqttClient != null)
			{
				GV.mqttClient.Disconnect();
				GV.mqttClient = null;
			}

            GV.IsAction = false;
            txtUserID.Text = "ID : " + GV.newUserID;
            genQRCodeImage(GV.newUserID);
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //收到的訊息內容以UTF8編碼
            // string ReceivedMessage = Encoding.UTF8.GetString(e.Message);

            // we need this construction because the receiving code in the library and the UI with textbox run on different threads
            //將訊息寫進接收訊息框內，但因為MQTT接收的執行緒與UI執行緒不同，我們需要呼叫自訂的SetText函式做些處理
            // SetText(ReceivedMessage);
        }


        #region Methods

        private void PushData(string UserID)
        {
            List<GV.DATA_INFO_STRING> tmp = GetDataRecord(GV.connection, "SELECT * FROM itec WHERE ID = '" + 
                                                                             UserID + "' order by EventTime asc");
            if (tmp.Count > 0)
            {
            }
        }

        public List<GV.DATA_INFO_STRING> GetDataRecord(SQLiteConnection conn, string sql)
        {
            List<GV.DATA_INFO_STRING> entries = new List<GV.DATA_INFO_STRING>();

            using (ISQLiteStatement dbState = GV.connection.Prepare(sql))
            {
                while (dbState.Step() == SQLiteResult.ROW)
                {
                    entries.Add(
                        new GV.DATA_INFO_STRING
                        {
                            ID = dbState.GetText(0),
                            EventType = dbState.GetText(1),
                            EventTime = dbState.GetText(2),
                            Cadence = dbState.GetText(3),
                            Power = dbState.GetText(4),
                            Resistance = dbState.GetText(5),
                            HandleP = dbState.GetText(6),
                            SaddleP = dbState.GetText(7),
                            PedalLeftP = dbState.GetText(8),
                            PedalLeftAcc = dbState.GetText(9),
                            PeadlLeftGyro = dbState.GetText(10),
                            PeadlRightP = dbState.GetText(11),
                            PedalRightAcc = dbState.GetText(12),
                            PeadlRightGyro = dbState.GetText(13),
                            CrankAcc = dbState.GetText(14),
                            CrankGyro = dbState.GetText(15)
                            //iiiCadence = dbState.GetText(14),
                            //iiiResistance = dbState.GetText(15),
                            //iiiPower = dbState.GetText(16),
                            //magCadence = dbState.GetText(17),
                            //magResistance = dbState.GetText(18),
                            //magPower = dbState.GetText(19)
                        });
                }
            }

            return entries.ToList<GV.DATA_INFO_STRING>();
        }

        async void genQRCodeImage(string ID)
        {
            /* GetGraphic方法参数说明
             * Bitmap qrCodeImage = qrcode.GetGraphic(5,Color.Black,Color.White,null,15,6,false);　
             * 
                            public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Bitmap icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
                        * 
                            int pixelsPerModule:生成二维码图片的像素大小 ，我这里设置的是5 
                        * 
                            Color darkColor：暗色   一般设置为Color.Black 黑色
                        * 
                            Color lightColor:亮色   一般设置为Color.White  白色
                        * 
                            Bitmap icon :二维码 水印图标 例如：Bitmap icon = new Bitmap(context.Server.MapPath("~/images/zs.png")); 默认为NULL ，加上这个二维码中间会显示一个图标
                        * 
                            int iconSizePercent： 水印图标的大小比例 ，可根据自己的喜好设置 
                        * 
                            int iconBorderWidth： 水印图标的边框
                        * 
                            bool drawQuietZones:静止区，位于二维码某一边的空白边界,用来阻止读者获取与正在浏览的二维码无关的信息 即是否绘画二维码的空白边框区域 默认为true
                */

            #region Create QRCode Image
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(ID,  QRCodeGenerator.ECCLevel.L);
            PngByteQRCode qrCodePng = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImagePng = qrCodePng.GetGraphic(20);//, new byte[] { 144, 201, 111 }, new byte[] { 118, 126, 152 });
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(qrCodeImagePng);
                    await writer.StoreAsync();
                }
                var image = new BitmapImage();
                await image.SetSourceAsync(stream);

                myQRCode.Source = image;
            }
        }

        #endregion

        //string getJsonformat(string _Id, int _cadence, int _resistance, int _power,
        //                     int[] handleP, int[] saddleP, int[] peadlLeftP, int[] peadlLeftacc, int[] peadlLeftgyro,
        //                     int[] peadalRightP, int[] peadlRightacc, int[] peadlRightgyro,
        //                     // 期美
        //                     int _iiiCadence, int _iiiResistance, int _iiiPower, int _magCadence, int _magResistance , int _magPower
        //                    )
        //{
        //    var jsonStr = new
        //    {
        //        ID = _Id,
        //        cadence = _cadence,
        //        resistance = _resistance,
        //        power = _power,
        //        handleP,
        //        saddleP,
        //        peadlLeftP,
        //        peadlLeftacc,
        //        peadlLeftgyro,
        //        peadalRightP,
        //        peadlRightacc,
        //        peadlRightgyro,
        //        iiiCadence = _iiiCadence,
        //        iiiResistance = _iiiResistance,
        //        iiiPower = _iiiPower,
        //        magCadence = _magCadence,
        //        magResistance = _magResistance,
        //        magPower = _magPower 
        //    };

        //    return JsonConvert.SerializeObject(jsonStr).ToString();
        //}
        #endregion

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            GV.IsAction = false;
            this.Frame.Navigate(typeof(StartPage));
        }
    }
}
