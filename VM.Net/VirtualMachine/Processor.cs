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

        private uint myStartAddress;
        private uint myExecutionAddress;
        
        private byte myCompareFlag;
        private byte myProcessorFlags;

        private long myClockSpeed;
        private long myPrevTicks;
        private long myClockTickCount;
        private Stopwatch myClock;

        private IPeripheral[] myPeripherals;

        public void AttachPeripheral(IPeripheral peripheral, int slot)
        {
            myPeripherals[slot] = peripheral;
        }

        private Thread myProcessorThread;

        public ProcessorCache Cache
        {
            get { return myCache; }
        }
        public uint InstructionPointer
        {
            get { return myCache.Register_IP; }
            set
            {
                myCache.Register_IP = value;
            }
        }
        public Memory Memory
        {
            get { return myMemory; }
            set { myMemory = value; }
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

            myPeripherals = new IPeripheral[32];

            myClock = new Stopwatch();
            myClock.Start();

            ClockSpeed = 5;
            
            myCompareFlag = 0;
            myProcessorFlags = 0;
        }

        public void LoadProgram(BinaryReader reader, uint memoryAddress)
        {
            char[] magic;
            magic = reader.ReadChars(CompilerSettings.MagicLength);

            if (CompilerSettings.VerifyMagicNumbers(magic))
            {
                uint counter = 0;
                uint progLength = 0;

                long startPos = reader.BaseStream.Position;

                myStartAddress = memoryAddress;
                progLength = reader.ReadUInt32();
                myExecutionAddress = reader.ReadUInt32();

                while (counter < progLength)
                {
                    myMemory[(myStartAddress + counter + 4)] = reader.ReadByte();
                    counter++;
                }

                long endPos = reader.BaseStream.Position;

                progLength = (uint)(endPos - startPos);

                myMemory[myStartAddress] = (byte)(progLength >> 8);
                myMemory[myStartAddress + 1] = (byte)(progLength & 255);

                myMemory[myStartAddress + 2] = (byte)(myExecutionAddress >> 8);
                myMemory[myStartAddress + 3] = (byte)(myExecutionAddress & 255);
                
            }
            else
                MessageBox.Show("Bad file encountered");
        }

        public void ExecuteProgramFromMemory(uint memoryAddress)
        {
            myCompareFlag = 0;
            myProcessorFlags = 0;

            myCache.Clear();

            // Decode header
            uint progLength = (uint)((myMemory[memoryAddress] << 8) + myMemory[memoryAddress + 1]);
            uint execAddress = (uint)((myMemory[memoryAddress + 2] << 8) + myMemory[memoryAddress + 3]);

            myCache.Register_IP = (uint)(memoryAddress + execAddress + 4);

            if (myProcessorThread != null)
                myProcessorThread.Abort();

            myProcessorThread = null;

            myProcessorThread = new Thread(() => ExecuteProgram(execAddress, progLength, (uint)(memoryAddress + 4)));
            myProcessorThread.Start();
        }

        private byte GetCompareFlags(uint left, uint right)
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

            }

            myPrevTicks = myClock.ElapsedTicks;
        }

        private void PushStack(uint value)
        {
            myMemory.SetUInt32(myCache.Register_SP, value);
            myCache.Register_SP += 4;
        }

        private uint PopStack()
        {
            myCache.Register_SP -= 4;
            uint result = myMemory.GetUInt32(myCache.Register_SP);
            return result;
        }

        public void ExecuteProgram(uint executionAddress, uint programLength, uint startMemoryAddress)
        {
            uint programDataLocation = (uint)(startMemoryAddress + programLength + (1024 * 1024));
            uint stackLocation = startMemoryAddress + programLength;
            stackLocation += (8 - (stackLocation % 8));
            stackLocation += 32;

            myCache.Register_BP = stackLocation;
            myCache.Register_SP = stackLocation;

            long progTimerStart = Environment.TickCount;

            byte instruction = myMemory[myCache.Register_IP];
            byte prevInstruction = instruction;

            do // As long as we are not at the end
            {
                programLength--;

                if (myClockTickCount > 0)
                    NanoSleep();

                byte byteValue = 0;
                byte altByteValue = 0;
                uint wordValue = 0;
                uint altWordValue = 0;
                uint memWord = 0;
                
                byte oldCarryFlag;
                byte addValueByte;

                bool jumped = false;

                long startTime = Environment.TickCount;

                // Reslove this once for speed
                uint instructionPointer = myCache.Register_IP;
                
                switch (instruction)
                {
                    case 0x02: // LOD ,X #value
                        byteValue = myMemory[instructionPointer + 1];
                        wordValue = myMemory.GetUInt32(instructionPointer + 2);
                        memWord = myMemory[wordValue];
                        myCache.SetRegisterSafe(byteValue, memWord);

                        myCache.Register_IP += 1 + 4;
                        break;

                    case 0x03: // LOD ,X ,Y
                        byteValue = myMemory[instructionPointer + 1];
                        altByteValue = myMemory[instructionPointer + 2];

                        memWord = myMemory[myCache.GetValueWord(altByteValue)];

                        myCache.SetRegisterSafe(byteValue, memWord);

                        myCache.Register_IP += 1 + 4;
                        break;

                    case 0x04: //STM #value #location
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);
                        altWordValue = myMemory.GetUInt32(instructionPointer + 5);

                        myMemory.SetUInt32(altWordValue, wordValue);
                        myCache.Register_IP += 4 + 4;
                        break;

                    case 0x05: //STM #value ,X
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);
                        byteValue = myMemory[instructionPointer + 5];

                        myMemory.SetUInt32(altWordValue, myCache.GetValueWord(byteValue));
                        myCache.Register_IP += 4 + 1;
                        break;

                    case 0x06: //STOR ,X #location
                        byteValue = myMemory[programDataLocation + instructionPointer + 1];
                        wordValue = myMemory.GetUInt32(programDataLocation + instructionPointer + 2);

                        myMemory.SetUInt32(wordValue, myCache.GetValueWord(byteValue));
                        myCache.Register_IP += 1 + 4;
                        break;

                    case 0x07: //STOR ,X ,Y
                        byteValue = myMemory[programDataLocation + instructionPointer + 1];
                        altByteValue = myMemory[programDataLocation + instructionPointer + 2];

                        myMemory.SetUInt32(myCache.GetValueWord(altByteValue), myCache.GetValueWord(byteValue));
                        myCache.Register_IP += 1 + 1;
                        break;

                    case 0x08: //CMP ,X #y
                        byteValue = myMemory[instructionPointer + 1];
                        wordValue = myMemory.GetUInt32(instructionPointer + 2);
                        
                        myCompareFlag = GetCompareFlags(myCache.GetValueWord(byteValue), wordValue);

                        myCache.Register_IP += 1 + 4;
                        break;

                    case 0x09: // CMP ,X ,Y
                        byteValue = myMemory[instructionPointer + 1];
                        altByteValue = myMemory[instructionPointer + 2];

                        myCache.Register_RIA = myCache.GetValueWord(byteValue);
                        myCompareFlag = GetCompareFlags(myCache.Register_RIA, myCache.GetValueWord(altByteValue));

                        myCache.Register_IP += 1 + 1;
                        break;

                    case 0x0A: //CMP #val1 #val2
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);
                        altWordValue = myMemory.GetUInt32(instructionPointer + 5);
                        
                        myCompareFlag = GetCompareFlags(wordValue, altWordValue);

                        myCache.Register_IP += 4 + 4;
                        break;

                    case 0x0B: //CMP #val1 ,X 
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);
                        byteValue = myMemory[instructionPointer + 5];

                        myCompareFlag = GetCompareFlags(wordValue, myCache.GetValueWord(byteValue));

                        myCache.Register_IP += 4 + 4;
                        break;

                    case 0x0C: // JMP
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);
                        myCache.Register_IP = startMemoryAddress + wordValue;
                        jumped = true;
                        break;

                    case 0x0D: // JEQ
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);

                        if ((myCompareFlag & (byte)CompareFlags.Equal) != 0)
                        {
                            myCache.Register_IP = startMemoryAddress + wordValue;
                            jumped = true;
                        }
                        else
                            myCache.Register_IP += 4;

                        break;

                    case 0x0E: // JNE
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);

                        if ((myCompareFlag & (byte)CompareFlags.NotEqual) != 0)
                        {
                            myCache.Register_IP = startMemoryAddress + wordValue;
                            jumped = true;
                        }
                        else
                            myCache.Register_IP += 4;
                        break;

                    case 0x0F: // JGT
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);

                        if ((myCompareFlag & (byte)CompareFlags.GreaterThan) != 0)
                        {
                            myCache.Register_IP = startMemoryAddress + wordValue;
                            jumped = true;
                        }
                        else
                            myCache.Register_IP += 4;
                        break;

                    case 0x10: // JLT
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);

                        if ((myCompareFlag & (byte)CompareFlags.LessThan) != 0)
                        {
                            myCache.Register_IP = wordValue;
                            jumped = true;
                        }
                        else
                            myCache.Register_IP += 4;
                        break;

                    case 0x11: // INC ,X
                        byteValue = myMemory[instructionPointer + 1];
                        wordValue = myCache.GetValueWord(byteValue);

                        if (wordValue == 0xFFFFFFFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { wordValue++; }
                        myCache.SetRegisterSafe(byteValue, wordValue);
                        myCache.Register_IP += 1;
                        break;

                    case 0x12: // DEC ,X
                        byteValue = myMemory[instructionPointer + 1];
                        wordValue = myCache.GetValueWord(byteValue);

                        if (wordValue == 0x00)
                            myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                        else
                            myProcessorFlags &= 0xFE;

                        unchecked { wordValue--; }
                        myCache.SetRegisterSafe(byteValue, wordValue);
                        myCache.Register_IP += 1;
                        break;

                    case 0x13: // ROL ,X
                        byteValue = myMemory[instructionPointer + 1];
                        wordValue = myCache.GetValueWord(byteValue);

                        oldCarryFlag = (byte)(myProcessorFlags & (Byte)ProcessorFlags.Carry);

                        if ((wordValue & 0x7FFFFFFF) == 0x7FFFFFFF)
                            myProcessorFlags |= (byte)ProcessorFlags.Carry;
                        else
                            myProcessorFlags &= 0xFD;

                        wordValue <<= 1;
                        myCache.SetRegisterSafe(byteValue, wordValue);
                        myCache.Register_IP += 1;
                        break;

                    case 0x14: // ROR ,X
                        byteValue = myMemory[instructionPointer + 1];
                        wordValue = myCache.GetValueWord(byteValue);

                        oldCarryFlag = (byte)(myProcessorFlags & (Byte)ProcessorFlags.Carry);

                        if ((wordValue & 0x01) == 0x01)
                            myProcessorFlags |= (byte)ProcessorFlags.Carry;
                        else
                            myProcessorFlags &= 0xFD;

                        wordValue >>= 1;
                        myCache.SetRegisterSafe(byteValue, wordValue);
                        myCache.Register_IP += 1;
                        break;

                    case 0x15: // ADDC ,X #val
                        if ((myProcessorFlags & (byte)ProcessorFlags.Carry) == (byte)ProcessorFlags.Carry)
                        {
                            byteValue = myMemory[instructionPointer + 1];
                            wordValue = myMemory.GetUInt32(instructionPointer + 2);

                            if (wordValue == 0x7FFFFFFF)
                                myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                            else
                                myProcessorFlags &= 0xFE;

                            unchecked { wordValue++; }

                            myCache.SetRegisterSafe(byteValue, wordValue);
                        }

                        myCache.Register_IP += 1 + 4;
                        break;

                    case 0x16: // ADDC ,X ,Y
                        if ((myProcessorFlags & (byte)ProcessorFlags.Carry) == (byte)ProcessorFlags.Carry)
                        {
                            byteValue = myMemory[instructionPointer + 1];
                            altByteValue = myMemory[instructionPointer + 2];

                            wordValue = myCache.GetValueWord(altByteValue);

                            if (wordValue == 0x7FFFFFFF)
                                myProcessorFlags |= (byte)ProcessorFlags.Overflow;
                            else
                                myProcessorFlags &= 0xFE;

                            unchecked { wordValue++; }

                            myCache.SetRegisterSafe(byteValue, wordValue);
                        }

                        myCache.Register_IP += 1 + 1;
                        break;
                        
                    case 0x17:
                        byteValue = myMemory[instructionPointer + 1];
                        myCache.SetRegisterSafe(byteValue, ~myCache.GetValueWord(byteValue));
                        break;

                    case 0x18: // AND ,X #val
                        byteValue = myMemory[instructionPointer + 1];
                        wordValue = myMemory.GetUInt32(instructionPointer + 2);
                        
                        myCache.SetRegisterSafe(byteValue, (myCache.GetValueWord(byteValue) & wordValue));
                        myCache.Register_IP += 1 + 4;
                        break;

                    case 0x19: // AND ,X ,Y
                        byteValue = myMemory[instructionPointer + 1];
                        altByteValue = myMemory[instructionPointer + 2];

                        myCache.SetRegisterSafe(byteValue, (myCache.GetValueWord(byteValue) & myCache.GetValueWord(altByteValue)));
                        myCache.Register_IP += 1 + 1;
                        break;


                        // TODO 0x1A - 0x2D


                    case 0x2E: // POLL #device ,X
                        byteValue = myMemory[instructionPointer + 1];
                        altByteValue = myMemory[instructionPointer + 2];

                        uint? pollValue1 = myPeripherals[byteValue]?.Poll();

                        if (pollValue1.HasValue)
                            myCache.SetRegisterSafe(altByteValue, pollValue1.Value);

                        myCache.Register_IP += 1 + 1;
                        break;

                    case 0x2F: // POLL ,X ,Y
                        byteValue = myMemory[instructionPointer + 1];
                        altByteValue = myMemory[instructionPointer + 2];

                        uint? pollValue2 = myPeripherals[(myCache.GetValueWord(byteValue) & 0xFF)]?.Poll();

                        if (pollValue2.HasValue)
                            myCache.SetRegisterSafe(altByteValue, pollValue2.Value);
                        
                        myCache.Register_IP += 1 + 1;
                        break;

                    case 0x30: // PASS #value #device
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);
                        byteValue = myMemory[instructionPointer + 5];

                        myPeripherals[byteValue]?.Pass(wordValue);
                        myCache.Register_IP += 4 + 1;
                        break;

                    case 0x31: // PASS ,X #device
                        byteValue = myMemory[instructionPointer + 1];
                        altByteValue = myMemory[instructionPointer + 2];

                        myPeripherals[altByteValue]?.Pass(myCache.GetValueWord(byteValue));
                        myCache.Register_IP += 1 + 1;
                        break;


                    case 0x32: // PASS #value ,X
                        wordValue = myMemory.GetUInt32( instructionPointer + 1);
                        byteValue = myMemory[instructionPointer + 5];

                        myPeripherals[(myCache.GetValueWord(byteValue) & 0xFF)]?.Pass(wordValue);
                        myCache.Register_IP += 4 + 1;
                        break;

                    case 0x33: // PASS ,X ,Y
                        byteValue = myMemory[instructionPointer + 1];
                        altByteValue = myMemory[instructionPointer + 2];

                        myPeripherals[(myCache.GetValueWord(altByteValue) & 0xFF)]?.Pass(myCache.GetValueWord(byteValue));
                        
                        myCache.Register_IP += 1 + 1;
                        break;

                    case 0x34: // PUSH #val
                        wordValue = myMemory.GetUInt32(instructionPointer + 1);

                        PushStack(wordValue);
                                             
                        myCache.Register_IP += 4;
                        break;

                    case 0x35: // PUSH ,X
                        byteValue = myMemory[instructionPointer + 1];

                        if (byteValue == 0xF0)
                            PushStack(myCache.Register_IP + 7);
                        else
                            PushStack(myCache.GetValueWord(byteValue));

                        myCache.Register_IP += 1;
                        break;

                    case 0x36: // POP ,X
                        byteValue = myMemory[instructionPointer + 1];

                        wordValue = PopStack();

                        myCache.SetRegisterSafe(byteValue, wordValue);

                        if (byteValue == (byte)RegisterAddress.IP)
                            jumped = true;
                        else
                            myCache.Register_IP += 1;

                        break;

                    case 0x37: // MOV ,X ,Y
                        byteValue = myMemory[instructionPointer + 1];
                        altByteValue = myMemory[instructionPointer + 2];

                        myCache.SetRegisterSafe(byteValue, myCache.GetValueWord(altByteValue));

                        myCache.Register_IP += 1 + 1;
                        break;
                        

                    default:
                        MessageBox.Show(string.Format("Bad bytecode @{0:X4}: {1:X2}, previous instruction is {2:X2}", instructionPointer, instruction, prevInstruction));
                        myCache.Register_IP++;
                        myProcessorThread.Abort();
                        break;
                }

                // Assume we ran an instruction
                if (!jumped)
                    myCache.Register_IP += 1;

                long endTime = Environment.TickCount;

                Debug.WriteLine(string.Format("Took {0} ticks to handle byte {1:X2}", endTime - startTime, instruction));

                prevInstruction = instruction;
                instruction = myMemory[myCache.Register_IP];
            } while (instruction != 0x01);

            long progEndTime = Environment.TickCount;

            Debug.WriteLine(string.Format("Took {0} ticks to execute program", progEndTime - progTimerStart));
        }
    }
}
