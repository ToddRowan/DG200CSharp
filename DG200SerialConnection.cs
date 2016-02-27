using System;
using System.IO.Ports;
using kimandtodd.DG200CSharp.commands;

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
        private ConnectionStatus _status;

        private BaseCommand _currentCommand;

        private IDataOutput _outputter;

        public enum ConnectionStatus
        {
            UNKNOWN,
            CONNECTED,
            DISCONNECTED
        };

        /// <summary>
        /// Constructor. Requires the name of the port where the DG200 is connected.
        /// </summary>
        /// <param name="portName">Where to find the DG200.</param>
        public DG200SerialConnection(string portName) 
        {
            _portName = portName;
            _status = ConnectionStatus.UNKNOWN;
            this._outputter = null;
        }

        /// <summary>
        /// Returns the name of the port we've been querying. 
        /// </summary>
        /// <returns>A string with the port name.</returns>
        public String getPortName()
        {
            return this._portName;
        }

        /// <summary>
        /// Outputs a status value. Not entirely reliable atm.
        /// </summary>
        /// <returns>An enum of the current status.</returns>
        public ConnectionStatus getConnectionStatus()
        {
            return this._status;
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
        /// Opens a port if it needs to be opened, otherwise returns true if already opened. 
        /// </summary>
        /// <returns>true if the port is open. false, otherwise.</returns>
        private Boolean OpenPort() 
        {
            if (this._prt == null) 
            {
                this._prt = new SerialPort();
                this._prt.PortName = this._portName;

                this.ConfigPort(this._prt);
            }

            if (!this._prt.IsOpen)
            {
                try
                {
                    this._prt.Open();
                }
                catch (System.IO.IOException ioe) 
                {
                    _status = ConnectionStatus.DISCONNECTED;
                    if (ioe.HResult == -2146232800)
                    {
                        throw new kimandtodd.DG200CSharp.commands.exceptions.CommandException("The specified port was not found.");
                    }
                    else
                    {
                        // This gets thrown if there is a device there but it doesn't like being opened.
                        throw new kimandtodd.DG200CSharp.commands.exceptions.CommandException("The connection attempt failed. Is the device not a DG200?");
                    }
                }
            }

            return this._prt.IsOpen;
        }

        /// <summary>
        /// Executes a command through the serial connection. 
        /// </summary>
        /// <param name="cmd"></param>
        public void Execute(BaseCommand cmd)
        {
            this._currentCommand = cmd;
            this.SendMessage(this._currentCommand.getCommandData());
            while (this.Read())
            {
            }
        }

        /// <summary>
        /// Sends a message to the DG200 over the serial line. 
        /// </summary>
        /// <param name="Msg">An array of bytes to send to the DG200.</param>
        /// <returns>true if the message was sent. false if the port can't be opened.</returns>
        private Boolean SendMessage(byte[] Msg) 
        {
            if( this.OpenPort() ) 
            {
                this._prt.Write(Msg, 0, Msg.Length);
                this._status = ConnectionStatus.CONNECTED; //"Port open.";
                return true;
            }
            else 
            {
                return false;
            }
        }

        /// <summary>
        /// A status message that oughta be removed. 
        /// </summary>
        /// <returns>A string about expected and received byte counts.</returns>
        public String Status() 
        {
            return "";//Received " + this._bytesReceived  + " bytes of expected " + this._bytesExpected;
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
            // Put in the buffer. 
            this._currentCommand.addCommandResultData(newReceivedData, BytesRead);

            // need to change this to pass the data to the command. 
            // TODO: Move this output option to the command. 
            //this._outputter.Output(newReceivedData, BytesRead);

            // Ask the command if it has received enough data.
            return this._currentCommand.continueReading();
        }

        /// <summary>
        /// Closes the port and tells any output handler to finish in whatever way it does. 
        /// </summary>
        public void Close() 
        {
            this._prt.Close();
            //this._outputter.Finish();
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
        //private byte[] CreateByteArrayFromInteger(int theInt, int arraysize) 
        //{
        //    // loop through the array and do bitwise ands on each byte
        //    // Look at the downloaded Jon Skeet code.
        //    return System.BitConverter.GetBytes(theInt);
        //}

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

            for (l = ByteArray.GetLowerBound(0); l < ByteArray.GetLowerBound(0) + BytesToCopy; l++)
            {
                //strRet = strRet & l & ", " & Hex$(ByteArray(l)) & vbCrLf
                strRet = strRet + ByteArray[l].ToString("X") + "-";
            }

            //Remove last space at end.
            return strRet.Substring(0, strRet.Length - 1);
        }
    }
}