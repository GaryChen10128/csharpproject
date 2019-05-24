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

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace iTec_uwp
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class PressureDisplayPage : Page
    {
        #region Variables
        DispatcherTimer Pressure_timer;

        #region 握把顏色控制
        //-------------------------Right
        private int _GripRightPoint_1;
        public int GripRightPoint_1
        {
            get { return _GripRightPoint_1; }
            set
            {
                _GripRightPoint_1 = value;
                Image img = new Image();

                int iValue = Math.Abs(GV.SensorRefValue[0] - _GripRightPoint_1);
                if (iValue < GV.prvGripsMarginOfError)
                {
                    picGripRight1.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/0.png"));
                    return;
                }

                switch (iValue)
                {
                    case int n when (n >= GV.prvGripsMarginOfError && n < GV.prvGripsMarginOfError + 9):  //  0
                        picGripRight1.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/10.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 10):  //  0 ~ 10
                        picGripRight1.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/30.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 20):  // 10 ~ 20
                        picGripRight1.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/50.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 20 && n < GV.prvGripsMarginOfError + 30):  // 20 ~ 30
                        picGripRight1.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/70.png"));
                        break;
                    case int n when (n > GV.prvGripsMarginOfError + 30):  // 30 ~ 40
                        picGripRight1.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/90.png"));
                        break;
                    default:
                        break;
                }
            }
        }

        private int _GripRightPoint_2;
        public int GripRightPoint_2
        {
            get { return _GripRightPoint_2; }
            set
            {
                _GripRightPoint_2 = value;
                Image img = new Image();

                int iValue = Math.Abs(GV.SensorRefValue[1] - _GripRightPoint_2);

                if (iValue < GV.prvGripsMarginOfError)
                {
                    picGripRight2.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/0.png"));
                    return;
                }

                switch (iValue)
                {
                    case int n when (n >= GV.prvGripsMarginOfError && n < GV.prvGripsMarginOfError + 9):  //  0
                        picGripRight2.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/10.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 10):  //  0 ~ 10
                        picGripRight2.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/30.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 20):  // 10 ~ 20
                        picGripRight2.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/50.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 20 && n < GV.prvGripsMarginOfError + 30):  // 20 ~ 30
                        picGripRight2.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/70.png"));
                        break;
                    case int n when (n > GV.prvGripsMarginOfError + 30):  // 30 ~ 40
                        picGripRight2.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/90.png"));
                        break;
                    default:
                        break;
                }
            }
        }

        private int _GripRightPoint_3;
        public int GripRightPoint_3
        {
            get { return _GripRightPoint_3; }
            set
            {
                _GripRightPoint_3 = value;
                Image img = new Image();

                int iValue = Math.Abs(GV.SensorRefValue[2] - _GripRightPoint_3);
                if (iValue < GV.prvGripsMarginOfError)
                {
                    picGripRight3.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/0.png"));
                    return;
                }

                switch (iValue)
                {
                    case int n when (n >= GV.prvGripsMarginOfError && n < GV.prvGripsMarginOfError + 9):  //  0
                        picGripRight3.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/10.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 10):  //  0 ~ 10
                        picGripRight3.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/30.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 20):  // 10 ~ 20
                        picGripRight3.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/50.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 20 && n < GV.prvGripsMarginOfError + 30):  // 20 ~ 30
                        picGripRight3.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/70.png"));
                        break;
                    case int n when (n > GV.prvGripsMarginOfError + 30):  // 30 ~ 40
                        picGripRight3.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/90.png"));
                        break;
                    default:
                        break;
                }
            }
        }

        private int _GripRightPoint_4;
        public int GripRightPoint_4
        {
            get { return _GripRightPoint_4; }
            set
            {
                _GripRightPoint_4 = value;
                Image img = new Image();

                int iValue = Math.Abs(GV.SensorRefValue[3] - _GripRightPoint_4);
                if (iValue < GV.prvGripsMarginOfError)
                {
                    picGripRight4.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/0_48.png"));
                    return;
                }

                switch (iValue)
                {
                    case int n when (n >= GV.prvGripsMarginOfError && n < GV.prvGripsMarginOfError + 9):  //  0
                        picGripRight4.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/10_48.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 10):  //  0 ~ 10
                        picGripRight4.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/30_48.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 20):  // 10 ~ 20
                        picGripRight4.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/50_48.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 20 && n < GV.prvGripsMarginOfError + 30):  // 20 ~ 30
                        picGripRight4.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/70_48.png"));
                        break;
                    case int n when (n > GV.prvGripsMarginOfError + 30):  // 30 ~ 40
                        picGripRight4.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Right/90_48.png"));
                        break;
                    default:
                        break;
                }
            }
        }
        //-------------------------Left
        private int _GripLeftPoint_5;
        public int GripLeftPoint_5
        {
            get { return _GripLeftPoint_5; }
            set
            {
                _GripLeftPoint_5 = value;
                Image img = new Image();

                int iValue = Math.Abs(GV.SensorRefValue[4] - _GripLeftPoint_5);
                if (iValue < GV.prvGripsMarginOfError)
                {
                    picGripLeft5.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/0.png"));
                    return;
                }

                switch (iValue)
                {
                    case int n when (n >= GV.prvGripsMarginOfError && n < GV.prvGripsMarginOfError + 9):  //  0
                        picGripLeft5.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/10.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 10):  //  0 ~ 10
                        picGripLeft5.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/30.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 20):  // 10 ~ 20
                        picGripLeft5.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/50.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 20 && n < GV.prvGripsMarginOfError + 30):  // 20 ~ 30
                        picGripLeft5.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/70.png"));
                        break;
                    case int n when (n > GV.prvGripsMarginOfError + 30):  // 30 ~ 40
                        picGripLeft5.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/90.png"));
                        break;
                    default:
                        break;
                }
            }
        }

        private int _GripLeftPoint_6;
        public int GripLeftPoint_6
        {
            get { return _GripLeftPoint_6; }
            set
            {
                _GripLeftPoint_6 = value;
                Image img = new Image();

                int iValue = Math.Abs(GV.SensorRefValue[5] - _GripLeftPoint_6);
                if (iValue < GV.prvGripsMarginOfError)
                {
                    picGripLeft6.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/0.png"));
                    return;
                }

                switch (iValue)
                {
                    case int n when (n >= GV.prvGripsMarginOfError && n < GV.prvGripsMarginOfError + 9):  //  0
                        picGripLeft6.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/10.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 10):  //  0 ~ 10
                        picGripLeft6.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/30.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 20):  // 10 ~ 20
                        picGripLeft6.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/50.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 20 && n < GV.prvGripsMarginOfError + 30):  // 20 ~ 30
                        picGripLeft6.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/70.png"));
                        break;
                    case int n when (n > GV.prvGripsMarginOfError + 30):  // 30 ~ 40
                        picGripLeft6.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/90.png"));
                        break;
                    default:
                        break;
                }
            }
        }

        private int _GripLeftPoint_7;
        public int GripLeftPoint_7
        {
            get { return _GripLeftPoint_7; }
            set
            {
                _GripLeftPoint_7 = value;
                Image img = new Image();

                int iValue = Math.Abs(GV.SensorRefValue[6] - _GripLeftPoint_7);
                if (iValue < GV.prvGripsMarginOfError)
                {
                    picGripLeft7.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/0.png"));
                    return;
                }

                switch (iValue)
                {
                    case int n when (n >= GV.prvGripsMarginOfError && n < GV.prvGripsMarginOfError + 9):  //  0
                        picGripLeft7.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/10.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 10):  //  0 ~ 10
                        picGripLeft7.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/30.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 20):  // 10 ~ 20
                        picGripLeft7.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/50.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 20 && n < GV.prvGripsMarginOfError + 30):  // 20 ~ 30
                        picGripLeft7.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/70.png"));
                        break;
                    case int n when (n > GV.prvGripsMarginOfError + 30):  // 30 ~ 40
                        picGripLeft7.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/90.png"));
                        break;
                    default:
                        break;
                }
            }
        }

        private int _GripLeftPoint_8;
        public int GripLeftPoint_8
        {
            get { return _GripLeftPoint_8; }
            set
            {
                _GripLeftPoint_8 = value;
                Image img = new Image();

                int iValue = Math.Abs(GV.SensorRefValue[7] - _GripLeftPoint_8);
                if (iValue < GV.prvGripsMarginOfError)
                {
                    picGripLeft8.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/0_48.png"));
                    return;
                }

                switch (iValue)
                {
                    case int n when (n >= GV.prvGripsMarginOfError && n < GV.prvGripsMarginOfError + 9):  //  0
                        picGripLeft8.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/10_48.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 10):  //  0 ~ 10
                        picGripLeft8.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/30_48.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 10 && n < GV.prvGripsMarginOfError + 20):  // 10 ~ 20
                        picGripLeft8.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/50_48.png"));
                        break;
                    case int n when (n >= GV.prvGripsMarginOfError + 20 && n < GV.prvGripsMarginOfError + 30):  // 20 ~ 30
                        picGripLeft8.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/70_48.png"));
                        break;
                    case int n when (n > GV.prvGripsMarginOfError + 30):  // 30 ~ 40
                        picGripLeft8.Source = new BitmapImage(new Uri(@"ms-appx:///Assets/ColorPercent/Area1-2/Grips/Left/90_48.png"));
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        private int _GripLeft;
        public int GripLeft
        {
            get { return _GripLeft; }
            set
            {
                _GripLeft = value;
                txtGripLeft.Text = string.Format("{0}%", _GripLeft);
            }
        }

        private int _GripRight;
        public int GripRight
        {
            get { return _GripRight; }
            set
            {
                _GripRight = value;
                txtGripRight.Text = string.Format("{0}%", _GripRight);
            }
        }

        private int _Seat;
        public int Seat
        {
            get { return _Seat; }
            set
            {
                _Seat = value;
                txtSeat.Text = string.Format("{0}%", _Seat);
            }
        }

        private int _PedalLeft;
        public int PedalLeft
        {
            get { return _PedalLeft; }
            set
            {
                _PedalLeft = value;
                txtPedalLeft.Text = string.Format("{0}%", _PedalLeft);
            }
        }

        private int _PedalRight;
        public int PedalRight
        {
            get { return _PedalRight; }
            set
            {
                _PedalRight = value;
                txtPedalRight.Text = string.Format("{0}%", _PedalRight);
            }
        }

        #endregion

        public PressureDisplayPage()
        {
            this.InitializeComponent();

            #region Disable focus
            txtGripLeft.AllowFocusOnInteraction = false;
            txtGripRight.AllowFocusOnInteraction = false;
            txtPedalLeft.AllowFocusOnInteraction = false;
            txtPedalRight.AllowFocusOnInteraction = false;
            txtSeat.AllowFocusOnInteraction = false;
            #endregion

            this.DataContext = new HeatmapModel();

            Pressure_timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(200) };
            Pressure_timer.Tick += Pressure_Timer_Tick;
            Pressure_timer.Start();
        }

        #region Timer
        private async void Pressure_Timer_Tick(object sender, object e)
        {
            // 資策會演算法套用位置
            /*
                Class Angle_calculate
                Class Force_calculate
                Class Cycling_Parameter
                Function showBikeParamaer
            */

            Random rnd = new Random();

            #region 文字數值百分比

            /*
            PedalLeft = rnd.Next(0, 100);
            PedalRight = rnd.Next(0,100);
            Seat = rnd.Next(0, 100);
            GripLeft = rnd.Next(0, 100);
            GripRight = rnd.Next(0, 100);
            */

            #endregion

            #region 握把顏色百分比

            //Right
            //GripRightPoint_1 = rnd.Next(1000, 1005);
            //GripRightPoint_2 = rnd.Next(1000, 1005);
            //GripRightPoint_3 = rnd.Next(1000, 1005);
            //GripRightPoint_4 = rnd.Next(1000, 1005);
            GripRightPoint_1 = BitConverter.ToInt16(GV.i2cChannel1.Sensor1, 0);
            GripRightPoint_2 = BitConverter.ToInt16(GV.i2cChannel1.Sensor2, 0);
            GripRightPoint_3 = BitConverter.ToInt16(GV.i2cChannel1.Sensor3, 0);
            GripRightPoint_4 = BitConverter.ToInt16(GV.i2cChannel1.Sensor4, 0);
            //Left
            //GripLeftPoint_5 = rnd.Next(1000, 1005);
            //GripLeftPoint_6 = rnd.Next(1000, 1005);
            //GripLeftPoint_7 = rnd.Next(1000, 1005);
            //GripLeftPoint_8 = rnd.Next(1000, 1005);
            GripLeftPoint_5 = BitConverter.ToInt16(GV.i2cChannel2.Sensor1, 0);
            GripLeftPoint_6 = BitConverter.ToInt16(GV.i2cChannel2.Sensor2, 0);
            GripLeftPoint_7 = BitConverter.ToInt16(GV.i2cChannel2.Sensor3, 0);
            GripLeftPoint_8 = BitConverter.ToInt16(GV.i2cChannel2.Sensor4, 0);
            #endregion

            #region Reload
            pvSeat_1.InvalidatePlot(true);
            pvPedalLeft_1.InvalidatePlot(true);
            pvPedalRight_1.InvalidatePlot(true);
            #endregion
        }

        #endregion

        #region Controls

        private void btnExit_Click(object sender, RoutedEventArgs e)
        { 
            GV.IsAction = false;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(QRcodePage));
        }

        #endregion
    }
}