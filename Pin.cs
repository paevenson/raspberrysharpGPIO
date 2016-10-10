using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace RaspberrySharpGPIOLib
{
    public class Pin
    {
        public static int QUERY_IN_INTERVAL = 10; //10 ms

        public const string GPIO_PATH = "/sys/class/gpio/";

        public enum DIRECTION { IN, OUT, NONE };

        //Read In Thread
        private Thread m_QueryInThread;
        private bool m_StopQueryThread = false;

        bool m_IsExported = false;

        int m_PinId;

        DIRECTION m_PinDiection = DIRECTION.NONE;

        int m_Value = 0;

        public event Action<int, bool> ExportChanged; //<PinId, Export>
        public event Action<int, DIRECTION> DirectionChanged; //<PinId, Direction>
        public event Action<int, int> ValueChanged; //<PinId, Value>

        public bool IsExported
        {
            get { return m_IsExported; }
            private set
            {
                //retru if we arent changing
                if ((value == true && m_IsExported == true) || (value == false && m_IsExported == false))
                    return;

                ChangeExportState(value);
            }
        }

        public DIRECTION Direction
        {
            get { return m_PinDiection; }
            set
            {
                //export it
                if (IsExported == false)
                    IsExported = true;

                if (m_PinDiection != value)
                    ChangeDirectionState(value);
            }
        }

        public int Value
        {
            get { return m_Value; }
            set
            {
                if (IsExported == false)
                    IsExported = true;

                if (m_Value != value)
                    ChangePinValue(value);
            }
        }

        public Pin(int id)
        {
            m_PinId = id;

            if(ReadExport())
                CleanUpPin(true);

            IsExported = true;

            m_Value = ReadPinValue();
        }

        public void CleanUpPin(bool force = false)
        {
            IsExported = false;

            if(m_QueryInThread != null && m_QueryInThread.IsAlive)
                m_StopQueryThread = true;

            if (force)
                ChangeExportState(false);
        }

        private int ReadPinValue()
        {
            return ReadIO(GPIO_PATH + "gpio" + m_PinId.ToString() + "/value");
        }

        private bool ReadExport()
        {
            return Directory.Exists(GPIO_PATH + "gpio" + m_PinId.ToString());
        }

        private DIRECTION ReadDirection()
        {
            return (DIRECTION)ReadIO(GPIO_PATH + "gpio" + m_PinId.ToString() + "/direction");
        }

        private int ReadIO(string path)
        {
            string filename = GPIO_PATH + "gpio" + m_PinId.ToString() + "/value";
            if (File.Exists(path))
            {
                string readValue = File.ReadAllText(path);
                if (readValue != null && readValue.Length > 0 && readValue[0] == '1')
                    return 1;
            }

            return 0;
        }

        private void ChangeExportState(bool export)
        {
            File.WriteAllText(GPIO_PATH + (export ? "export":"unexport"), m_PinId.ToString());
            m_IsExported = export;

            if (ExportChanged != null)
                ExportChanged(m_PinId, m_IsExported);
        }

        //
        private void ChangeDirectionState(DIRECTION direction)
        {
            File.WriteAllText(GPIO_PATH + "gpio" + m_PinId.ToString() + "/direction", direction.ToString().ToLowerInvariant());

            //start up the thread
            if(direction == DIRECTION.IN)
            {
                m_QueryInThread = new Thread(ReadPinThread);
                m_QueryInThread.Start();
            }
            else if(m_PinDiection == DIRECTION.IN && direction != DIRECTION.IN)
            {
                m_StopQueryThread = true;   //Stop our thread
                while (m_StopQueryThread == true) Thread.Sleep(2); //wait for the thread to stop...
            }

            m_PinDiection = direction;

            if (DirectionChanged != null)
                DirectionChanged(m_PinId, m_PinDiection);
        }

        //0 == low, 1 == high
        private void ChangePinValue(int pinValue)
        {
            if(Direction == DIRECTION.OUT)
                File.WriteAllText(GPIO_PATH + "gpio" + m_PinId.ToString() + "/value", pinValue.ToString());
            m_Value = pinValue;

            if (ValueChanged != null)
                ValueChanged(m_PinId, pinValue);
        }

        //loops based in the query_in_interval
        private void ReadPinThread()
        {
            while(m_StopQueryThread == false)
            {
                try
                {
                    this.Value = ReadPinValue();
                }
                catch  {  }

                Thread.Sleep(QUERY_IN_INTERVAL);
            }

            m_StopQueryThread = false;
        }
    }
}
