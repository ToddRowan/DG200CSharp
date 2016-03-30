using System;

namespace kimandtodd.DG200CSharp
{
    public class DG200Utils
    {
        private DG200Utils()
        {

        }

        /// <summary>
        /// Turns a four-byte array into an integer. Assumes big-endian from the device. 
        /// </summary>
        /// <param name="arr">An ArraySegment with the marked bytes for conversion.</param>
        /// <returns>An integer converted from the byte data</returns>
        public static int bigEndianArrayToInt32(ArraySegment<byte> arr)
        {
            int size = 4, countUp = 0, countDown = arr.Offset + size - 1;
            byte[] tmpArr = new byte[size];
            while (countUp < size)
            {
                tmpArr[countUp++] = arr.Array[countDown--];
            }

            return BitConverter.ToInt32(tmpArr, 0);
        }

        /// <summary>
        /// Turns an integer into a four-byte array. Assumes big-endian for the device. 
        /// </summary>
        /// <param name="src">An integer to convert.</param>
        /// <returns>The byte data</returns>
        public static byte[] int32ToBigEndianArray(int src)
        {
            int size = 4, countUp = 0, countDown = 3;
            byte[] tmpArr = BitConverter.GetBytes(src);
            byte[] arr = new byte[4];
            while (countUp < size)
            {
                arr[countUp++] = tmpArr[countDown--];
            }

            return arr;
        }

        /// <summary>
        /// Turns a two-byte array into an integer. Assumes big-endian from the device. 
        /// </summary>
        /// <param name="arr">An ArraySegment with the marked bytes for conversion.</param>
        /// <returns>A two-byte integer converted from the byte data</returns>
        public static Int16 bigEndianArrayToInt16(ArraySegment<byte> arr)
        {
            int size = 2, countUp = 0, countDown = arr.Offset + size - 1;
            byte[] tmpArr = new byte[size];
            while (countUp < size)
            {
                tmpArr[countUp++] = arr.Array[countDown--];
            }

            return BitConverter.ToInt16(tmpArr, 0);
        }

        /// <summary>
        /// Turns a two-byte array into an integer. Assumes big-endian from the device. 
        /// </summary>
        /// <param name="arr">A four-byte array.</param>
        /// <returns>A four-byte integer converted from the byte data</returns>
        public static Int32 bigEndianArrayToInt32(byte[] arr)
        {
            int size = 4, countUp = 0, countDown = 3;
            byte[] tmpArr = new byte[size];
            while (countUp < size)
            {
                tmpArr[countUp++] = arr[countDown--];
            }

            return BitConverter.ToInt32(tmpArr, 0);
        }
    }
}
