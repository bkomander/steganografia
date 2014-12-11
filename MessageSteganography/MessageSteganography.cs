using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MessageSteganography
{
    public static class MessageSteganography
    {
        private static string MAGIC = "{FC6EA6D1-3690-483D-A662-D0BA9D79493E}";

        public static void hideMessage(Bitmap bmp, string message)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            rgbValues = hideMessage(rgbValues, message);

            // Copy the RGB values into the bitamp.
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            bmp.UnlockBits(bmpData);
        }
        public static string readMessage(Bitmap bmp)
        {            
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            bmp.UnlockBits(bmpData);

            return readMessage(rgbValues);

        }

        private static byte[] hideMessage(byte[] data, string message)
        {
            message = MAGIC + message;
            string bitsMessage = GetBits(message) + "00000000";

            if (data.Length < bitsMessage.Length)
            {
                throw new InvalidOperationException("not enough space.");
            }

            byte[] dataOut = (byte[])data.Clone();
            for (int idx = 0; idx < bitsMessage.Length; idx++)
            {
                if (bitsMessage[idx] == '0')
                {
                    dataOut[idx] &= 0xFE;
                }
                else if (bitsMessage[idx] == '1')
                {
                    dataOut[idx] |= 0x01;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return dataOut;
        }

        private static string readMessage(byte[] data)
        {
            string bitsMessage = "";
            int lastZeros = 0;

            for (int idx = 0; idx < data.Length; idx++)
            {
                if ((data[idx] & (byte)0x01) > 0)
                {
                    bitsMessage += "1";
                    lastZeros = 0;
                }
                else
                {
                    bitsMessage += "0";
                    lastZeros++;
                }

                if (lastZeros >= 8)
                {
                    bitsMessage = bitsMessage.Remove(bitsMessage.Length - 8, 8);
                    break;
                }
            }

            //int idxEOF = bitsMessage.IndexOf("00000000");
            //if (idxEOF >= 0)
            //{
            //    bitsMessage = bitsMessage.Remove(idxEOF, bitsMessage.Length - idxEOF);
            //}

            long result;
            Math.DivRem(bitsMessage.Length, 8, out result);
            if (result > 0)
            {
                bitsMessage = bitsMessage.Remove(bitsMessage.Length - (int)result, (int)result);
            }

            byte[] byteString = new byte[bitsMessage.Length / 8];
            for (int idx = 0; idx < byteString.Length; idx++)
            {
                byte b = 0;
                for (int jdx = 0; jdx < 8; jdx++)
                {
                    if (bitsMessage[(idx * 8) + jdx] == '1')
                    {
                        b |= (byte)(0x80 >> jdx);
                    }
                }
                byteString[idx] = b;
            }

            string message = System.Text.Encoding.ASCII.GetString(byteString);
            if (message.StartsWith(MAGIC))
            {
                return message.Remove(0, MAGIC.Length);
            }
            else
            {
                return "";
            }
        }

        private static string GetBits(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in Encoding.ASCII.GetBytes(input))
            {
                string bits = Convert.ToString(b, 2);
                while (bits.Length < 8)
                {
                    bits = "0" + bits;
                }
                sb.Append(bits);
            }
            return sb.ToString();
        }
    }
}
