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
using System.Data;
using System.Collections.ObjectModel;
using SQLitePCL;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace iTec_uwp
{

    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    /// 
    public sealed partial class RawDataPage : Page
    {
        private ObservableCollection<RolData> AllRowsData;
        private DispatcherTimer Reload_timer;

        #region Class
        public class RolData
        {
            public RolData(string _EventTime,
                           string _Point1, string _Point2, string _Point3,
                           string _Point4, string _Point5, string _Point6,
                           string _Point7, string _Point8)
            {
                this.EventTime = _EventTime;
                this.Point1 = _Point1;
                this.Point2 = _Point2;
                this.Point3 = _Point3;
                this.Point4 = _Point4;
                this.Point5 = _Point5;
                this.Point6 = _Point6;
                this.Point7 = _Point7;
                this.Point8 = _Point8;
            }

            public string EventTime { get; set; }
            public string Point1 { get; set; }
            public string Point2 { get; set; }
            public string Point3 { get; set; }
            public string Point4 { get; set; }
            public string Point5 { get; set; }
            public string Point6 { get; set; }
            public string Point7 { get; set; }
            public string Point8 { get; set; }
        }

        #endregion

        public RawDataPage()
        {
            this.InitializeComponent();

            cbSelect.SelectedIndex = 0;
            RefreshDataView(cbSelect.SelectedIndex);
        }

        #region Methods

        private void RefreshDataView(int sel)
        {
            MyListView.ItemsSource = null;

            List<GV.DATA_INFO_STRING> tmp = GetDataRecord(GV.connection, "SELECT * FROM itec order by Key desc limit 30");

            if (tmp.Count > 0)
            {
                AllRowsData = new ObservableCollection<RolData>();

                for (int i = 0; i < tmp.Count; i++)
                {
                    string _eventTime = string.Format("{0} : {1} : {2} .{3}", tmp[i].EventTime.Substring(8, 2), tmp[i].EventTime.Substring(10, 2), tmp[i].EventTime.Substring(12, 2) , tmp[i].EventTime.Substring(14, 2));

                    #region Selection
                    if (sel == 0)
                    {
                        string[] sPlit = tmp[i].HandleP.ToString().Split(',');

                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            sPlit[3], sPlit[4], sPlit[5],
                                            sPlit[6], sPlit[7]
                            ));
                    }
                    else if (sel == 1)
                    {
                        string[] sPlit = tmp[i].SaddleP.ToString().Split(',');
                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            sPlit[3], sPlit[4], sPlit[5],
                                            sPlit[6], sPlit[7]
                            ));
                    }
                    else if (sel == 2)
                    {
                        string[] sPlit = tmp[i].PedalLeftP.ToString().Split(',');
                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            sPlit[3], "", "", "", ""
                            ));
                    }
                    else if (sel == 3)
                    {
                        string[] sPlit = tmp[i].PedalLeftAcc.ToString().Split(',');
                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            "", "", "", "", ""
                            ));
                    }
                    else if (sel == 4)
                    {
                        string[] sPlit = tmp[i].PeadlLeftGyro.ToString().Split(',');
                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            "", "", "", "", ""
                            ));
                    }
                    else if (sel == 5)
                    {
                        string[] sPlit = tmp[i].PeadlRightP.ToString().Split(',');
                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            sPlit[3], "", "", "", ""
                            ));
                    }
                    else if (sel == 6)
                    {
                        string[] sPlit = tmp[i].PedalRightAcc.ToString().Split(',');
                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            "", "", "", "", ""
                            ));
                    }
                    else if (sel == 7)
                    {
                        string[] sPlit = tmp[i].PeadlRightGyro.ToString().Split(',');
                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            "", "", "", "", ""
                            ));
                    }
                    else if (sel == 8)
                    {
                        string[] sPlit = tmp[i].CrankAcc.ToString().Split(',');
                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            "", "", "", "", ""
                            ));
                    }
                    else if (sel == 9)
                    {
                        string[] sPlit = tmp[i].CrankGyro.ToString().Split(',');
                        AllRowsData.Add(new RolData(
                                _eventTime, sPlit[0], sPlit[1], sPlit[2],
                                            "", "", "", "", ""
                            ));
                    }

                    #endregion
                }
                MyListView.ItemsSource = AllRowsData;
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
                            Key = dbState.GetInteger(0),
                            ID = dbState.GetText(1),
                            EventType = dbState.GetText(2),
                            EventTime = dbState.GetText(3),
                            Cadence = dbState.GetText(4),
                            Power = dbState.GetText(5),
                            Resistance = dbState.GetText(6),
                            HandleP = dbState.GetText(7),
                            SaddleP = dbState.GetText(8),
                            PedalLeftP = dbState.GetText(9),
                            PedalLeftAcc = dbState.GetText(10),
                            PeadlLeftGyro = dbState.GetText(11),
                            PeadlRightP = dbState.GetText(12),
                            PedalRightAcc = dbState.GetText(13),
                            PeadlRightGyro = dbState.GetText(14),
                            CrankAcc = dbState.GetText(15),
                            CrankGyro = dbState.GetText(16)
                        });
                }
            }

            return entries.ToList<GV.DATA_INFO_STRING>();
        }

        #endregion

        #region Controls
        private void cbSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDataView(cbSelect.SelectedIndex);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            GV.IsAction = false;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(QRcodePage));
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshDataView(cbSelect.SelectedIndex);
        }
        #endregion
    }
}
