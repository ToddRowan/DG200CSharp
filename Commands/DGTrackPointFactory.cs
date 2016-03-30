using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{
    /// <summary>
    /// Reads raw binary data from the DG200 and converts it into trackpoint instances 
    /// that can be saved and processed as needed.
    /// </summary>
    public class DGTrackPointFactory
    {
        private UInt16 _formatType;
        private CommandBuffer _buf;
        private int _expectedBytes;
        private IDGTrackPoint _firstTrack;

        private delegate IDGTrackPoint getTrackPointMethod(byte[] bytes);

        private getTrackPointMethod getTrackPoint;

        /// <summary>
        /// Constructor. Initializes the internal settings to produce the correct track type for all records. 
        /// </summary>
        /// <param name="buffer">Buffer of data to process.</param>
        public DGTrackPointFactory(CommandBuffer buffer)
        {
            this._buf = buffer;

            // Read the first track to find out what the format is for the rest of them.
            this._buf.Position = BaseCommandResult.PAYLOAD_START;
            byte[] firstTrackArr = new byte[32];
            this._buf.Read(firstTrackArr, 0, firstTrackArr.Length);

            this._firstTrack = new DGFirstTrackPoint(firstTrackArr);

            // The byte size of the remaining records is based on what's found in the first one. 
            this._formatType = this._firstTrack.getTrackFormat();
            this._expectedBytes = this._formatType == BaseDGTrackPoint.FORMAT_POSITION_DATE_SPEED_ALTITUDE ? 32 : 20;

            switch (this._formatType)
            {
                case 0:
                    this.getTrackPoint = this.getPositionOnlyTrackPoint;
                    break;

                case 1:
                    this.getTrackPoint = this.getDateTimeTrackPoint;
                    break;

                default:
                    this.getTrackPoint = this.getAltitudeTrackPoint;
                    break;
            }
        }

        private IDGTrackPoint getPositionOnlyTrackPoint(byte[] bytes)
        {
            return new DGPositionTrackPoint(bytes);
        }

        private IDGTrackPoint getDateTimeTrackPoint(byte[] bytes)
        {
            return new DGPositionDateTimeTrackPoint(bytes);
        }

        private IDGTrackPoint getAltitudeTrackPoint(byte[] bytes)
        {
            return new DGPositionDateTimeAltitudeTrackPoint(bytes);
        }

        /// <summary>
        /// Ask the factory about the format type for all items in this track.
        /// </summary>
        /// <returns>The format type. 0 for position, 1 to add date/time &amp; speed, 3 adds altitude. </returns>
        public UInt16 getFormatType()
        {
            return this._formatType;
        }

        /// <summary>
        /// Generates the track points from the internal buffer. 
        /// </summary>
        /// <returns>Yields track points that are stored.</returns>
        public IEnumerable<IDGTrackPoint> getTrackPoints()
        {
            // Send back the first one.
            yield return this._firstTrack;

            byte[] readArr = new byte[this._expectedBytes];

            this._buf.Read(readArr, 0, readArr.Length);

            // Then move through the buffer and start building tracks.
            while (!this.endingBytes(readArr))
            {
                yield return this.getTrackPoint(readArr);
                this._buf.Read(readArr, 0, readArr.Length);
            }

            DG200FileLogger.Log("DGTrackPointFactory: Completed processing.", 3);
        }

        // The ending sequence is six straight bytes of 0xff
        private bool endingBytes(byte[] bytes)
        {
            bool retVar = true;

            for (int x = 0; x < 6; x++ )
            {
                if (bytes[x] != 0xff)
                {
                    retVar = false;
                }
            }
            return retVar;
        }


    }
}
