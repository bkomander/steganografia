using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using MessageSteganography;

namespace BitmapMessageReadWriteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[1] read test.bmp, hide message, save to test2.bmp");
            Console.WriteLine("[2] read test.bmp, decode message, read test2.bmp, decode message");

            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.D1)
            {
                Bitmap bmp = new Bitmap("test.bmp");

                Console.WriteLine("MESSAGE in test.bmp: {0}.",
                    MessageSteganography.MessageSteganography.readMessage(bmp));

                string message = Guid.NewGuid().ToString();
                Console.WriteLine("Writing message {0} to test2.bmp.", message);
                MessageSteganography.MessageSteganography.hideMessage(bmp, message);

                bmp.Save("test2.bmp");

                Console.WriteLine("ReadKey();");
                Console.ReadKey();
            }
            else if (key.Key == ConsoleKey.D2)
            {
                Bitmap bmp = new Bitmap("test.bmp");

                Console.WriteLine("MESSAGE in test.bmp: {0}.",
                    MessageSteganography.MessageSteganography.readMessage(bmp));

                Bitmap bmp2 = new Bitmap("test2.bmp");

                Console.WriteLine("MESSAGE in test2.bmp: {0}.",
                    MessageSteganography.MessageSteganography.readMessage(bmp2));

                Console.WriteLine("ReadKey();");
                Console.ReadKey();
            }
        }
    }
}
