using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VM.Net.Common;

namespace VM.Net.VirtualMachine
{
    public class Processor
    {
        private const long TICKS_PER_SECOND = 10000000;

        private Memory myMemory;
        private Memory myVideoMemory;

        private ProcessorCache myCache;

        private VirtualScreen myScreen;

        private ushort myStartAddress;
        private ushort myExecutionAddress;

        private ushort myInstructionPointer;

        private byte myCompareFlag;
        private byte myProcessorFlags;

        private long myClockSpeed;
        private long myPrevTicks;
        private long myClockTickCount;
        private Stopwatch myClock;

        private Thread myProcessorThread;

        public ProcessorCache Cache
        {
            get { return myCache; }
        }
        public ushort InstructionPointer
        {
            get { return myInstructionPointer; }
            set
            {
                myInstructionPointer = value;
            }
        }
        public VirtualScreen Screen
        {
            get { return myScreen; }
            set { myScreen = value; }
        }
        public long ClockSpeed
        {
            get { return myClockSpeed; }
            set
            {
                myClockSpeed = value;
                myClockTickCount = (TICKS_PER_SECOND / value);
            }
        }
        public bool IsPasued
        {
            get { return myClock.IsRunning; }
            set
            {
                if (value)
                    myClock.Stop();
                else
                    myClock.Start();
            }
        }

        public Processor()
        {
            myMemory = new Memory(63 * 1024);
            myVideoMemory = new Memory(128 * 72 * 3);
            myCache = new ProcessorCache();

            myClock = new Stopwatch();
            myClock.Start();

            ClockSpeed = 5;

            myInstructionPointer = 0;
            myCompareFlag = 0;
            myProcessorFlags = 0;
        }

        public void LoadProgram(BinaryReader reader, ushort memoryAddress)
        {
            char[] magic;
            magic = reader.ReadChars(CompilerSettings.MagicLength);

            if (CompilerSettings.VerifyMagicNumbers(magic))
            {
                ushort counter = 0;
                ushort progLength = 0;

                myStartAddress = memoryAddress;
                progLength = reader.ReadUInt16();
                myExecutionAddress = reader.ReadUInt16();

                myMemory[myStartAddress] = (byte)(progLength >> 8);
                myMemory[myStartAddress + 1] = (byte)(progLength & 255);

                myMemory[myStartAddress + 2] = (byte)(myExecutionAddress >> 8);
                myMemory[myStartAddress + 3] = (byte)(myExecutionAddress & 255);

                while (counter < progLength)
                {
                    myMemory[(myStartAddress + counter + 4)] = reader.ReadByte();
                    counter++;
                }

                myInstructionPointer = (ushort)(memoryAddress + myExecutionAddress + 4);
                
            }
            else
                MessageBox.Show("Bad file encountered");
        }

        public void ExecuteProgramFromMemory(ushort memoryAddress)
        {
            myCompareFlag = 0;
            myProcessorFlags = 0;

            myCache.Clear();

            // Decode header
            ushort progLength = (ushort)((myMemory[memoryAddress] << 8) + myMemory[memoryAddress + 1]);
            ushort execAddress = (ushort)((myMemory[memoryAddress + 2] << 8) + myMemory[memoryAddress + 3]);

            myInstructionPointer = (ushort)(memoryAddress + execAddress + 4);

            myProcessorThread = new Thread(() => ExecuteProgram(execAddress, progLength, (ushort)(memoryAddress + 4)));
            myProcessorThread.Start();
        }

        private byte GetCompareFlags(ushort left, ushort right)
        {
            byte result = 0;

            if (left == right)
                result |= (byte)CompareFlags.Equal;

            if (left != right)
                result |= (byte)CompareFlags.NotEqual;

            if (left > right)
                result |= (byte)CompareFlags.GreaterThan;

            if (left < right)
                result |= (byte)CompareFlags.LessThan;

            return result;
        }

        private void NanoSleep()
        {
            while(myClock.ElapsedTicks < myPrevTicks + myClockTickCount)
            {
                System.Diagnostics.Debug.Write("foo");
            }

            myPrevTicks = myClock.ElapsedTicks;
        }

        public void ExecuteProgram(ushort executionAddress, ushort programLength, ushort startMemoryAddress)
        {
            //programLength = 64000;

            byte instruction = myMemory[myInstructionPointer];

            byte prevInstruction = instruction;

            ushort programDataLocation = (ushort)(startMemoryAddress + programLength);

            long progTimerStart = Environment.TickCount;

            while (instruction != 0x04) // As long as we are not at the end
            {
                instruction = myMemory[myInstructionPointer];
                programLength--;

                //if (myClockTickCount > 0)
                //    NanoSleep();
                
                byte compValueByte = 0;
                ushort compValueWord = 0;
                ushort jumpTo = 0;
                byte oldCarryFlag;
                byte addValueByte;
                byte subOpCode;
                ushort location;

                long startTime = Environment.TickCount;

                switch (instruction)
                {
                    case 0x01: // LDA #<value>
                        myCache.RegisterA = myMemory[InstructionPointer + 1];
                        programLength -= 1;
                        myInstructionPointer += 2;
                        break;

                    case 0x02: // LDX #<value>
                        myCache.RegisterX = (ushort)(myMemory[InstructionPointer + 2] << 8);
                        myCache.RegisterX += myMemory[InstructionPointer + 1];
                        programLength -= 2;
                        myInstructionPointer += 3;
                        break;

                    case 0x03: // STA ,X
                        subOpCode = myMemory[myInstructionPointer + 1];

                        switch (subOpCode)
                        {
                            case 0x01:
                                myMemory[myCache.RegisterX] = myCache.RegisterA;
                                myScreen?.ThreadSafePoke(myCache.RegisterX, myCache.RegisterA);
                                break;
                            case 0x02:
                                myMemory[myCache.RegisterY] = myCache.RegisterA;
                                myScreen?.ThreadSafePoke(myCache.RegisterY, myCache.RegisterA);
                                break;
                        }
                        myInstructionPointer += 2;
                        break;

                    case 0x04: // END
                        myInstructionPointer++;
                        break;

                    case 0x05: // CMPA
                        compValueByte = myMemory[myInstructionPointer + 1];
                        myCompareFlag = GetCompareFlags(myCache.RegisterA, compValueByte);
                        myInstructionPointer += 2;
                        break;

                    case 0x06: // CMPB
                        compValueByte = myMemory[myInstructionPointer + 1];
                        myCompareFlag = GetCompareFlags(myCache.RegisterB, compValueByte);
                        myInstructionPointer += 2;
                        break;

                    case 0x07: // CMPX
                        compValueWord = (ushort)((myMemory[myInstructionPointer + 2] << 8) + myMemory[myInstructionPointer + 1]);
                        myCompareFlag = GetCompareFlags(myCache.RegisterX, compValueWord);
                        myInstructionPointer += 3;
                        break;

                    case 0x08: // CMPY
                        compValueWord = (ushort)((myMemory[myInstructionPointer + 2] << 8) + myMemory[myInstructionPointer + 1]);
                        myCompareFlag = GetCompareFlags(myCache.RegisterY, compValueWord);
                        myInstructionPointer += 3;
                        break;

                    case 0x09: // CMPD
                        compValueWord = (ushort)((myMemory[myInstructionPointer + 2] << 8) + myMemory[myInstructionPointer + 1]);
                        myCompareFlag = GetCompareFlags(myCache.RegisterD, compValueWord);
                        myInstructionPointer += 3;
                        break;

                    case 0x0A: // JMP
                        jumpTo = (ushort)(startMemoryAddress + (myMemory[myInstructionPointer + 2] << 8) + myMemory[myInstructionPointer + 1]);
                        myInstructionPointer = jumpTo;
                        break;

                    case 0x0B: // JEQ
                        jumpTo = (ushort)(startMemoryAddress + (myMemory[myInstructionPointer + 2] << 8) + myMemory[myInstructionPointer + 1]);

                        if ((myCompareFlag & (byte)CompareFlags.Equal) == 1)
                            myInstructionPointer = jumpTo;
                        else
                            myInstructionPointer += 3;
                        break;


                    case 0x0C: // JNE
                        jumpTo = (ushort)(startMemoryAddress + (myMemory[myInstructionPointer + 2] << 8) + myMemory[myInstructionPointer + 1]);

                        if ((myCompareFlag & (byte)CompareFlags.NotEqual) == (byte)CompareFlags.NotEqual)
                            myInstructionPointer = jumpTo;
                        else
                            myInstructionPointer += 3;
                        break;

                    case 0x0D: // JGT
                        jumpTo = (ushort)(startMemoryAddress + (myMemory[myInstructionPointer + 2] << 8) + myMemory[myInstructionPointer + 1]);

                        if ((myCompareFlag & (byte)CompareFlags.GreaterThan) == (byte)CompareFlags.GreaterThan)
                            myInstructionPointer = jumpTo;
                        else
                            myInstructionPointer += 3;
                        break;

                    case 0x0E: // JLT
                        jumpTo = (ushort)(startMemoryAddress + (myMemory[myInstructionPointer + 2] << 8) + myMemory[myInstructionPointer + 1]);

                        if ((myCompareFlag & (byte)CompareFlags.LessThan) == (byte)CompareFlags.LessThan)
                            myInstructionPointer = jumpTo;
                        else
                            myInstructionPointer += 3;
                        break;

                    case 0x0F: // LDB #<value>
                        myCache.RegisterB = myMemory[InstructionPointer + 1];
                        programLength -= 1;
                        myInstructionPointer += 2;
                        break;

                    case 0x10: // LDY #<value>
                        myCache.RegisterY = (ushort)(myMemory[InstructionPointer + 2] << 8);
                        myCache.RegisterY += myMemory[InstructionPointer + 1];
                        programLength -= 2;
                        myInstructionPointer += 3;
                        break;

                    case 0x11: // VCL
                        myVideoMemory.ClearSector(myScreen.ScreenMemoryLocation, myScreen.ScreenMemorySize);
                        myScreen?.Clear();
                        myInstructionPointer += 1;
                        break;

                    case 0x12: // INCA
                        if (myCache.RegisterA == 0xFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterA++; }
                        myInstructionPointer++;
                        break;
                        
                    case 0x13: // INCB
                        if (myCache.RegisterB == 0xFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterB++; }
                        myInstructionPointer++;
                        break;
                        
                    case 0x14: // INCX
                        if (myCache.RegisterX == 0xFFFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterX++; }
                        myInstructionPointer++;
                        break;

                    case 0x15: // INCY
                        if (myCache.RegisterY == 0xFFFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterY++; }
                        myInstructionPointer++;
                        break;
                        
                    case 0x16: // INCD
                        if (myCache.RegisterD == 0xFFFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterD++; }
                        myInstructionPointer++;
                        break;
                        
                    case 0x17: // DECA
                        myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterA--; }
                        myInstructionPointer++;
                        break;

                    case 0x18: // DECB
                        myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterB--; }
                        myInstructionPointer++;
                        break;

                    case 0x19: // DECX
                        myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterX--; }
                        myInstructionPointer++;
                        break;

                    case 0x1A: // DECY
                        myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterY--; }
                        myInstructionPointer++;
                        break;

                    case 0x1B: // DECD
                        myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterD--; }
                        myInstructionPointer++;
                        break;


                    case 0x1C: // ROLA
                        oldCarryFlag = (byte)(myProcessorFlags & (byte)ProcessorFlags.Carry);

                        if ((myCache.RegisterA & 128) == 128)
                            myProcessorFlags |= (byte)ProcessorFlags.Carry;
                        else
                            myProcessorFlags &= 0xFD;

                        myCache.RegisterA <<= 1;

                        if (oldCarryFlag > 0)
                            myCache.RegisterA |= 1;
                        
                        myInstructionPointer++;
                        break;

                    case 0x1D: // ROLB
                        oldCarryFlag = (byte)(myProcessorFlags & (byte)ProcessorFlags.Carry);

                        if ((myCache.RegisterB & 128) == 128)
                            myProcessorFlags |= (byte)ProcessorFlags.Carry;
                        else
                            myProcessorFlags &= 0xFD;

                        myCache.RegisterB <<= 1;

                        if (oldCarryFlag > 0)
                            myCache.RegisterB |= 1;

                        myInstructionPointer++;
                        break;

                    case 0x1E: // RORA
                        oldCarryFlag = (byte)(myProcessorFlags & (byte)ProcessorFlags.Carry);

                        if ((myCache.RegisterA & 1) == 1)
                            myProcessorFlags |= (byte)ProcessorFlags.Carry;
                        else
                            myProcessorFlags &= 0xFD;

                        myCache.RegisterA >>= 1;

                        if (oldCarryFlag > 0)
                            myCache.RegisterA |= 128;

                        myInstructionPointer++;
                        break;

                    case 0x1F: // RORB
                        oldCarryFlag = (byte)(myProcessorFlags & (byte)ProcessorFlags.Carry);

                        if ((myCache.RegisterA & 1) == 1)
                            myProcessorFlags |= (byte)ProcessorFlags.Carry;
                        else
                            myProcessorFlags &= 0xFD;

                        myCache.RegisterB >>= 1;

                        if (oldCarryFlag > 0)
                            myCache.RegisterB |= 128;

                        myInstructionPointer++;
                        break;

                    case 0x20: // ADDA
                        addValueByte = myMemory[myInstructionPointer + 1];

                        if (myCache.RegisterA == 0xFF && addValueByte > 0)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterA += addValueByte; }

                        myInstructionPointer += 2;
                        break;

                    case 0x21: // ADDB
                        addValueByte = myMemory[myInstructionPointer + 1];

                        if (myCache.RegisterB == 0xFF && addValueByte > 0)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterB += addValueByte; }

                        myInstructionPointer += 2;
                        break;

                    case 0x22: //ADCA
                        if ((myProcessorFlags & (byte)ProcessorFlags.Carry) == (byte)ProcessorFlags.Carry)
                        {
                            if (myCache.RegisterA == 0xFF)
                                myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                            else
                                myProcessorFlags &= 0xFE;

                            unchecked { myCache.RegisterA++; }
                        }
                        myInstructionPointer += 1;
                        break;

                    case 0x23: //ADCB
                        if ((myProcessorFlags & (byte)ProcessorFlags.Carry) == (byte)ProcessorFlags.Carry)
                        {
                            if (myCache.RegisterB == 0xFF)
                                myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                            else
                                myProcessorFlags &= 0xFE;

                            unchecked { myCache.RegisterB++; }
                        }
                        myInstructionPointer += 1;
                        break;

                    case 0x24: // ADDAB
                        if ((0xFF - myCache.RegisterA) > myCache.RegisterB)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterD = (ushort)(myCache.RegisterA + myCache.RegisterB); }

                        myInstructionPointer += 1;
                        break;

                    case 0x25: // VCP ,X
                        subOpCode = myMemory[myInstructionPointer + 1];

                        switch (subOpCode)
                        {
                            case 0x01:
                                myVideoMemory[myCache.RegisterX] = myCache.RegisterA;
                                myScreen?.ThreadSafePoke(myCache.RegisterX, myCache.RegisterA);
                                break;
                            case 0x02:
                                myVideoMemory[myCache.RegisterY] = myCache.RegisterA;
                                myScreen?.ThreadSafePoke(myCache.RegisterY, myCache.RegisterA);
                                break;
                        }
                        myInstructionPointer += 2;
                        break;
                        
                    case 0x26: // STB ,X
                        subOpCode = myMemory[myInstructionPointer + 1];

                        switch (subOpCode)
                        {
                            case 0x01:
                                myMemory[myCache.RegisterX] = myCache.RegisterB;
                                myScreen?.ThreadSafePoke(myCache.RegisterX, myCache.RegisterB);
                                break;
                            case 0x02:
                                myMemory[myCache.RegisterY] = myCache.RegisterB;
                                myScreen?.ThreadSafePoke(myCache.RegisterY, myCache.RegisterB);
                                break;
                        }
                        myInstructionPointer += 2;
                        break;

                    case 0x27: // NOT ,X
                        subOpCode = myMemory[myInstructionPointer + 1];

                        switch (subOpCode)
                        {
                            case 0x01:
                                myCache.RegisterA = (byte)(~myCache.RegisterA & 0xFF);
                                break;
                            case 0x02:
                                myCache.RegisterB = (byte)(~myCache.RegisterB & 0xFF);
                                break;
                            case 0x03:
                                myCache.RegisterX = (ushort)(~myCache.RegisterX & 0xFFFF);
                                break;
                            case 0x04:
                                myCache.RegisterY = (ushort)(~myCache.RegisterY & 0xFFFF);
                                break;
                            case 0x05:
                                myCache.RegisterD = (ushort)(~myCache.RegisterD & 0xFFFF);
                                break;
                        }
                        myInstructionPointer += 2;
                        break;

                    case 0x28: // STO ,X #val
                        subOpCode = myMemory[myInstructionPointer + 1];
                        location = (ushort)((myMemory[InstructionPointer + 3] << 8) + myMemory[InstructionPointer + 2] + programDataLocation);

                        switch (subOpCode)
                        {
                            case 0x01:
                                myMemory[location] = myCache.RegisterA;
                                break;
                            case 0x02:
                                myMemory[location] = myCache.RegisterB;
                                break;
                            case 0x03:
                                myMemory[location] = (byte)(myCache.RegisterX << 8);
                                myMemory[location + 1] = (byte)(myCache.RegisterX & 0xFF);
                                break;
                            case 0x04:
                                myMemory[location] = (byte)(myCache.RegisterY << 8);
                                myMemory[location + 1] = (byte)(myCache.RegisterY & 0xFF);
                                break;
                            case 0x05:
                                myMemory[location] = (byte)(myCache.RegisterD << 8);
                                myMemory[location + 1] = (byte)(myCache.RegisterD & 0xFF);
                                break;
                        }
                        myInstructionPointer += 4;
                        break;

                    case 0x29: // LOD ,X #val
                        subOpCode = myMemory[myInstructionPointer + 1];
                        location = (ushort)(((myMemory[InstructionPointer + 3] << 8) + myMemory[InstructionPointer + 2]) + programDataLocation);

                        switch (subOpCode)
                        {
                            case 0x01:
                                myCache.RegisterA = myMemory[location];
                                break;
                            case 0x02:
                                myCache.RegisterB = myMemory[location];
                                break;
                            case 0x03:
                                myCache.RegisterX = (ushort)((myMemory[location] << 8) + myMemory[location + 1]);
                                break;
                            case 0x04:
                                myCache.RegisterY = (ushort)((myMemory[location] << 8) + myMemory[location + 1]);
                                break;
                            case 0x05:
                                myCache.RegisterD = (ushort)((myMemory[location] << 8) + myMemory[location + 1]);
                                break;
                        }
                        myInstructionPointer += 4;
                        break;

                    case 0x2A:
                        if (myCache.RegisterX + myCache.RegisterY > 0xFFFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterX += myCache.RegisterY; }

                        myInstructionPointer += 1;
                        break;

                    case 0x2B: // ADDX #val
                        compValueWord = (ushort)(((myMemory[InstructionPointer + 2] << 8) + myMemory[InstructionPointer + 1]));

                        if (myCache.RegisterX + compValueWord > 0xFFFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterX += compValueWord; }

                        myInstructionPointer += 3;
                        break;

                    case 0x2C: // ADDY #val
                        compValueWord = (ushort)(((myMemory[InstructionPointer + 2] << 8) + myMemory[InstructionPointer + 1]));

                        if (myCache.RegisterY + compValueWord > 0xFFFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { myCache.RegisterY += compValueWord; }

                        myInstructionPointer += 3;
                        break;

                    default:
                        MessageBox.Show(string.Format("Bad bytecode @{0:X4}: {1:X2}", myInstructionPointer, instruction));
                        myInstructionPointer++;
                        break;
                }

                long endTime = Environment.TickCount;

                Debug.WriteLine(string.Format("Took {0} ticks to handle byte {1:X2}", endTime - startTime, instruction));

                prevInstruction = instruction;
            }

            long progEndTime = Environment.TickCount;

            Debug.WriteLine(string.Format("Took {0} ticks to execute program", progEndTime - progTimerStart));
        }
    }
}
