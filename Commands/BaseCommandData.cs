using System;

namespace kimandtodd.DG200CSharp
{
    public class BaseCommandData : ICommandData
    {
        private static byte[] commandHeader = new byte[] { 0xA0, 0xA2 };
        private static byte[] commandFooter = new byte[] { 0xB0, 0xB3 };
        private static int HEADERLENGTH = commandHeader.Length;
        private static int FOOTERLENGTH = commandFooter.Length;

        protected byte[] getCommandHeader()
        {
            return BaseCommandData.commandHeader;
        }

        protected byte[] getCommandFooter()
        {
            return BaseCommandData.commandFooter;
        }

        protected byte[] buildCommandArray(byte[] command)
        {
            byte[] payloadLength = getPayloadLengthArray(command);
            byte[] _full = new byte[BaseCommandData.HEADERLENGTH + payloadLength.Length + command.Length + DG200CheckSumCalculator.CHECKSUMLENGTH + BaseCommandData.FOOTERLENGTH];
            this.getCommandHeader().CopyTo(_full, 0);
            payloadLength.CopyTo(_full, BaseCommandData.HEADERLENGTH);
            command.CopyTo(_full, BaseCommandData.HEADERLENGTH + payloadLength.Length);
            DG200CheckSumCalculator.CalculateCheckSum(command).CopyTo(_full, BaseCommandData.HEADERLENGTH + payloadLength.Length + command.Length);
            this.getCommandFooter().CopyTo(_full, BaseCommandData.HEADERLENGTH + payloadLength.Length + command.Length + DG200CheckSumCalculator.CHECKSUMLENGTH);
            return _full;
        }

        public byte[] getCommandData()
        {
            return new byte[] { };
        }

        private byte[] getPayloadLengthArray(byte[] payload)
        {
            byte[] payloadLength = new byte[2];

            byte[] convertedLength = BitConverter.GetBytes(payload.Length);

            payloadLength[1] = convertedLength[0];
            payloadLength[0] = convertedLength[1];

            return payloadLength;
        }

        /// <summary>
        /// One of the DG200 command result payloads contains an incorrect payload size. 
        /// This method allows that command to notify the serial processor about that. 
        /// </summary>
        /// <returns>Zero if no override is necessary. Otherwise returns the true payload size.</returns>
        public int getCorrectedExpectedBytes()
        {
            return 0;
        }

        /// <summary>
        /// One of the DG200 commands returns two complete payloads, one after the other.
        /// This method allows that command to notify the serial processor to keep reading 
        /// from the buffer after the first session completes. 
        /// </summary>
        /// <returns>One if no override is necessary. Otherwise returns the true session count.</returns>
        public int getExpectedSessionCount()
        {
            return 1;
        }
    }
}
