using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.Devices.I2c;
using Windows.Devices.SerialCommunication;
using SQLitePCL;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.Devices.Radios;

namespace iTec_uwp
{
    public static class GV
    {
        #region Global Structs

        public struct structI2C
        {
            public byte[] Sensor1;
            public byte[] Sensor2;
            public byte[] Sensor3;
            public byte[] Sensor4;
        }

        public class DATA_INFO
        {
            public string ID { get; set; }
            public string EventType { get; set; }
            public string EventTime { get; set; }
            public int Cadence { get; set; }
            public int Power { get; set; }
            public int Resistance { get; set; }
            public int[] HandleP { get; set; }
            public int[] SaddleP { get; set; }
            public int[] PedalLeftP { get; set; }
            public int[] PedalLeftAcc { get; set; }
            public int[] PeadlLeftGyro { get; set; }
            public int[] PeadlRightP { get; set; }
            public int[] PedalRightAcc { get; set; }
            public int[] PeadlRightGyro { get; set; }
            public int[] CrankAcc { get; set; }
            public int[] CrankGyro { get; set; } 
        };

        public struct BLE_DATA
        {
            public byte Header;
            public byte Type;
            public byte AccX_L;
            public byte AccX_H;
            public byte AccY_L;
            public byte AccY_H;
            public byte AccZ_L;
            public byte AccZ_H;
            public byte GyroX_L;
            public byte GyroX_H;
            public byte GyroY_L;
            public byte GyroY_H;
            public byte GyroZ_L;
            public byte GyroZ_H;
            public byte MagX_L;
            public byte MagX_H;
            public byte MagY_L;
            public byte MagY_H;
            public byte MagZ_L;
            public byte MagZ_H;
        };

        public struct BLE_DATA_SENSOR
        {
            public byte Header;
            public byte Type;
            public byte Point1;
            public byte Point2;
            public byte Point3;
            public byte Point4;
            public byte Point5;
            public byte Point6;
            public byte Point7;
            public byte Point8;
            public byte Point9;
            public byte Point10;
            public byte Point11;
            public byte Point12;
            public byte Point13;
            public byte Point14;
            public byte Point15;
            public byte Point16;
            public byte Point17;
            public byte Point18;
        };

        public struct HMI
        {
            public int Cadence;
            public int Power;
            public int Resistance;
        }

        public class DATA_INFO_STRING
        {
            public long Key { get; set; }
            public string ID { get; set; }
            public string EventType { get; set; }
            public string EventTime { get; set; }
            public string Cadence { get; set; }
            public string Power { get; set; }
            public string Resistance { get; set; }
            public string HandleP { get; set; }
            public string SaddleP { get; set; }
            public string PedalLeftP { get; set; }
            public string PedalLeftAcc { get; set; }
            public string PeadlLeftGyro { get; set; }
            public string PeadlRightP { get; set; }
            public string PedalRightAcc { get; set; }
            public string PeadlRightGyro { get; set; }
            public string CrankAcc { get; set; }
            public string CrankGyro { get; set; }
        };

        #endregion

        #region Global Variables
        public static string newUserID = "";
        public static string _dbName = ApplicationData.Current.LocalFolder.Path + "\\IoTPi3DB.db";  //資料庫路徑
        public static string _tableName = "itec";
        public static int _recordIdx = 0;

        public static string _MQTT_ServerIP = "220.130.123.67";
        public static string _MQTT_ServerPort = "1833";
        public static string _MQTT_Topic = "gym/iii_14F";

        public static string DeviceID = "FY001#";           //硬體序號
        public static int WriteToDB_Interval = 20;          //資料寫入時間 sec.
        public static int ScanSensor_Interval = 100;        //i2C 掃描時間 sec.
        public static MqttClient mqttClient;                //MQTT Client

        public static int prvGripsMarginOfError = 3;        //握把壓力值誤差值 (計算判斷 "Sensor 即時壓力值 - Sensor 感應基準值")
        public static int SensorRefOption = 5;              //壓力感應基準值設定 (抓掃瞄後的第 ? 筆)
        public static int prvSensorCurrValue = 0;
        public static bool prvSensorOnOff = false;
        public static int[] SensorRefValue = new int[16];

        #region I2C

        public static I2cDevice tca9548, fdc2214;
        public static DispatcherTimer db_timer;
        public static DispatcherTimer hmi_timer;
        public static DispatcherTimer iSHAPE_timer;
        public static DispatcherTimer i2c_timer;
        public static IReadOnlyList<DeviceInformation> devices;
        public static IReadOnlyList<DeviceInformation> i2c_list;

        #endregion

        #region 藍芽
        public static string Left_HandPedal_BLE_Name = "BLE_LEFT_00";
        public static string Right_HandPedal_BLE_Name = "BLE_RIGHT_00";
        public static string Crank_BLE_Name = "IMU_UP_RIGHT14";

        public static DeviceInformationCollection BLE_DeviceList ;
        public static GattCharacteristic Left_HandPedal_notifyCharacteristic;
        public static GattCharacteristic Left_HandPedal_writeCharacteristic;
        public static GattCharacteristic Left_HandPedal_readCharacteristic;
        public static GattCharacteristic Right_HandPedal_notifyCharacteristic;
        public static GattCharacteristic Right_HandPedal_writeCharacteristic;
        public static GattCharacteristic Right_HandPedal_readCharacteristic;
        public static GattCharacteristic Crank_notifyCharacteristic;
        public static GattCharacteristic Crank_writeCharacteristic;
        public static GattCharacteristic Crank_readCharacteristic;

        public static BLE_DATA Left_HandPedal, Right_HandPedal , Crank;
        public static BLE_DATA_SENSOR Left_HandPedal_Sensor, Right_HandPedal_Sensor;

        #endregion

        public static HMI ITEC_HMI , III_HMI ;                // HMI Display                 
        public static SerialDevice iSHAPE = null;             // 期美 USART      
        public static DeviceInformationCollection dic = null;
        public static Task<UInt32> loadAsyncTask = null;
        public static DataWriter dataWriteObject = null;
        public static DataReader dataReaderObject = null;

        #region Database

        public static SQLiteConnection connection;
        public static bool IsAction = false;

        #endregion 

        public static byte[] command = new byte[1] { 0x0 };
        public static structI2C i2cChannel1 = new structI2C();
        public static structI2C i2cChannel2 = new structI2C();
        public static structI2C i2cChannel3 = new structI2C();
        public static structI2C i2cChannel4 = new structI2C();
        public static structI2C i2cChannel5 = new structI2C();

        #endregion

        #region DB Methods

        public static SQLiteConnection CreateDbConnection()
        {
            connection = new SQLiteConnection(_dbName);
            if (null == connection)
            {
                throw new Exception("create db connection failed");
            }
            return connection;
        }

        public static void CreateTable(SQLiteConnection connection)
        {
            //創建表
            string sql = string.Format(" CREATE TABLE [{0}] ( " +
                                       " [Key] INTEGER PRIMARY KEY AUTOINCREMENT," +
                                       " [ID] nvarchar(32)," +
                                       " [EventType] nvarchar(254)," +
                                       " [EventTime] nvarchar(254), "+
                                       " [Cadence]  nvarchar(254), " +
                                       " [Power]  nvarchar(254)," +
                                       " [Resistance]  nvarchar(254)," +
                                       " [HandleP] nvarchar(254)," +
                                       " [SaddleP] nvarchar(254)," +
                                       " [PedalLeftP] nvarchar(254)," +
                                       " [PedalLeftAcc] nvarchar(254)," +
                                       " [PeadlLeftGyro] nvarchar(254)," +
                                       " [PeadlRightP] nvarchar(254)," +
                                       " [PedalRightAcc] nvarchar(254)," +
                                       " [PeadlRightGyro] nvarchar(254)," +
                                       " [CrankAcc]  nvarchar(254)," +
                                       " [CrankGyro]  nvarchar(254));", _tableName);

            using (ISQLiteStatement sqliteStatement = connection.Prepare(sql))
            {
                sqliteStatement.Step();
            }
        }

        public static void InsertRow(SQLiteConnection connection , DATA_INFO db)
        {
            string _HandleP = string.Join(",", db.HandleP);
            string _SaddleP = string.Join(",", db.SaddleP);
            string _PedalLeftP = string.Join(",", db.PedalLeftP);
            string _PedalLeftAcc = string.Join(",", db.PedalLeftAcc);
            string _PeadlLeftGyro = string.Join(",", db.PeadlLeftGyro);
            string _PeadlRightP = string.Join(",", db.PeadlRightP);
            string _PedalRightAcc = string.Join(",", db.PedalRightAcc);
            string _PeadlRightGyro = string.Join(",", db.PeadlRightGyro);
            string _CrankAcc = string.Join(",", db.CrankAcc);
            string _CrankGyro = string.Join(",", db.CrankGyro);

            string sql = string.Format("INSERT INTO [itec] (" +
                                           "[ID], [EventType], [EventTime], [Cadence], [Power], [Resistance], " +
                                           "[HandleP], [SaddleP], [PedalLeftP], [PedalLeftAcc], [PeadlLeftGyro], " +
                                           "[PeadlRightP], [PedalRightAcc], [PeadlRightGyro], [CrankAcc], " +
                                           "[CrankGyro]) " +
                                       "VALUES (" +
                                            "'{0}','{1}','{2}',{3},{4},{5}," +
                                            "'{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}'," +
                                            "'{14}','{15}')",
                                            db.ID , db.EventType , db.EventTime , db.Cadence , db.Power , db.Resistance,
                                            _HandleP , _SaddleP , _PedalLeftP , _PedalLeftAcc , _PeadlLeftGyro ,
                                            _PeadlRightP , _PedalRightAcc , _PeadlRightGyro ,
                                            _CrankAcc , _CrankGyro
                                      );


            using (ISQLiteStatement sqliteStatement = connection.Prepare(sql))
            {
                sqliteStatement.Step();
            }
        }

        #region Methods
        public static async Task ChangeBluetoothStateAsync(bool bluetoothState)
        {
            try
            {
                var access = await Radio.RequestAccessAsync();
                if (access != Windows.Devices.Radios.RadioAccessStatus.Allowed)
                {
                    return;
                }
                BluetoothAdapter adapter = await BluetoothAdapter.GetDefaultAsync();
                if (null != adapter)
                {
                    var btRadio = await adapter.GetRadioAsync();
                    if (bluetoothState)
                    {
                        await btRadio.SetStateAsync(RadioState.On);
                    }
                    else
                    {
                        await btRadio.SetStateAsync(RadioState.Off);
                    }
                }
                else
                {
                }

            }
            catch
            {
            }
        }

        #endregion 

        #endregion

    }
}