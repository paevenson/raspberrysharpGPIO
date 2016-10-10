using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberrySharpGPIOLib
{
    //map the pin# to its gpio#
    public enum PIN { pin3 = GPIO.gpio2, pin5 = GPIO.gpio3, pin7 = GPIO.gpio4, pin8 = GPIO.gpio14, pin10 = GPIO.gpio15, pin11 = GPIO.gpio17, pin12 = GPIO.gpio18, pin13 = 27, pin15 = GPIO.gpio22, pin16 = GPIO.gpio23, pin18 = GPIO.gpio24, pin19 = GPIO.gpio10, pin21 = GPIO.gpio9, pin22 = GPIO.gpio25, pin23 = GPIO.gpio11, pin24 = GPIO.gpio8, pin26 = GPIO.gpio7, pin29 = GPIO.gpio5, pin31 = GPIO.gpio6, pin32 = GPIO.gpio12, pin33 = GPIO.gpio13, pin35 = GPIO.gpio19, pin36 = GPIO.gpio16, pin37 = GPIO.gpio26, pin38 = GPIO.gpio20, pin40 = GPIO.gpio21 };

    //map the gpio to its # (for easy file access)
    public enum GPIO { gpio2 = 2, gpio3 = 3, gpio4 = 4, gpio5 = 5, gpio6 = 6, gpio7 = 7, gpio8 = 8, gpio9 = 9, gpio10 = 10, gpio11 = 11, gpio12 = 12, gpio13 = 13, gpio14 = 14, gpio15 = 15, gpio16 = 16, gpio17 = 17, gpio18 = 18, gpio19 = 19, gpio20 = 20, gpio21 = 21, gpio22 = 22, gpio23 = 23, gpio24 = 24, gpio25 = 25, gpio26 = 26 };

    public class RaspberrySharpGPIO
    {
        #region Singleton
        private static RaspberrySharpGPIO m_Instance = null;
        public static RaspberrySharpGPIO Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new RaspberrySharpGPIO();
                }

                return m_Instance;
            }
        }

        #endregion 

        private Dictionary<int, Pin> m_PinTable = new Dictionary<int, Pin>();

        #region overload []
        public Pin this[GPIO i]
        {
            get { return this[(int)i]; }
        }

        public Pin this[PIN i]
        {
            get { return this[(int)i]; }
        }

        private Pin this[int i]
        {
            get
            {
                if(!m_PinTable.ContainsKey(i))
                    m_PinTable.Add(i, new Pin(i));

                return m_PinTable[i];
            }
        }
        #endregion

        #region SetPinDirections
        public void SetPinDirections(Pin.DIRECTION direction, params GPIO[] gpios)
        {
            foreach (GPIO gpio in gpios)
            {
                this[gpio].Direction = direction;
            }
        }

        public void SetPinDirections(Pin.DIRECTION direction, params PIN[] pins)
        {
            foreach (PIN pin in pins)
            {
                this[pin].Direction = direction;
            }
        }

        public void SetPinDirections(Pin.DIRECTION direction, params Pin[] pins)
        {
            foreach (Pin pin in pins)
            {
                pin.Direction = direction;
            }
        }
        #endregion

        public void CleanUpAllPins()
        {
            foreach (int key in m_PinTable.Keys)
                m_PinTable[key].CleanUpPin();
        }
    }
}
