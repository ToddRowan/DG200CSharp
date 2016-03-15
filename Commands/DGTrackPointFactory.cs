using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{
    public class DGTrackPointFactory
    {
        private UInt16 _formatType;
        private CommandBuffer _buf;
        private int _expectedBytes;
        private IDGTrackPoint _firstTrack;

        private delegate IDGTrackPoint getTrackPointMethod(byte[] bytes);

        private getTrackPointMethod getTrackPoint;

        public DGTrackPointFactory(CommandBuffer buffer)
        {
            this._buf = buffer;

            // Read the first track to find out what the format is for the rest of them.
            this._buf.Position = BaseCommandResult.PAYLOAD_START;
            byte[] firstTrackArr = new byte[32];
            this._buf.Read(firstTrackArr, 0, firstTrackArr.Length);

            this._firstTrack = new DGFirstTrackPoint(firstTrackArr);

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
                    this.getTrackPoint = getAltitudeTrackPoint;
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

        public UInt16 getFormatType()
        {
            return this._formatType;
        }

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
