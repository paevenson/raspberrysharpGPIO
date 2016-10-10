# raspberrysharpGPIO
C# GPIO library for the raspberry pi

using System;
using RaspberrySharpGPIOLib;
using System.Threading;
namespace raspberryPiGpioHelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            RaspberrySharpGPIO.Instance[GPIO.gpio26].Direction = Pin.DIRECTION.OUT;
            RaspberrySharpGPIO.Instance[GPIO.gpio26].Value = 1;
            Thread.Sleep(1000);
            RaspberrySharpGPIO.Instance.CleanUpAllPins();
        }
    }
}
