using System;
using System.IO.Ports;

namespace kimandtodd.DG200CSharp
{
    public class DG200SerialConnection
    {
        // From the GlobalSat spec
        // Baud rate: 230400
        // flow control: no
        // Data bit: 8
        // Stop bit: 1 
        private Int32 _baudRate = 230400;
        private Int32 _dataBits = 8;
        private StopBits _stopBits = StopBits.One;
        private String _portName;
        private SerialPort _prt;

        private Int32 _bytesExpected;
        private Int32 _bytesReceived;
        private bool _expectedBytesCalculated = false;
        private byte[] _expectedBytesCountArray = new byte[2] {0x0,0x0};

        private byte[] _commandStart;
        private byte[] _commandEnd;

        private IDataOutput _outputter;

        private SerialDataReceivedEventHandler SerialDataRecvHandler;


        /// <summary>
        /// Constructor. Requires the name of the port where the DG200 is connected.
        /// </summary>
        /// <param name="portName">Where to find the DG200.</param>
        public DG200SerialConnection(string portName) 
        {
            _portName = portName;
            _commandStart = new byte[2] {0xA0, 0xA2};
            _commandEnd = new byte[2] {0xB0, 0xB3};
        }

        /// <summary>
        /// Provides a class that can be used to output the binary stream from the DG200. 
        /// </summary>
        /// <param name="outputHandler">A handler that can process arrays of bytes.</param>
        public void SetDataOutput(IDataOutput outputHandler) 
        {
            // If we get a new output handler, force any existing to clean up. 
            if (this._outputter != null)
            {
                this._outputter.Finish();
            }

            this._outputter = outputHandler;
        }

        /// <summary>
        /// This is the data handler for listening to a serial device that sends bytes at irregular intervals. 
        /// </summary>
        /// <param name="newHandler">A class that does something...</param>
        public void SetDataHandler(SerialDataReceivedEventHandler newHandler) 
        {
            this.SerialDataRecvHandler = newHandler;
        }

        /// <summary>
        /// Opens a port if it needs to be opened, otherwise returns true if already opened. 
        /// </summary>
        /// <returns>true if the port is open. false, otherwise.</returns>
        private Boolean OpenPort() 
        {
            if (this._prt == null) {
                this._prt = new SerialPort();
                this._prt.PortName = this._portName;
                // If not set, specify the routine that runs when 
                // a DataReceived event occurs at the comPort.
                // If SerialDataRecvHandler Is Nothing Then
                // Me.SetDataHandler(AddressOf DataReceived)
                //End If

                //AddHandler _p.DataReceived, SerialDataRecvHandler

                this.ConfigPort(this._prt);
                this._prt.Open();
            }

            if (this._prt.IsOpen) 
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        /// <summary>
        /// Sends a message to the DG200 over the serial line. 
        /// </summary>
        /// <param name="Msg">An array of bytes to send to the DG200.</param>
        /// <returns>true if the message was sent. false if the port can't be opened.</returns>
        public Boolean SendMessage(byte[] Msg) 
        {
            if( this.OpenPort() ) 
            {
                this.ResetCounters();
                this._prt.Write(Msg, 0, Msg.Length);
                return true;
            }
            else 
            {
                return false;
            }
        }

        /// <summary>
        /// Resets the data counters when receiving the byte stream from the DG 200.
        /// </summary>
        public void ResetCounters() 
        {
            this._bytesExpected = 0;
            this._bytesReceived = 0;
            this._expectedBytesCalculated = false;
            this._expectedBytesCountArray[0] = 0x0;
            this._expectedBytesCountArray[1] = 0x0;
        }

        /// <summary>
        /// Updates the expected and received byte counters for the stream coming from the DG200. 
        /// Note the the size of the array is fixed, but only the bytesRead parameter indicates how many actual bytes came from the DG200.
        /// </summary>
        /// <param name="byteArray">The array of bytes from the DG200.</param>
        /// <param name="bytesRead">The number of bytes read over the serial port. </param>
        private void UpdateCounters(byte[] byteArray, Int32 bytesRead) 
        {
            // If we are just starting, then the expected count is zero.
            // We need to calculate it based on the data within the payload.
            if (!this._expectedBytesCalculated)
            {
                if ((this._bytesReceived + bytesRead) == 3) // it seems like this would break if we already have 3 bytes and we get a read event of zero bytes. 
                {
                    //copy one value to array
                    this._expectedBytesCountArray[0] = byteArray[2 - this._bytesReceived];
                }
                else if (this._bytesReceived == 3 && bytesRead > 0)
                {
                    //copy one value to end of array
                    this._expectedBytesCountArray[1] = byteArray[0];
                    this._expectedBytesCalculated = true;
                }
                else if (this._bytesReceived < 4 && (this._bytesReceived + bytesRead) >= 4)
                {
                    // copy both values to array
                    this._expectedBytesCountArray[0] = byteArray[2 - this._bytesReceived];
                    this._expectedBytesCountArray[1] = byteArray[3 - this._bytesReceived];
                    this._expectedBytesCalculated = true;
                }

                if (this._expectedBytesCalculated)
                {
                    this._bytesExpected = this.calculateExpectedBytes(this._expectedBytesCountArray);
                    Console.WriteLine("calculating expected bytes. calcuation is: " + this._bytesExpected);
                }
                else
                {
                    Console.WriteLine("Got bytes, but not enough to perform calculation.");
                }
            }

            this._bytesReceived += bytesRead;
       }

        /// <summary>
        /// This routine runs when data arrives at the port _p. Not used at the moment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DataReceived(Object sender, SerialDataReceivedEventArgs e ) {

            byte[] newReceivedData = new byte[128];

            // Get data from the COM port.
            Int32 BytesRead = this._prt.Read(newReceivedData, 0, newReceivedData.Length);

            this.UpdateCounters(newReceivedData, BytesRead);

            this._outputter.Output(newReceivedData, BytesRead);
        }

        /// <summary>
        /// Indicates if the expected amount of data has been received. 
        /// </summary>
        /// <returns>true if received is still less than expected.</returns>
        public Boolean isReceiving() 
        {
            return this._bytesReceived < this._bytesExpected;
        }

        /// <summary>
        /// A status message that oughta be removed. 
        /// </summary>
        /// <returns>A string about expected and received byte counts.</returns>
        public String Status() 
        {
            return "Received " + this._bytesReceived  + " bytes of expected " + this._bytesExpected;
        }

        /// <summary>
        /// Reads serial data out of the system buffer. 
        /// </summary>
        /// <returns>true if more data is expected, false if there's nothing left to process.</returns>
        public Boolean Read() 
        {
            byte[] newReceivedData = new byte[128];

            // Get data from the COM port.
            Int32 BytesRead = this._prt.Read(newReceivedData, 0, newReceivedData.Length);

            this.UpdateCounters(newReceivedData, BytesRead);

            this._outputter.Output(newReceivedData, BytesRead);

            if (this._expectedBytesCalculated && this._bytesReceived >= this._bytesExpected) 
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes the port and tells any output handler to finish in whatever way it does. 
        /// </summary>
        public void Close() 
        {
            this._prt.Close();
            this._outputter.Finish();
        }

        /// <summary>
        /// A helper function that turns an array of bytes into a dash-delimited string.
        /// </summary>
        /// <param name="ByteArray">The array of bytes to process.</param>
        /// <param name="BytesToCopy">The number of bytes to include in the output. Set to zero for all of them.</param>
        /// <returns>The dash-delimited string of bytes in hex.</returns>
        public static string ByteArrayToHex(byte[] ByteArray, int BytesToCopy) 
        {
            long l;
            string strRet = "";
            if (BytesToCopy == 0)
            {
                BytesToCopy = ByteArray.Length;
            }

            for (l = ByteArray.GetLowerBound(0); l < ByteArray.GetLowerBound(0) + BytesToCopy; l++) {
                //strRet = strRet & l & ", " & Hex$(ByteArray(l)) & vbCrLf
                strRet = strRet + ByteArray[l].ToString("X") + "-";
            }

            //Remove last space at end.
            return strRet.Substring(0, strRet.Length - 1);
        }

        /// <summary>
        /// Not sure what I meant with this.
        /// </summary>
        /// <param name="PortName"></param>
        /// <returns></returns>
        public Boolean CheckPort(string PortName) 
        {
            SerialPort p = new SerialPort();
            p.PortName = PortName;
            this.ConfigPort(p);

            p.Open();

            p.Close();

            return false;
        }

        /// <summary>
        /// Sets the serial port to match what the DG200 supports. 
        /// </summary>
        /// <param name="_p">The port object to configure.</param>
        private void ConfigPort(SerialPort _p) 
        {
            _p.DataBits = this._dataBits;
            _p.StopBits = this._stopBits;
            _p.BaudRate = this._baudRate;
            //_p.ReceivedBytesThreshold = 8
            //_p.ReadTimeout = 2000
        }

        /// <summary>
        /// Converts an integer into an array of bytes. 
        /// </summary>
        /// <param name="theInt"></param>
        /// <param name="arraysize"></param>
        /// <returns></returns>
        private byte[] CreateByteArrayFromInteger (int theInt, int arraysize) 
        {
            // loop through the array and do bitwise ands on each byte
            // Look at the downloaded Jon Skeet code.
            return System.BitConverter.GetBytes(theInt);
        }

        /// <summary>
        /// Sends a string message over the serial port. 
        /// </summary>
        /// <param name="p1"></param>
        public void SendMessage(string p1) 
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the two-byte, big-endian, payload size value into an x64 little-endian integer. 
        /// </summary>
        /// <param name="payloadSize">The two byte array with the payload value in it.</param>
        /// <returns>The integer of the payload size. Includes eight bytes of padding included in every message.</returns>
        private int calculateExpectedBytes(byte[] payloadSize)
        {
            int total = 0;

            total = payloadSize[0] << 8;
            total += payloadSize[1] & 255;

            // Add eight bytes for 
            total += 8;

            return total;
        }
    }
}
