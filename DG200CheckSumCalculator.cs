using System;

namespace kimandtodd.DG200CSharp
{
    /// <summary>
    /// Calculates the checksum appended to messages and data payloads that move in between the 
    /// DG200 and the controlling application. 
    /// </summary>
    public class DG200CheckSumCalculator
    {
        private DG200CheckSumCalculator()
        {
            // Only a static method here.
        }

        public readonly static int CHECKSUMLENGTH = 2;

        /// <summary>
        /// Calculates the checksum for either a DG200 command or command response payload. 
        /// </summary>
        /// <param name="payload">An array of bytes from which to do the calculation.</param>
        /// <returns>A two-item array with the calculated checksum result.</returns>
        public static byte[] CalculateCheckSum(byte[] payload)
        {
            int CheckSum;
            int _mask = Convert.ToInt32(Math.Pow(2, 15) - 1);
            CheckSum = payload[0];
            for (UInt32 inx = 1; inx < (payload.Length); inx++)
            {
                CheckSum += payload[inx];
            }

            CheckSum = CheckSum & _mask;

            byte[] retArr = new byte[2];

            byte[] byteArr = BitConverter.GetBytes(CheckSum);

            retArr[0] = byteArr[1];
            retArr[1] = byteArr[0];
            return retArr;
        }
    }
}