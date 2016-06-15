using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VM.Net.VirtualMachine
{
    public partial class VirtualScreen : UserControl, IPeripheral
    {

        private ushort myScreenMemoryLocation;

        private uint[] myScreenMemory;
        private uint myScreenMode = 0x01;

        private Timer myRefreshTimer;

        private Stack<uint> myInputStream;

        public ushort ScreenMemoryLocation
        {
            get { return myScreenMemoryLocation; }
            set { myScreenMemoryLocation = value; }
        }
        public ushort ScreenMemorySize
        {
            get { return (ushort)myScreenMemory.Length; }
        }

        private delegate void PokeDelegate(uint address, byte value);
        private delegate void EmptyDelegate();
        private delegate void PassDelegate(uint value);

        private object myThreadLock;

        private int myWidth;
        private int myHeight;

        public VirtualScreen() : this(128, 72) { }

        public VirtualScreen(int width = 128, int height = 72)
        {
            InitializeComponent();
            DoubleBuffered = true;

            myWidth = width;
            myHeight = height;

            myInputStream = new Stack<uint>();

            myScreenMemoryLocation = 0x00;
            //myScreenMemory = new byte[4000]; // by default we use an 80x25 screen of 2 bytes
            myScreenMemory = new uint[width * height];

            myThreadLock = new Object();

            myRefreshTimer = new Timer();
            myRefreshTimer.Interval = 1000 / 1;
            myRefreshTimer.Tick += (X, Y) => { OnRefeshTimer(); };
            myRefreshTimer.Start();

            Clear();
        }

        private void OnRefeshTimer()
        {
            Refresh();
        }

        public void Clear()
        {
            if (InvokeRequired)
            {
                Invoke(new EmptyDelegate(Clear));
            }
            else
            {
                lock (myThreadLock)
                {
                    for(int index = 0; index < myScreenMemory.Length; index +=3)
                    {
                        myScreenMemory[index] = 0;
                        myScreenMemory[index + 1] = 0;
                        myScreenMemory[index + 2] = 0;
                    }

                    //for (int index = 0; index < myScreenMemory.Length; index += 2)
                    //{
                    //    myScreenMemory[index] = 32;
                    //    myScreenMemory[index + 1] = 7;
                    //}

                    //Refresh();
                }
            }
        }
        
        public void ThreadSafePoke(uint address, byte value)
        {
            if (InvokeRequired)
            {
                Invoke(new PokeDelegate(Poke), address, value);
            }
            else
            {
                Poke(address, value);
            }
        }

        /// <summary>
        /// Sets a single value in the virtual screen's memory to the given value
        /// </summary>
        /// <param name="address">The address in terms of shared memory</param>
        /// <param name="value">The value to set</param>
        public void Poke(uint address, byte value)
        {
            ushort memoryLocation;

            try { memoryLocation = (ushort)(address - myScreenMemoryLocation); }
            catch (Exception) { return; }

            if (memoryLocation >= myScreenMemory.Length)
                return;

            myScreenMemory[memoryLocation] = value;
            //Refresh();
        }

        /// <summary>
        /// Gets a single value in the virtual screen's memory
        /// </summary>
        /// <param name="address">The address in terms of shared memory</param>
        /// <returns>The value at the given address</returns>
        public uint Peek(uint address)
        {
            ushort memoryLocation;

            try { memoryLocation = (ushort)(address - myScreenMemoryLocation); }
            catch (Exception) { return 0; }

            if (memoryLocation >= myScreenMemory.Length)
                return 0;

            return myScreenMemory[memoryLocation];
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            lock (myThreadLock)
            {
                Bitmap bmp = new Bitmap(myWidth, myHeight);
                Graphics bmpGraphics = Graphics.FromImage(bmp);
                Font font = new Font("Courier New", 10.0f, FontStyle.Bold);

                int xPos = 0;
                int yPos = 0;

                for (int index = 0; index < myScreenMemory.Length; index ++)
                {
                    xPos = index % myWidth;
                    yPos = index / myWidth;

                    bmp.SetPixel(xPos, yPos, Color.FromArgb((int)myScreenMemory[index]));
                }

                //for (int index = 0; index < myScreenMemory.Length; index += 2) 
                //{
                //    SolidBrush bgBrush = null;
                //    SolidBrush fgBrush = null;

                //    if ((myScreenMemory[index + 1] & 112) == 112)
                //    {
                //        bgBrush = new SolidBrush(Color.Gray);
                //    }
                //    if ((myScreenMemory[index + 1] & 112) == 96)
                //    {
                //        bgBrush = new SolidBrush(Color.Brown);
                //    }
                //    if ((myScreenMemory[index + 1] & 112) == 80)
                //    {
                //        bgBrush = new SolidBrush(Color.Magenta);
                //    }
                //    if ((myScreenMemory[index + 1] & 112) == 64)
                //    {
                //        bgBrush = new SolidBrush(Color.Red);
                //    }
                //    if ((myScreenMemory[index + 1] & 112) == 48)
                //    {
                //        bgBrush = new SolidBrush(Color.Cyan);
                //    }
                //    if ((myScreenMemory[index + 1] & 112) == 32)
                //    {
                //        bgBrush = new SolidBrush(Color.Green);
                //    }
                //    if ((myScreenMemory[index + 1] & 112) == 16)
                //    {
                //        bgBrush = new SolidBrush(Color.Blue);
                //    }
                //    if ((myScreenMemory[index + 1] & 112) == 0)
                //    {
                //        bgBrush = new SolidBrush(Color.Black);
                //    }
                //    if ((myScreenMemory[index + 1] & 7) == 0)
                //    {
                //        if ((myScreenMemory[index + 1] & 8) == 8)
                //        {
                //            fgBrush = new SolidBrush(Color.Gray);
                //        }
                //        else
                //        {
                //            fgBrush = new SolidBrush(Color.Black);
                //        }
                //    }
                //    if ((myScreenMemory[index + 1] & 7) == 1)
                //    {
                //        if ((myScreenMemory[index + 1] & 8) == 8)
                //        {
                //            fgBrush = new SolidBrush(Color.LightBlue);
                //        }
                //        else
                //        {
                //            fgBrush = new SolidBrush(Color.Blue);
                //        }
                //    }
                //    if ((myScreenMemory[index + 1] & 7) == 2)
                //    {
                //        if ((myScreenMemory[index + 1] & 8) == 8)
                //        {
                //            fgBrush = new SolidBrush(Color.LightGreen);
                //        }
                //        else
                //        {
                //            fgBrush = new SolidBrush(Color.Green);
                //        }
                //    }
                //    if ((myScreenMemory[index + 1] & 7) == 3)
                //    {
                //        if ((myScreenMemory[index + 1] & 8) == 8)
                //        {
                //            fgBrush = new SolidBrush(Color.LightCyan);
                //        }
                //        else
                //        {
                //            fgBrush = new SolidBrush(Color.Cyan);
                //        }
                //    }
                //    if ((myScreenMemory[index + 1] & 7) == 4)
                //    {
                //        if ((myScreenMemory[index + 1] & 8) == 8)
                //        {
                //            fgBrush = new SolidBrush(Color.Pink);
                //        }
                //        else
                //        {
                //            fgBrush = new SolidBrush(Color.Red);
                //        }
                //    }
                //    if ((myScreenMemory[index + 1] & 7) == 5)
                //    {
                //        if ((myScreenMemory[index + 1] & 8) == 8)
                //        {
                //            fgBrush = new SolidBrush(Color.Fuchsia);
                //        }
                //        else
                //        {
                //            fgBrush = new SolidBrush(Color.Magenta);
                //        }
                //    }
                //    if ((myScreenMemory[index + 1] & 7) == 6)
                //    {
                //        if ((myScreenMemory[index + 1] & 8) == 8)
                //        {
                //            fgBrush = new SolidBrush(Color.Yellow);
                //        }
                //        else
                //        {
                //            fgBrush = new SolidBrush(Color.Brown);
                //        }
                //    }
                //    if ((myScreenMemory[index + 1] & 7) == 7)
                //    {
                //        if ((myScreenMemory[index + 1] & 8) == 8)
                //        {
                //            fgBrush = new SolidBrush(Color.White);
                //        }
                //        else
                //        {
                //            fgBrush = new SolidBrush(Color.Gray);
                //        }
                //    }

                //    if (bgBrush ==  null       )
                //        bgBrush = new SolidBrush(Color.Black);
                //    if (fgBrush ==  null       )
                //        fgBrush = new SolidBrush(Color.Gray);

                //    if (((xPos) % 640 == 0) && (xPos != 0))
                //    {
                //        yPos += 11;
                //        xPos = 0;
                //    }
                //    string s = Encoding.ASCII.GetString(myScreenMemory, index, 1);
                //    PointF pos = new PointF(xPos, yPos);

                //    bmpGraphics.FillRectangle(bgBrush, xPos + 2, yPos + 2, 8.0f, 11.0f);
                //    bmpGraphics.DrawString(s, font, fgBrush, pos);
                //    xPos += 8;
                //}

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.FillRectangle(Brushes.Black, this.ClientRectangle);
                e.Graphics.DrawImage(bmp, 0, 0, Width, Height);
                bmpGraphics.Dispose();
                bmp.Dispose();
            }
        }

        public uint Poll()
        {
            return 0x00000000;
        }

        public void Pass(uint value)
        {
            if (InvokeRequired)
            {
                Invoke(new PassDelegate(Pass), value);
            }
            else
            {
                byte cmd = (byte)((value & 0xFF000000) >> 24);

                switch (cmd)
                {
                    case 0x01:
                        myScreenMode = 0x01;
                        break;
                    case 0x02:
                        myScreenMode = 0x02;
                        break;
                    case 0x03:
                        myScreenMode = 0x03;
                        break;
                    default:
                        myInputStream.Push(value);
                        break;
                }

                switch (myScreenMode)
                {
                    case 0x01:
                        if (myInputStream.Count > 1)
                        {
                            uint memLoc = myInputStream.Pop();
                            uint col = myInputStream.Pop();

                            myScreenMemory[memLoc] = col;
                        }
                        break;

                    case 0x02:
                        if (myInputStream.Count == myScreenMemory.Length)
                        {
                            myScreenMemory = myInputStream.ToArray();
                            myInputStream.Clear();
                        }
                        break;
                    case 0x03:
                        if (myInputStream.Count == 1)
                        {
                            uint color = myInputStream.Pop();

                            for (int index = 0; index < myScreenMemory.Length; index++)
                                myScreenMemory[index] = color;
                        }
                        break;
                }
            }
        }
    }
}
