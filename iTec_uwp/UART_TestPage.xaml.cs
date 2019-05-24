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
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;

using Windows.Devices.SerialCommunication;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace iTec_uwp
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class UART_TestPage : Page
    {
        DataWriter dataWriteObject = null;
        DataReader dataReaderObject = null;

        private ObservableCollection<DeviceInformation> listOfDevices;
        private CancellationTokenSource ReadCancellationTokenSource;

        /* RS232*/

        private async void ListAvailablePorts()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);

                //status.Text = "Select a device and connect";

                for (int i = 0; i < dis.Count; i++)
                {
                    listOfDevices.Add(dis[i]);
                }

               // DeviceListSource.Source = listOfDevices;

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
                    dataReaderObject = new DataReader(GV.iSHAPE.InputStream);
                    dataWriteObject = new DataWriter(GV.iSHAPE.OutputStream);

                    // keep reading the serial input
                    while (true)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (TaskCanceledException tce)
            {
               // status.Text = "Reading task was cancelled, closing device and cleaning up";
                CloseDevice();
            }
            catch (Exception ex)
            {
                //status.Text = ex.Message;
            }
            finally
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
            }
        }

        private async Task WriteAsync(byte[] sendBytes)
        {
            Task<UInt32> storeAsyncTask;

            if (sendBytes.Length > 0)
            {
                // dataWriteObject.WriteString(sendText.Text);

                dataWriteObject.WriteBytes(sendBytes);
                // Launch an async task to complete the write operation
                storeAsyncTask = dataWriteObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await storeAsyncTask;
                if (bytesWritten > 0)
                {
                    //status.Text = sendText.Text + ", ";
                    //status.Text += "bytes written successfully!";
                }
                //sendText.Text = "";
            }
            else
            {
                //status.Text = "Enter the text you want to write and then click on 'WRITE'";
            }
        }


        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 32;

            cancellationToken.ThrowIfCancellationRequested();

            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            //using (var childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            //{
                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask();

                // Launch the task and wait
                UInt32 bytesRead = await loadAsyncTask;
                if (bytesRead > 0)
                {
                    string _text = "";
                    byte[] cc = new byte[bytesRead];

                    dataReaderObject.ReadBytes(cc);

                    for (int i = 0; i < cc.Length; i++)
                        _text += string.Format("{0} ", cc[i].ToString("X2"));
                    txtMsg.Text = _text + "\r\n";

                    //rcvdText.Text = dataReaderObject.ReadString(bytesRead);
                    //status.Text = "bytes read successfully!";
                }
            //}
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

        private async void  UART_Init()
        {

            #region UART

            /*
            string aqs = SerialDevice.GetDeviceSelector("UART0");
           var dis = await DeviceInformation.FindAllAsync(aqs);
           GV.iSHAPE = await SerialDevice.FromIdAsync(dis[0].Id);
           */

            listOfDevices = new ObservableCollection<DeviceInformation>();
            ListAvailablePorts();

            DeviceInformation entry = (DeviceInformation)listOfDevices[0];

            try
            {
                GV.iSHAPE = await SerialDevice.FromIdAsync(entry.Id);
                if (GV.iSHAPE == null) return;

                // Configure serial settings
                GV.iSHAPE.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                GV.iSHAPE.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                GV.iSHAPE.BaudRate = 9600;
                GV.iSHAPE.Parity = SerialParity.None;
                GV.iSHAPE.StopBits = SerialStopBitCount.One;
                GV.iSHAPE.DataBits = 8;
                GV.iSHAPE.Handshake = SerialHandshake.None;

                // Display configured settings
                //status.Text = "Serial port configured successfully: ";
                //status.Text += serialPort.BaudRate + "-";
                //status.Text += serialPort.DataBits + "-";
                //status.Text += serialPort.Parity.ToString() + "-";
                //status.Text += serialPort.StopBits;

                // Set the RcvdText field to invoke the TextChanged callback
                // The callback launches an async Read task to wait for data
                //rcvdText.Text = "Waiting for data...";

                // Create cancellation token object to close I/O operations when closing the device
                ReadCancellationTokenSource = new CancellationTokenSource();

                // Enable 'WRITE' button to allow sending data
                //sendTextButton.IsEnabled = true;

                Listen();
            }
            catch (Exception ex)
            {
                //status.Text = ex.Message;
                //comPortInput.IsEnabled = true;
                //sendTextButton.IsEnabled = false;
            }


            #endregion
        }

        public UART_TestPage()
        {
            this.InitializeComponent();

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            UART_Init();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDevice();
        }

        private async void btnRPM_Click(object sender, RoutedEventArgs e)
        {
           await WriteAsync(new byte[] { 0xF5 ,  0x41 , 0x36 , 0xF6});
        }

        private async void btnPower_Click(object sender, RoutedEventArgs e)
        {
            await WriteAsync(new byte[] { 0xF5, 0x44, 0x39, 0xF6 });
        }

        private async void btnResistance_Click(object sender, RoutedEventArgs e)
        {
            await WriteAsync(new byte[] { 0xF5, 0x49, 0x3E, 0xF6 });
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtMsg.Text = "";
        }

    }
}
