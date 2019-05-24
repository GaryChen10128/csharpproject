using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using System.Threading;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.Devices.SerialCommunication;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x404

namespace iTec_uwp
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        #region Variables
        IReadOnlyList<GattCharacteristic> Left_characteristics = null;
        IReadOnlyList<GattCharacteristic> Right_characteristics = null;
        IReadOnlyList<GattCharacteristic> Crank_characteristics = null;
        
        //DataWriter dataWriteObject = null;
        //DataReader dataReaderObject = null;

        private ObservableCollection<DeviceInformation> listOfDevices;
        private CancellationTokenSource ReadCancellationTokenSource;

        private Thread dbThread = null ;
        private Thread i2cThread = null;
        private Thread hmiThread = null;
        private bool dbThreadIsStop = false;
        private bool hmiThreadIsStop = false;
        private bool i2cThreadIsStop = false;

        #region Left
        //Sensor Point
        byte[] PedalLeftP_Sensor1, PedalLeftP_Sensor2, PedalLeftP_Sensor3, PedalLeftP_Sensor4 ;
        //ACC
        byte[] PedalLeft_AccX, PedalLeft_AccY, PedalLeft_AccZ;
        int _L_AccX, _L_AccY, _L_AccZ;
        //Gyro
        byte[] PedalLeft_GyroX, PedalLeft_GyroY, PedalLeft_GyroZ;
        int _L_GyroX, _L_GyroY, _L_GyroZ;
        #endregion

        #region Right
        //Sensor Point
        byte[] PedalRightP_Sensor1, PedalRightP_Sensor2, PedalRightP_Sensor3, PedalRightP_Sensor4;
        //ACC
        byte[] PedalRight_AccX, PedalRight_AccY, PedalRight_AccZ;
        int _R_AccX, _R_AccY, _R_AccZ;
        //Gyro
        byte[] PedalRight_GyroX, PedalRight_GyroY, PedalRight_GyroZ;
        int _R_GyroX, _R_GyroY, _R_GyroZ;
        #endregion

        #region Crank
        //ACC
        byte[] Crank_AccX, Crank_AccY, Crank_AccZ;
        int _Crank_AccX, _Crank_AccY, _Crank_AccZ;

        //Gyro
        byte[] Crank_GyroX, Crank_GyroY, Crank_GyroZ;
        int _Crank_GyroX, _Crank_GyroY, _Crank_GyroZ;
        #endregion

        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            this.pgContent.Navigate(typeof(HomePage));

            if (GV.IsAction) Run();
        }

        #region i2C Config
        private async void FDC2214_Config(I2cDevice i2c) //fdc2214
        {
            //setRCOUNTCH0
            i2c.Write(new byte[] { 0x80, 0xFF, 0xFF });
            //setRCOUNTCH1
            i2c.Write(new byte[] { 0x09, 0xFF, 0xFF });
            //setRCOUNTCH2
            i2c.Write(new byte[] { 0x0A, 0xFF, 0xFF });
            //setRCOUNTCH3
            i2c.Write(new byte[] { 0x0B, 0xFF, 0xFF });

            //setTLECOUNTCH0
            i2c.Write(new byte[] { 0x10, 0x44, 0x00 });
            //setTLECOUNTCH1
            i2c.Write(new byte[] { 0x11, 0x44, 0x00 });
            //setTLECOUNTCH2
            i2c.Write(new byte[] { 0x12, 0x44, 0x00 });
            //setTLECOUNTCH3
            i2c.Write(new byte[] { 0x13, 0x44, 0x00 });

            //setCLOCKDIVIDERSCH0
            i2c.Write(new byte[] { 0x14, 0x10, 0x01 });
            //setCLOCKDIVIDERSCH1
            i2c.Write(new byte[] { 0x15, 0x10, 0x01 });
            //setCLOCKDIVIDERSCH2
            i2c.Write(new byte[] { 0x16, 0x10, 0x01 });
            //setCLOCKDIVIDERSCH3
            i2c.Write(new byte[] { 0x17, 0x10, 0x01 });

            //setERRORCONFIG
            i2c.Write(new byte[] { 0x19, 0x00, 0x00 });

            //setCONFIG
            i2c.Write(new byte[] { 0x1A, 0xD4, 0x81 });
            //setMUXCONFIG
            i2c.Write(new byte[] { 0x1B, 0xC2, 0x0F });
            //setCRESETDEV
            i2c.Write(new byte[] { 0x1C, 0x06, 0x00 });

            //setDRIVECURRENTCH0
            i2c.Write(new byte[] { 0x1E, 0x7C, 0x00 });
            //setDRIVECURRENTCH1
            i2c.Write(new byte[] { 0x1F, 0x7C, 0x00 });
            //setDRIVECURRENTCH2
            i2c.Write(new byte[] { 0x20, 0x7C, 0x00 });
            //setDRIVECURRENTCH3
            i2c.Write(new byte[] { 0x21, 0x7C, 0x00 });

            await Task.Delay(200);
        }

        #endregion

        #region Methods

        private async void Run()
        {
            await StartScenarioAsync();
        }

        private async Task StartScenarioAsync()
        {
            #region UART

             if (GV.iSHAPE == null)
             {
                     listOfDevices = new ObservableCollection<DeviceInformation>();
                     ListAvailablePorts();
                     DeviceInformation entry = (DeviceInformation)listOfDevices[0];

                     try
                     {
                         if (GV.iSHAPE == null)
                         {
                             GV.iSHAPE = await SerialDevice.FromIdAsync(entry.Id);
                         }

                         // Configure serial settings
                         GV.iSHAPE.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                         GV.iSHAPE.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                         GV.iSHAPE.BaudRate = 9600;
                         GV.iSHAPE.Parity = SerialParity.None;
                         GV.iSHAPE.StopBits = SerialStopBitCount.One;
                         GV.iSHAPE.DataBits = 8;
                         GV.iSHAPE.Handshake = SerialHandshake.None;

                         // Create cancellation token object to close I/O operations when closing the device
                         ReadCancellationTokenSource = new CancellationTokenSource();

                         Listen();
                 }
                 catch (Exception ex)
                 {
                 }
             }

            #endregion

            #region BLE

            //ffa3 -> 0x36  , ffa4 -> 03A
            //BLE_Left_HandPedal(0x36);//BLE_Left_HandPedal(0x3A);
            //BLE_Right_HandPedal(0x3A);
            //BLE_Crank(0x3A);

            #endregion

            #region I2C

            string i2cDeviceSelector = I2cDevice.GetDeviceSelector();
            if ( GV.devices == null)
                GV.devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);

            string i2cDeviceSelector2 = I2cDevice.GetDeviceSelector();
            if (GV.i2c_list == null)
                GV.i2c_list= await DeviceInformation.FindAllAsync(i2cDeviceSelector2);

            var tca9548_settings = new I2cConnectionSettings(0x70);
            var fdc2214_settings = new I2cConnectionSettings(0x2A);

            if ( GV.tca9548 == null)
                 GV.tca9548 = await I2cDevice.FromIdAsync(GV.devices[0].Id, tca9548_settings);

            if ( GV.fdc2214 == null)
                 GV.fdc2214 = await I2cDevice.FromIdAsync(GV.i2c_list[0].Id, fdc2214_settings);

            if (GV.tca9548 != null && GV.fdc2214 != null)
            {
                //Channel 1
                GV.tca9548.Write(new byte[] { 0x70, (byte)0x01 });
                FDC2214_Config(GV.fdc2214);

                //Channel 2
                GV.tca9548.Write(new byte[] { 0x70, (byte)0x02 });
                FDC2214_Config(GV.fdc2214);

                //Channel 3
                //GV.tca9548.Write(new byte[] { 0x70, (byte)0x04 });
                //FDC2214_Config(GV.fdc2214);

                //Channel 4
                GV.tca9548.Write(new byte[] { 0x70, (byte)0x08 });
                FDC2214_Config(GV.fdc2214);

                //Channel 5
                GV.tca9548.Write(new byte[] { 0x70, (byte)0x10 });
                FDC2214_Config(GV.fdc2214);

                //Channel 6
                //GV.tca9548.Write(new byte[] { 0x70, (byte)0x20 });
                //FDC2214_Config(GV.fdc2214_ch6);

                //Channel 7
                //GV.tca9548.Write(new byte[] { 0x70, (byte)0x40 });
                //FDC2214_Config(GV.fdc2214_ch7);

                //Channel 8
                //GV.tca9548.Write(new byte[] { 0x70, (byte)0x80 });
                //FDC2214_Config(GV.fdc2214_ch8);

                //Start the polling timer.
                i2cThread = new Thread(new ThreadStart(i2cHandle_Thread));
                i2cThread.Start();
            }

            #endregion

            #region MQTT
            /*
            GV.mqttClient = new MqttClient(GV._MQTT_ServerIP);
            GV.mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            GV.mqttClient.Connect("");
            */
            #endregion

            #region DB
            dbThread = new Thread(new ThreadStart(DbaseHandle_Thread));
            dbThread.IsBackground = true;
            dbThread.Start();
            #endregion

            #region UI
            hmiThread = new Thread(new ThreadStart(hmiThread_Thread));
            dbThread.IsBackground = true;
            hmiThread.Start();
            #endregion

            #region iSHAPE
            GV.iSHAPE_timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
            GV.iSHAPE_timer.Tick += iSHAPE_Timer_Tick;
            GV.iSHAPE_timer.Start();
            #endregion
        }

        private object GetObjectFromBytes(byte[] buffer, Type objType)
        {
            object obj = null;
            if ((buffer != null) && (buffer.Length > 0))
            {
                IntPtr ptrObj = IntPtr.Zero;
                try
                {
                    int objSize = Marshal.SizeOf(objType);
                    if (objSize > 0)
                    {
                        if (buffer.Length < objSize)
                            throw new Exception(String.Format("Buffer smaller than needed for creation of object of type {0}", objType));
                        ptrObj = Marshal.AllocHGlobal(objSize);
                        if (ptrObj != IntPtr.Zero)
                        {
                            Marshal.Copy(buffer, 0, ptrObj, objSize);
                            obj = Marshal.PtrToStructure(ptrObj, objType);
                        }
                        else
                            throw new Exception(String.Format("Couldn't allocate memory to create object of type {0}", objType));
                    }
                }
                finally
                {
                    if (ptrObj != IntPtr.Zero)
                        Marshal.FreeHGlobal(ptrObj);
                }
            }
            return obj;
        }

        public async void BLE_Left_HandPedal(byte AttrHandle)
        {
            var query = BluetoothLEDevice.GetDeviceSelector();

            if (GV.BLE_DeviceList == null) {
                 GV.BLE_DeviceList = await DeviceInformation.FindAllAsync(query);
            }

            int count = GV.BLE_DeviceList.Count();

            if (count > 0)
            {
                var deviceInfo = GV.BLE_DeviceList.Where(x => x.Name == GV.Left_HandPedal_BLE_Name).FirstOrDefault();
                    if (deviceInfo != null)
                    {
                        var bleDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
                        GattDeviceServicesResult deviceServices = await bleDevice.GetGattServicesAsync();
                        if (deviceServices.Status == GattCommunicationStatus.Success)
                        {
                            var services = deviceServices.Services;
                            foreach (var service in services)
                            {
                                if (service.AttributeHandle == 0x2F)
                                {
                                    var accessStatus = await service.RequestAccessAsync();
                                    if (accessStatus == DeviceAccessStatus.Allowed)
                                    {
                                        var characteristic = await service.GetCharacteristicsAsync();

                                        if (characteristic.Status == GattCommunicationStatus.Success)
                                        {
                                            Left_characteristics = characteristic.Characteristics;

                                            foreach (GattCharacteristic gattChar in Left_characteristics)
                                            {
                                                if (gattChar.AttributeHandle == 0x30)
                                                {
                                                    GV.Left_HandPedal_writeCharacteristic = gattChar;
                                                    var writer = new DataWriter();
                                                    writer.WriteByte(0x01);
                                                    GattWriteResult wret = await GV.Left_HandPedal_writeCharacteristic.
                                                                                 WriteValueWithResultAsync(writer.DetachBuffer());
                                                }

                                                if (gattChar.AttributeHandle == AttrHandle)
                                                {
                                                    GV.Left_HandPedal_notifyCharacteristic = gattChar;
                                                }
                                            }

                                            var status = await GV.Left_HandPedal_notifyCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                                                             GattClientCharacteristicConfigurationDescriptorValue.Notify);

                                            if (status == GattCommunicationStatus.Success)
                                                GV.Left_HandPedal_notifyCharacteristic.ValueChanged += Left_HandPedal_NotifyCharacteristic_ValueChanged;
                                        }
                                    }
                                    break;
                                }
                            }
                    }
                }
                else  { }
            }
        }

        public async void BLE_Right_HandPedal(byte AttrHandle)
        {
            var query = BluetoothLEDevice.GetDeviceSelector();

            if (GV.BLE_DeviceList == null)
            {
                GV.BLE_DeviceList = await DeviceInformation.FindAllAsync(query);
            }

            int count = GV.BLE_DeviceList.Count();

            if (count > 0)
            {
                var deviceInfo = GV.BLE_DeviceList.Where(x => x.Name == GV.Right_HandPedal_BLE_Name).FirstOrDefault();
                    if (deviceInfo != null)
                    {
                        var bleDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
                        GattDeviceServicesResult deviceServices = await bleDevice.GetGattServicesAsync();
                        if (deviceServices.Status == GattCommunicationStatus.Success)
                        {
                            var services = deviceServices.Services;
                            foreach (var service in services)
                            {
                                if (service.AttributeHandle == 0x2F)
                                {
                                    var accessStatus = await service.RequestAccessAsync();
                                    if (accessStatus == DeviceAccessStatus.Allowed)
                                    {
                                        var characteristic = await service.GetCharacteristicsAsync();

                                        if (characteristic.Status == GattCommunicationStatus.Success)
                                        {
                                            Right_characteristics = characteristic.Characteristics;

                                            foreach (GattCharacteristic gattChar in Right_characteristics)
                                            {
                                                if (gattChar.AttributeHandle == 0x30)
                                                {
                                                    GV.Right_HandPedal_writeCharacteristic = gattChar;
                                                    var writer = new DataWriter();
                                                    writer.WriteByte(0x01);
                                                    GattWriteResult wret = await GV.Right_HandPedal_writeCharacteristic.
                                                                                 WriteValueWithResultAsync(writer.DetachBuffer());
                                                }

                                                if (gattChar.AttributeHandle == AttrHandle)
                                                {
                                                    GV.Right_HandPedal_notifyCharacteristic = gattChar;
                                                }
                                            }

                                            var status = await GV.Right_HandPedal_notifyCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                                                             GattClientCharacteristicConfigurationDescriptorValue.Notify);

                                            if (status == GattCommunicationStatus.Success)
                                                GV.Right_HandPedal_notifyCharacteristic.ValueChanged += Right_HandPedal_NotifyCharacteristic_ValueChanged;
                                        }
                                    }
                                    break;
                                }
                            }
                    }
                }
                else { }
            }
        }

        public async void BLE_Crank(byte AttrHandle)
        {
            var query = BluetoothLEDevice.GetDeviceSelector();

            if (GV.BLE_DeviceList == null)
            {
                GV.BLE_DeviceList = await DeviceInformation.FindAllAsync(query);
            }

            int count = GV.BLE_DeviceList.Count();

            if (count > 0)
            {
                var deviceInfo = GV.BLE_DeviceList.Where(x => x.Name == GV.Crank_BLE_Name).FirstOrDefault();
                if (deviceInfo != null)
                {
                    var bleDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
                    GattDeviceServicesResult deviceServices = await bleDevice.GetGattServicesAsync();
                        if (deviceServices.Status == GattCommunicationStatus.Success)
                        {
                            var services = deviceServices.Services;
                            foreach (var service in services)
                            {
                                if (service.AttributeHandle == 0x2F)
                                {
                                    var accessStatus = await service.RequestAccessAsync();
                                    if (accessStatus == DeviceAccessStatus.Allowed)
                                    {
                                        var characteristic = await service.GetCharacteristicsAsync();

                                        if (characteristic.Status == GattCommunicationStatus.Success)
                                        {
                                            Crank_characteristics = characteristic.Characteristics;

                                            foreach (GattCharacteristic gattChar in Crank_characteristics)
                                            {
                                                if (gattChar.AttributeHandle == 0x30)
                                                {
                                                    GV.Crank_writeCharacteristic = gattChar;
                                                    var writer = new DataWriter();
                                                    writer.WriteByte(0x01);
                                                    GattWriteResult wret = await GV.Crank_writeCharacteristic.
                                                                                 WriteValueWithResultAsync(writer.DetachBuffer());
                                                }

                                                if (gattChar.AttributeHandle == AttrHandle)                                                {
                                                    GV.Crank_notifyCharacteristic = gattChar;
                                                }
                                            }

                                            var status = await GV.Crank_notifyCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                                                             GattClientCharacteristicConfigurationDescriptorValue.Notify);

                                            if (status == GattCommunicationStatus.Success)
                                                GV.Crank_notifyCharacteristic.ValueChanged += Crank_NotifyCharacteristic_ValueChanged;
                                        }
                                    }
                                    break;
                                }
                            }
                    }
                }
                else { }
            }
        }

        /* UART */

        private async void ListAvailablePorts()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                if ( GV.dic == null)
                    GV.dic = await DeviceInformation.FindAllAsync(aqs);

                for (int i = 0; i < GV.dic.Count; i++)
                    listOfDevices.Add(GV.dic[i]);
            }
            catch (Exception ex)
            {
               // status.Text = ex.Message;
            }
        }

        private async void Listen()
        {
            try
            {
                if (GV.iSHAPE != null)
                {
                    if ( GV.dataReaderObject == null)
                         GV.dataReaderObject = new DataReader(GV.iSHAPE.InputStream);
                    if ( GV.dataWriteObject == null)
                         GV.dataWriteObject = new DataWriter(GV.iSHAPE.OutputStream);

                    // keep reading the serial input
                    while (GV.iSHAPE != null)
                    {
                      //  if (GV.iSHAPE != null)
                            await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (TaskCanceledException tce)
            {
                CloseDevice();
            }
            catch (Exception ex)
            {
                string t = ex.Message;
            }
            finally
            {
                //Cleanup once complete
                if (GV.dataReaderObject != null)
                {
                    GV.dataReaderObject.DetachStream();
                    GV.dataReaderObject = null;
                }
            }
        }

        private async Task WriteAsync(byte[] sendBytes)
        {
            Task<UInt32> storeAsyncTask;

            if (sendBytes.Length > 0)
            {
                //dataWriteObject.WriteString(sendText.Text);
                //dataWriteObject.WriteBytes(sendBytes);
                GV.dataWriteObject.WriteBytes(sendBytes);
                // Launch an async task to complete the write operation
                storeAsyncTask = GV.dataWriteObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await storeAsyncTask;
                if (bytesWritten > 0)
                {
                }
            }
            else
            {
            }
        }


        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Task<UInt32> loadAsyncTask;

                uint ReadBufferLength = 8;

                cancellationToken.ThrowIfCancellationRequested();

                GV.dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

                //using (var childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                //{
              //  if (GV.loadAsyncTask == null)
               // {
                   GV.loadAsyncTask = null;
                   GV.loadAsyncTask = GV.dataReaderObject.LoadAsync(ReadBufferLength).AsTask();// (childCancellationTokenSource.Token);
               // }

                // Launch the task and wait
                UInt32 bytesRead = await GV.loadAsyncTask;
                byte[] tmp = new byte[3];

                if (bytesRead > 0)
                {
                    byte[] iiiSerialValue = new byte[bytesRead];

                    GV.dataReaderObject.ReadBytes(iiiSerialValue);

                    if (iiiSerialValue[0] == 0xF1)
                    {
                        tmp[0] = iiiSerialValue[5];
                        tmp[1] = iiiSerialValue[4];
                        tmp[2] = iiiSerialValue[3];

                        if (iiiSerialValue[1] == 0x41) //RPM
                        {
                            GV.III_HMI.Cadence = Convert.ToInt16(string.Format("{0}{1}{2}", tmp[0] - 48, tmp[1] - 48, tmp[2] - 48));
                        }
                        if (iiiSerialValue[1] == 0x44) //Power
                        {
                            GV.III_HMI.Power = Convert.ToInt16(string.Format("{0}{1}{2}", tmp[0] - 48, tmp[1] - 48, tmp[2] - 48));
                        }
                        if (iiiSerialValue[1] == 0x49) //Resistance
                        {
                            GV.III_HMI.Resistance = Convert.ToInt16(string.Format("{0}{1}{2}", tmp[0] - 48, tmp[1] - 48, tmp[2] - 48)); ;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string t = ex.Message;
            }
        }

        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }

        /// <summary>
        /// CloseDevice:
        /// - Disposes SerialDevice object
        /// - Clears the enumerated device Id list
        /// </summary>
        private void CloseDevice()
        {
            if (GV.iSHAPE != null)
            {
                GV.iSHAPE.Dispose();
            }
            GV.iSHAPE = null;

            listOfDevices.Clear();
        }


        string getJsonformat(string _Id, int _cadence, int _resistance, int _power,
                     int[] handleP, int[] saddleP, int[] peadlLeftP, double[] peadlLeftacc, double[] peadlLeftgyro,
                     int[] peadalRightP, double[] peadlRightacc, double[] peadlRightgyro,
                     double[] crankAcc , double[] crankGyro
                     //// 期美
                     //int _iiiCadence, int _iiiResistance, int _iiiPower, int _magCadence, int _magResistance, int _magPower
                    )
        {
            var jsonStr = new
            {
                ID = _Id,
                cadence = _cadence,
                resistance = _resistance,
                power = _power,
                handleP,
                saddleP,
                peadlLeftP,
                peadlLeftacc,
                peadlLeftgyro,
                peadalRightP,
                peadlRightacc,
                peadlRightgyro,
                crankAcc,
                crankGyro
            };

            return JsonConvert.SerializeObject(jsonStr).ToString();
        }

        #endregion

        #region Events

        private async void Left_HandPedal_NotifyCharacteristic_ValueChanged(GattCharacteristic sender,
            GattValueChangedEventArgs args)
        {
            byte[] bArray;
            bArray = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(bArray);

            if (bArray[1] == 0x02)
                GV.Left_HandPedal = (GV.BLE_DATA)GetObjectFromBytes(bArray, typeof(GV.BLE_DATA));
            if (bArray[1] == 0x01)
                GV.Left_HandPedal_Sensor = (GV.BLE_DATA_SENSOR)GetObjectFromBytes(bArray, typeof(GV.BLE_DATA_SENSOR));

            #region Left
            //Sensor Point
            PedalLeftP_Sensor1 = new byte[] { GV.Left_HandPedal_Sensor.Point15 };
            PedalLeftP_Sensor2 = new byte[] { GV.Left_HandPedal_Sensor.Point16 };
            PedalLeftP_Sensor3 = new byte[] { GV.Left_HandPedal_Sensor.Point17 };
            PedalLeftP_Sensor4 = new byte[] { GV.Left_HandPedal_Sensor.Point18 };

            //ACC
            PedalLeft_AccX = new byte[] { GV.Left_HandPedal.AccX_H, GV.Left_HandPedal.AccX_L };
            PedalLeft_AccY = new byte[] { GV.Left_HandPedal.AccY_H, GV.Left_HandPedal.AccY_L };
            PedalLeft_AccZ = new byte[] { GV.Left_HandPedal.AccZ_H, GV.Left_HandPedal.AccZ_L };
            _L_AccX = BitConverter.ToInt16(PedalLeft_AccX, 0);
            _L_AccY = BitConverter.ToInt16(PedalLeft_AccY, 0);
            _L_AccZ = BitConverter.ToInt16(PedalLeft_AccZ, 0);

            //Gyro
            PedalLeft_GyroX = new byte[] { GV.Left_HandPedal.GyroX_H, GV.Left_HandPedal.GyroX_L };
            PedalLeft_GyroY = new byte[] { GV.Left_HandPedal.GyroY_H, GV.Left_HandPedal.GyroY_L };
            PedalLeft_GyroZ = new byte[] { GV.Left_HandPedal.GyroZ_H, GV.Left_HandPedal.GyroZ_L };
            _L_GyroX = BitConverter.ToInt16(PedalLeft_GyroX, 0);
            _L_GyroY = BitConverter.ToInt16(PedalLeft_GyroY, 0);
            _L_GyroZ = BitConverter.ToInt16(PedalLeft_GyroZ, 0);

            #endregion
        }

        private async void Right_HandPedal_NotifyCharacteristic_ValueChanged(GattCharacteristic sender,
            GattValueChangedEventArgs args)
        {
            byte[] bArray;
            bArray = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(bArray);

            if (bArray[1] == 0x02)
                GV.Right_HandPedal = (GV.BLE_DATA)GetObjectFromBytes(bArray, typeof(GV.BLE_DATA));
            if (bArray[1] == 0x01)
                GV.Right_HandPedal_Sensor = (GV.BLE_DATA_SENSOR)GetObjectFromBytes(bArray, typeof(GV.BLE_DATA_SENSOR));

            #region Right
            //Sensor Point
            PedalRightP_Sensor1 = new byte[] { GV.Right_HandPedal_Sensor.Point15 };
            PedalRightP_Sensor2 = new byte[] { GV.Right_HandPedal_Sensor.Point16 };
            PedalRightP_Sensor3 = new byte[] { GV.Right_HandPedal_Sensor.Point17 };
            PedalRightP_Sensor4 = new byte[] { GV.Right_HandPedal_Sensor.Point18 };

            //ACC
            PedalRight_AccX = new byte[] { GV.Right_HandPedal.AccX_H, GV.Right_HandPedal.AccX_L };
            PedalRight_AccY = new byte[] { GV.Right_HandPedal.AccY_H, GV.Right_HandPedal.AccY_L };
            PedalRight_AccZ = new byte[] { GV.Right_HandPedal.AccZ_H, GV.Right_HandPedal.AccZ_L };
            _R_AccX = BitConverter.ToInt16(PedalRight_AccX, 0);
            _R_AccY = BitConverter.ToInt16(PedalRight_AccY, 0);
            _R_AccZ = BitConverter.ToInt16(PedalRight_AccZ, 0);

            //Gyro
            PedalRight_GyroX = new byte[] { GV.Right_HandPedal.GyroX_H, GV.Right_HandPedal.GyroX_L };
            PedalRight_GyroY = new byte[] { GV.Right_HandPedal.GyroY_H, GV.Right_HandPedal.GyroY_L };
            PedalRight_GyroZ = new byte[] { GV.Right_HandPedal.GyroZ_H, GV.Right_HandPedal.GyroZ_L };
            _R_GyroX = BitConverter.ToInt16(PedalRight_GyroX, 0);
            _R_GyroY = BitConverter.ToInt16(PedalRight_GyroY, 0);
            _R_GyroZ = BitConverter.ToInt16(PedalRight_GyroZ, 0);

            #endregion
        }

        private async void Crank_NotifyCharacteristic_ValueChanged(GattCharacteristic sender,
          GattValueChangedEventArgs args)
        {
            byte[] bArray;
            bArray = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(bArray);
            GV.Crank= (GV.BLE_DATA)GetObjectFromBytes(bArray, typeof(GV.BLE_DATA));

            #region Crank
            //ACC
            Crank_AccX = new byte[] { GV.Crank.AccX_H, GV.Crank.AccX_L };
            Crank_AccY = new byte[] { GV.Crank.AccY_H, GV.Crank.AccY_L };
            Crank_AccZ = new byte[] { GV.Crank.AccZ_H, GV.Crank.AccZ_L };
            _Crank_AccX = BitConverter.ToInt16(Crank_AccX, 0);
            _Crank_AccY = BitConverter.ToInt16(Crank_AccY, 0);
            _Crank_AccZ = BitConverter.ToInt16(Crank_AccZ, 0);

            //Gyro
            Crank_GyroX = new byte[] { GV.Crank.GyroX_H, GV.Crank.GyroX_L };
            Crank_GyroY = new byte[] { GV.Crank.GyroY_H, GV.Crank.GyroY_L };
            Crank_GyroZ = new byte[] { GV.Crank.GyroZ_H, GV.Crank.GyroZ_L };
            _Crank_GyroX = BitConverter.ToInt16(Crank_GyroX, 0);
            _Crank_GyroY = BitConverter.ToInt16(Crank_GyroY, 0);
            _Crank_GyroZ = BitConverter.ToInt16(Crank_GyroZ, 0);

            #endregion

        }

    #endregion

        #region Threads
    public void i2cHandle_Thread()
    {
        for (; ; )
        {
            Thread.Sleep(GV.ScanSensor_Interval);

            GV.prvSensorCurrValue++;

            if (i2cThreadIsStop) break;

            if (i2cThread.IsAlive)
            {
                try
                {
                    if (GV.tca9548 != null && GV.fdc2214 != null)
                    {
                        #region CH1
                        GV.tca9548.Write(new byte[] { 0x70, (byte)0x01 }); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x00 }, GV.i2cChannel1.Sensor1); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x02 }, GV.i2cChannel1.Sensor2);// await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x04 }, GV.i2cChannel1.Sensor3); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x06 }, GV.i2cChannel1.Sensor4); //await Task.Delay(100);
                        Array.Reverse(GV.i2cChannel1.Sensor1);
                        Array.Reverse(GV.i2cChannel1.Sensor2);
                        Array.Reverse(GV.i2cChannel1.Sensor3);
                        Array.Reverse(GV.i2cChannel1.Sensor4);
                        #endregion

                        #region CH2
                        GV.tca9548.Write(new byte[] { 0x70, (byte)0x02 }); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x00 }, GV.i2cChannel2.Sensor1); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x02 }, GV.i2cChannel2.Sensor2); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x04 }, GV.i2cChannel2.Sensor3); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x06 }, GV.i2cChannel2.Sensor4); //await Task.Delay(100);
                        Array.Reverse(GV.i2cChannel2.Sensor1);
                        Array.Reverse(GV.i2cChannel2.Sensor2);
                        Array.Reverse(GV.i2cChannel2.Sensor3);
                        Array.Reverse(GV.i2cChannel2.Sensor4);
                        #endregion

                        #region CH3
                        //GV.tca9548.Write(new byte[] { 0x70, (byte)0x04 }); await Task.Delay(500);
                        //GV.fdc2214.WriteRead(new byte[] { 0x00 }, GV.i2cChannel3.Sensor1); await Task.Delay(100);
                        //GV.fdc2214.WriteRead(new byte[] { 0x02 }, GV.i2cChannel3.Sensor2); await Task.Delay(100);
                        //GV.fdc2214.WriteRead(new byte[] { 0x04 }, GV.i2cChannel3.Sensor3); await Task.Delay(100);
                        //GV.fdc2214.WriteRead(new byte[] { 0x06 }, GV.i2cChannel3.Sensor4); await Task.Delay(100);
                        //Array.Reverse(GV.i2cChannel3.Sensor1);
                        //Array.Reverse(GV.i2cChannel3.Sensor2);
                        //Array.Reverse(GV.i2cChannel3.Sensor3);
                        //Array.Reverse(GV.i2cChannel3.Sensor4);
                        #endregion

                        #region CH4
                        GV.tca9548.Write(new byte[] { 0x70, (byte)0x08 }); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x00 }, GV.i2cChannel4.Sensor1); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x02 }, GV.i2cChannel4.Sensor2);// await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x04 }, GV.i2cChannel4.Sensor3); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x06 }, GV.i2cChannel4.Sensor4);// await Task.Delay(100);
                        Array.Reverse(GV.i2cChannel4.Sensor1);
                        Array.Reverse(GV.i2cChannel4.Sensor2);
                        Array.Reverse(GV.i2cChannel4.Sensor3);
                        Array.Reverse(GV.i2cChannel4.Sensor4);
                        #endregion

                        #region CH5
                        GV.tca9548.Write(new byte[] { 0x70, (byte)0x10 }); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x00 }, GV.i2cChannel5.Sensor1);// await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x02 }, GV.i2cChannel5.Sensor2); //await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x04 }, GV.i2cChannel5.Sensor3);// await Task.Delay(100);
                        GV.fdc2214.WriteRead(new byte[] { 0x06 }, GV.i2cChannel5.Sensor4); //await Task.Delay(100);
                        Array.Reverse(GV.i2cChannel5.Sensor1);
                        Array.Reverse(GV.i2cChannel5.Sensor2);
                        Array.Reverse(GV.i2cChannel5.Sensor3);
                        Array.Reverse(GV.i2cChannel5.Sensor4);
                        #endregion

                        #region 壓力點基準點判斷
                        if (GV.prvSensorOnOff != true && GV.prvSensorCurrValue == GV.SensorRefOption)
                        {
                            //CH1
                            GV.SensorRefValue[0] = BitConverter.ToInt16(GV.i2cChannel1.Sensor1, 0);
                            GV.SensorRefValue[1] = BitConverter.ToInt16(GV.i2cChannel1.Sensor2, 0);
                            GV.SensorRefValue[2] = BitConverter.ToInt16(GV.i2cChannel1.Sensor3, 0);
                            GV.SensorRefValue[3] = BitConverter.ToInt16(GV.i2cChannel1.Sensor4, 0);

                            //CH2
                            GV.SensorRefValue[4] = BitConverter.ToInt16(GV.i2cChannel2.Sensor1, 0);
                            GV.SensorRefValue[5] = BitConverter.ToInt16(GV.i2cChannel2.Sensor2, 0);
                            GV.SensorRefValue[6] = BitConverter.ToInt16(GV.i2cChannel2.Sensor3, 0);
                            GV.SensorRefValue[7] = BitConverter.ToInt16(GV.i2cChannel2.Sensor4, 0);

                            //CH4
                            GV.SensorRefValue[8] = BitConverter.ToInt16(GV.i2cChannel4.Sensor1, 0);
                            GV.SensorRefValue[9] = BitConverter.ToInt16(GV.i2cChannel4.Sensor2, 0);
                            GV.SensorRefValue[10] = BitConverter.ToInt16(GV.i2cChannel4.Sensor3, 0);
                            GV.SensorRefValue[11] = BitConverter.ToInt16(GV.i2cChannel4.Sensor4, 0);

                            //CH5
                            GV.SensorRefValue[12] = BitConverter.ToInt16(GV.i2cChannel5.Sensor1, 0);
                            GV.SensorRefValue[13] = BitConverter.ToInt16(GV.i2cChannel5.Sensor2, 0);
                            GV.SensorRefValue[14] = BitConverter.ToInt16(GV.i2cChannel5.Sensor3, 0);
                            GV.SensorRefValue[15] = BitConverter.ToInt16(GV.i2cChannel5.Sensor4, 0);

                            GV.prvSensorOnOff = true;
                        }
                        #endregion
                    }

                }
                catch (Exception ex)
                {
                }
            }
        }

    }
    public void hmiThread_Thread()
        {
            for (; ; )
            {
                Thread.Sleep(500);

                if (hmiThreadIsStop) break;

                if (hmiThread.IsAlive)
                {

                    // 資策會演算法套用位置
                    /*
                        Class Angle_calculate
                        Class Force_calculate
                        Class Cycling_Parameter
                        Function showBikeParamaer

                    */

                    /* 藍芽己讀入 Data Formagt

                       GV.Left_HandPedal.Header;   // 左踏板
                       GV.Right_HandPedal.Header;  // 右踏板
                       GV.Crank.Header;            // 曲柄

                    */

                    /* Test*/
                    Random rnd = new Random();

                    GV.ITEC_HMI.Power = rnd.Next(0, 1000);
                    GV.ITEC_HMI.Cadence = rnd.Next(0, 1000);
                    GV.ITEC_HMI.Resistance = rnd.Next(0, 100);
                    //GV.III_HMI.Power = rnd.Next(0, 1000);
                    //GV.III_HMI.Cadence = rnd.Next(0, 1000);
                    //GV.III_HMI.Resistance = rnd.Next(0, 100);
                }
            }

        }

        public void DbaseHandle_Thread()
        {
            try
            {
                GV.DATA_INFO dbInfo = null;

                for (; ; )
                {
                    Thread.Sleep(GV.WriteToDB_Interval);

                    if (dbThreadIsStop) break;

                    if (dbThread.IsAlive)
                    {
                        dbInfo = new GV.DATA_INFO()
                        {
                            ID = GV.newUserID,
                            EventType = "none",
                            EventTime = DateTime.Now.ToString("yyyyMMddHHmmssff"),
                            Cadence = GV.III_HMI.Cadence,
                            Power = GV.III_HMI.Power,
                            Resistance = GV.III_HMI.Resistance,
                            HandleP = new int[] { BitConverter.ToInt16(GV.i2cChannel1.Sensor1, 0),
                                          BitConverter.ToInt16(GV.i2cChannel1.Sensor2, 0),
                                          BitConverter.ToInt16(GV.i2cChannel1.Sensor3, 0),
                                          BitConverter.ToInt16(GV.i2cChannel1.Sensor4, 0),
                                          BitConverter.ToInt16(GV.i2cChannel2.Sensor1, 0),
                                          BitConverter.ToInt16(GV.i2cChannel2.Sensor2, 0),
                                          BitConverter.ToInt16(GV.i2cChannel2.Sensor3, 0),
                                          BitConverter.ToInt16(GV.i2cChannel2.Sensor4, 0)
                                        },
                            SaddleP = new int[] { BitConverter.ToInt16(GV.i2cChannel4.Sensor1, 0),
                                          BitConverter.ToInt16(GV.i2cChannel4.Sensor2, 0),
                                          BitConverter.ToInt16(GV.i2cChannel4.Sensor3, 0),
                                          BitConverter.ToInt16(GV.i2cChannel4.Sensor4, 0),
                                          BitConverter.ToInt16(GV.i2cChannel5.Sensor1, 0),
                                          BitConverter.ToInt16(GV.i2cChannel5.Sensor2, 0),
                                          BitConverter.ToInt16(GV.i2cChannel5.Sensor3, 0),
                                          BitConverter.ToInt16(GV.i2cChannel5.Sensor4, 0)
                                        },
                            PedalLeftP = new int[] { GV.Left_HandPedal_Sensor.Point15 ,
                                             GV.Left_HandPedal_Sensor.Point16 ,
                                             GV.Left_HandPedal_Sensor.Point17 ,
                                             GV.Left_HandPedal_Sensor.Point18
                                           },
                            PedalLeftAcc = new int[] { _L_AccX, _L_AccY, _L_AccZ },
                            PeadlLeftGyro = new int[] { _L_GyroX, _L_GyroY, _L_GyroZ },

                            PeadlRightP = new int[] { GV.Right_HandPedal_Sensor.Point15 ,
                                              GV.Right_HandPedal_Sensor.Point16 ,
                                              GV.Right_HandPedal_Sensor.Point17 ,
                                              GV.Right_HandPedal_Sensor.Point18
                                            },
                            PedalRightAcc = new int[] { _R_AccX, _R_AccY, _R_AccZ },
                            PeadlRightGyro = new int[] { _R_GyroX, _R_GyroY, _R_GyroZ },
                            CrankAcc = new int[] { _Crank_AccX, _Crank_AccY, _Crank_AccZ },
                            CrankGyro = new int[] { _Crank_GyroX, _Crank_GyroY, _Crank_GyroZ }
                        };

                        GV.InsertRow(GV.connection, dbInfo);
                    }

                    #region Push to MQTT Server

                    if (GV.mqttClient != null) //2019-02-25 Add 
                    {
                        if (GV.mqttClient.IsConnected)
                        {
                            string cc = getJsonformat("iTec", dbInfo.Cadence, dbInfo.Power, dbInfo.Resistance,
                                            new int[] { dbInfo.HandleP[0], dbInfo.HandleP[1], dbInfo.HandleP[2], dbInfo.HandleP[3], dbInfo.HandleP[4], dbInfo.HandleP[5], dbInfo.HandleP[6], dbInfo.HandleP[7] },
                                            new int[] { dbInfo.SaddleP[0], dbInfo.SaddleP[1], dbInfo.SaddleP[2], dbInfo.SaddleP[3], dbInfo.SaddleP[4], dbInfo.SaddleP[5], dbInfo.SaddleP[6], dbInfo.SaddleP[7] },
                                            new int[] { dbInfo.PedalLeftP[0], dbInfo.PedalLeftP[1], dbInfo.PedalLeftP[2], dbInfo.PedalLeftP[3] },
                                            new double[] { dbInfo.PedalLeftAcc[0], dbInfo.PedalLeftAcc[1], dbInfo.PedalLeftAcc[2] },
                                            new double[] { dbInfo.PeadlLeftGyro[0], dbInfo.PeadlLeftGyro[1], dbInfo.PeadlLeftGyro[2] },
                                            new int[] { dbInfo.PeadlRightP[0], dbInfo.PeadlRightP[1], dbInfo.PeadlRightP[2], dbInfo.PeadlRightP[3] },
                                            new double[] { dbInfo.PedalRightAcc[0], dbInfo.PedalRightAcc[1], dbInfo.PedalRightAcc[2] },
                                            new double[] { dbInfo.PeadlRightGyro[0], dbInfo.PeadlRightGyro[1], dbInfo.PeadlRightGyro[2] },
                                            new double[] { dbInfo.CrankAcc[0], dbInfo.CrankAcc[1], dbInfo.CrankAcc[2] },
                                            new double[] { dbInfo.CrankGyro[0], dbInfo.CrankGyro[1], dbInfo.CrankGyro[2] }
                                        );

                            GV.mqttClient.Publish(GV._MQTT_Topic, Encoding.UTF8.GetBytes(cc), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
            }
        }


        #endregion

        #region Timer
        private async void db_Timer_Tick(object sender, object e)
        {
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //收到的訊息內容以UTF8編碼
            // string ReceivedMessage = Encoding.UTF8.GetString(e.Message);

            // we need this construction because the receiving code in the library and the UI with textbox run on different threads
            //將訊息寫進接收訊息框內，但因為MQTT接收的執行緒與UI執行緒不同，我們需要呼叫自訂的SetText函式做些處理
            // SetText(ReceivedMessage);
        }

        private async void i2c_Timer_Tick(object sender, object e)
        {
        }

        private async void iSHAPE_Timer_Tick(object sender, object e)
        {
            if (GV.iSHAPE == null) return;

            // RPM
            await WriteAsync(new byte[] { 0xF5, 0x41, 0x36, 0xF6 });
            await Task.Delay(100);

            // Power
            await WriteAsync(new byte[] { 0xF5, 0x44, 0x39, 0xF6 });
            await Task.Delay(100);

            // Resistance
            await WriteAsync(new byte[] { 0xF5, 0x49, 0x3E, 0xF6 });
            await Task.Delay(100);
        }

        private async void hmi_Timer_Tick(object sender, object e)
        {
        }

        #endregion

        #region Controls
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            GV.IsAction = false;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(QRcodePage));
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.pgContent.Navigate(typeof(HomePage));
            btnHome.Background = new SolidColorBrush(Windows.UI.Colors.Transparent) ;
            btnHome.Foreground = new SolidColorBrush(Windows.UI.Colors.YellowGreen);
            btnPreDisp.Background = new SolidColorBrush(Windows.UI.Colors.Black);
            btnPreDisp.Foreground = new SolidColorBrush(Windows.UI.Colors.SlateGray);
            btnRawData.Background = new SolidColorBrush(Windows.UI.Colors.Black);
            btnRawData.Foreground = new SolidColorBrush(Windows.UI.Colors.SlateGray);
        }

        private void btnPreDisp_Click(object sender, RoutedEventArgs e)
        {
            this.pgContent.Navigate(typeof(PressureDisplayPage));
            btnHome.Background = new SolidColorBrush(Windows.UI.Colors.Black);
            btnHome.Foreground = new SolidColorBrush(Windows.UI.Colors.SlateGray);
            btnPreDisp.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            btnPreDisp.Foreground = new SolidColorBrush(Windows.UI.Colors.YellowGreen);
            btnRawData.Background = new SolidColorBrush(Windows.UI.Colors.Black);
            btnRawData.Foreground = new SolidColorBrush(Windows.UI.Colors.SlateGray);
        }

        private void btnRawData_Click(object sender, RoutedEventArgs e)
        {
            this.pgContent.Navigate(typeof(RawDataPage));
            btnHome.Background = new SolidColorBrush(Windows.UI.Colors.Black);
            btnHome.Foreground = new SolidColorBrush(Windows.UI.Colors.SlateGray);
            btnPreDisp.Background = new SolidColorBrush(Windows.UI.Colors.Black);
            btnPreDisp.Foreground = new SolidColorBrush(Windows.UI.Colors.SlateGray);
            btnRawData.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            btnRawData.Foreground = new SolidColorBrush(Windows.UI.Colors.YellowGreen);
        }

        #endregion

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            #region BLE
            if (GV.Left_HandPedal_notifyCharacteristic != null)
                GV.Left_HandPedal_notifyCharacteristic.ValueChanged -= Left_HandPedal_NotifyCharacteristic_ValueChanged;

            if (GV.Right_HandPedal_notifyCharacteristic != null)
                GV.Right_HandPedal_notifyCharacteristic.ValueChanged -= Right_HandPedal_NotifyCharacteristic_ValueChanged;

            if (GV.Crank_notifyCharacteristic != null)
                GV.Crank_notifyCharacteristic.ValueChanged -= Crank_NotifyCharacteristic_ValueChanged;
            #endregion

            #region i2c
            if (i2cThread != null)
            {
                i2cThreadIsStop = true;
                Thread.Sleep(100);
                i2cThread.Join();
            }
            #endregion

            #region DB
            if (dbThread != null)
            {
                dbThreadIsStop = true;
                Thread.Sleep(100);
                dbThread.Join();
            }
            #endregion

            #region HMI
            if (hmiThread != null)
            {
                hmiThreadIsStop = true;
                Thread.Sleep(100);
                hmiThread.Join();
            }
            #endregion

            #region iSHAPE
            //if (GV.iSHAPE != null)
            //{
            //    GV.iSHAPE_timer.Stop();
            //  //  await Task.Delay(500);
            //    GV.iSHAPE = null;

            //    ReadCancellationTokenSource = null;
            //    listOfDevices = null;
            //    GV.dataWriteObject = null;
            //    GV.dataReaderObject = null;
            //}
            #endregion
        }
    }
}
