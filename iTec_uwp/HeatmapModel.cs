using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SQLitePCL;

namespace iTec_uwp
{
    public class HeatmapModel : INotifyPropertyChanged, IDisposable
    {
        private const int UpdateInterval = 100;  //更新頻率
        private bool disposed;
        private readonly Timer timer;
        private readonly Stopwatch watch = new Stopwatch();

        #region Init
        double[,] data_PedalLeft_1 = new double [100, 100];
        double[,] data_PedalRight_1 = new double[100, 100];
        double[,] data_Seat_1 = new double [100, 100];

        public PlotModel Seat_1 { get; private set; }
        public PlotModel PedalLeft_1 { get; private set; }
        public PlotModel PedalRight_1 { get; private set; }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.timer.Dispose();
                }
            }

            this.disposed = true;
        }
        #endregion

        public HeatmapModel()
        {
            #region Init
            //坐墊
            Seat_1_Update();
            //左踏板
            PedalLeft_1_Update();
            //右踏板
            PedalRight_1_Update();
            #endregion

            this.timer = new Timer(OnTimerElapsed);
            this.timer.Change(1000, UpdateInterval);

            void OnTimerElapsed(object state)
            {
                lock (Seat_1.SyncRoot) { Seat_1_Update(); }
                lock (PedalLeft_1.SyncRoot) { PedalLeft_1_Update(); }
                lock (PedalRight_1.SyncRoot) { PedalRight_1_Update(); }
            }
        }

        #region Update Methods

        void PedalLeft_1_Update()  //左踏板-熱點計算更新
        {
            this.PedalLeft_1 = new PlotModel { Title = "" };

            PedalLeft_1.PlotAreaBorderColor = OxyColors.Transparent;

            PedalLeft_1.Axes.Add(new OxyPlot.Axes.LinearColorAxis
            {
                Palette = OxyPalettes.Rainbow(500)
            });

            PedalLeft_1.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                IsAxisVisible = false
            });

            PedalLeft_1.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false
            });

            #region  顏色距陣計算

            // generate 1d normal distribution
            var singleDataPedalLeft_1 = new double[100];
            Random rndPedalLeft_1 = new Random();

            for (int x = 0; x < 100; ++x)
            {
                singleDataPedalLeft_1[x] = Math.Exp((-1.0 / 2.0) * Math.Pow(((double)x - 50.0) / Convert.ToDouble(rndPedalLeft_1.Next(900, 1200)), 2.0));
            }

            // generate 2d normal distribution

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; ++y)
                {
                    data_PedalLeft_1[y,x] = singleDataPedalLeft_1[x] * singleDataPedalLeft_1[(y + 50) % 100] * 100;
                }
            }

            #endregion

            var heatMapPedalLeft_1_Series = new HeatMapSeries
            {
                X0 = 0,
                X1 = 99,
                Y0 = 0,
                Y1 = 99,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = data_PedalLeft_1
            };

            this.PedalLeft_1.Series.Add(heatMapPedalLeft_1_Series);

        }
        void PedalRight_1_Update() //右踏板-熱點計算更新
        {
            this.PedalRight_1 = new PlotModel { Title = "" };

            PedalRight_1.PlotAreaBorderColor = OxyColors.Transparent;

            PedalRight_1.Axes.Add(new OxyPlot.Axes.LinearColorAxis
            {
                Palette = OxyPalettes.Rainbow(500)
            });

            PedalRight_1.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                IsAxisVisible = false
            });

            PedalRight_1.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false
            });

            #region  顏色距陣計算

            // generate 1d normal distribution
            var singleDataPedalRight_1 = new double[100];
            Random rndPedalRight_1 = new Random();

            for (int x = 0; x < 100; ++x)
            {
                singleDataPedalRight_1[x] = Math.Exp((-1.0 / 2.0) * Math.Pow(((double)x - 50.0) / Convert.ToDouble(rndPedalRight_1.Next(900, 1200)), 2.0));
            }

            // generate 2d normal distribution

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; ++y)
                {
                    data_PedalRight_1[y, x] = singleDataPedalRight_1[x] * singleDataPedalRight_1[(y + 50) % 100] * 100;
                }
            }
            #endregion

            var heatMapPedalRight_1_Series = new HeatMapSeries
            {
                X0 = 0,
                X1 = 99,
                Y0 = 0,
                Y1 = 99,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = data_PedalRight_1
            };

            this.PedalRight_1.Series.Add(heatMapPedalRight_1_Series);

        } 
        void Seat_1_Update() //座墊-熱點計算更新
        {
            this.Seat_1 = new PlotModel { Title = "" };

            Seat_1.PlotAreaBorderColor = OxyColors.Transparent;

            Seat_1.Axes.Add(new OxyPlot.Axes.LinearColorAxis
            {
                Palette = OxyPalettes.Rainbow(500)
            });

            Seat_1.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                IsAxisVisible = false
            });

            Seat_1.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false
            });

            #region  顏色距陣計算
            // generate 1d normal distribution

            var singleDataSeat_1 = new double[100];
            Random rndSeat_1 = new Random();

            for (int x = 0; x < 100; ++x)
            {
                singleDataSeat_1[x] = Math.Exp((-1.0 / 2.0) * Math.Pow(((double)x - 50.0) / Convert.ToDouble(rndSeat_1.Next(900, 1200)), 2.0));
            }

            // generate 2d normal distribution
            for (int x = 0; x < 100; ++x)
            {
                for (int y = 0; y < 100; ++y)
                {
                    data_Seat_1[y, x] = singleDataSeat_1[x] * singleDataSeat_1[(y + 30) % 100] * 100;
                }
            }

            #endregion

            var heatMapSeat_1_Series = new HeatMapSeries
            {
                X0 = 0,
                X1 = 99,
                Y0 = 0,
                Y1 = 99,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = data_Seat_1
            };

            this.Seat_1.Series.Add(heatMapSeat_1_Series);
        }

        #endregion
    }
}

// 參考 https://github.com/oxyplot/oxyplot/blob/develop/Source/Examples/WPF/WpfExamples/Examples/RealtimeDemo/MainViewModel.cs