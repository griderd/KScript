using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KScript.Hardware.Peripherals;
using KScript.Interrupts;
using MicroLibrary;

namespace KScript.Hardware
{
    public enum CPUMode
    {
        Debug,
        Real
    }

    public class CPU : Hardware
    {
        enum InterruptState
        {
            Standby,
            WaitForDeviceId,
            WaitForObjectId,
            WaitForObjectState
        }

        public struct InterruptInfo
        {
            public int interruptType, deviceId, objectId, objectState;
        }

        public const int WORD_LENGTH = 4;

        public event EventHandler StateChanged;

        object lockObject = new object();

        //Timer clock;
        //Thread runThread;
        MicroStopwatch stopwatch;
        MicroTimer clock;

        Stack<int> callStack;
       
        int programCounter;
        Instructions instruction;
        int arg1, arg2;
        byte instructionByte;
        SystemBoard systemBoard;
        RAM memory;

        int[] generalRegisters;
        int interruptHandlerPtr;

        InterruptState interruptState = InterruptState.Standby;
        InterruptInfo currentInterrupt;

        public int[] CallStack { get { return callStack.ToArray(); } }
        public Instructions Instruction { get { return instruction; } }
        public int Arg1 { get { return arg1; } }
        public int Arg2 { get { return arg2; } }
        public int ProgramCounter { get { return programCounter; } }

        public int ClockSpeed { get { return (int)(1000000 / clock.Interval); } }

        int maxCycleLength;
        int lastCycleLength;
        long cycleLengths;
        long cycleCount;

        public int MaxCycleLength { get { return maxCycleLength; } }
        public int AverageCycleRate { get { if (AverageCycleLength == 0) { return 0; } return (int)(1000000 / AverageCycleLength); } }
        public int AverageCycleLength { get { if (cycleCount == 0) { return 0; } return (int)(cycleLengths / cycleCount); } }
        public int LastCycleLength { get { return lastCycleLength; } }
        public long CycleCount { get { return cycleCount; } }
        
        public int this[int index]
        {
            get
            {
                return generalRegisters[index];
            }
            private set
            {
                generalRegisters[index] = value;
            }
        }

        public int this[Instructions register]
        {
            get
            {
                if ((register < Instructions.rega) | (register > Instructions.regh))
                    throw new ArgumentOutOfRangeException();
                return this[GetRegisterId(register)];
            }
            private set
            {
                if ((register < Instructions.rega) | (register > Instructions.regh))
                    throw new ArgumentOutOfRangeException();

                this[GetRegisterId(register)] = value;
            }
        }

        public CPUMode Mode { get; private set; }

        public bool Busy { get; private set; }

        public bool ChangeMode(CPUMode mode)
        {
            if (!Running)
            {
                Mode = mode;
                return true;
            }
            return false;
        }

        public CPU(SystemBoard board, CPUMode mode, double frequency) : base(HardwareType.CPU)
        {
            // Initialize 
            instructionByte = 0;
            Mode = mode;

            // Initialize general-purpose registers
            generalRegisters = new int[6];

            systemBoard = board;

            callStack = new Stack<int>();
            Busy = false;
            clock = new MicroTimer(25);
            clock.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(clock_MicroTimerElapsed);
            stopwatch = new MicroStopwatch();

            cycleLengths = 0;
            cycleCount = 0;
            lastCycleLength = 0;
            maxCycleLength = 0;
        }

        void clock_MicroTimerElapsed(object sender, MicroTimerEventArgs timerEventArgs)
        {
            BeginCycle();
        }

        public void Step()
        {
            if (Mode == CPUMode.Debug)
            {
                BeginCycle();
            }
        }

        protected override void _Start()
        {
            memory = (RAM)systemBoard[IOPorts.RAM];
            programCounter = 0;
            //runThread = new Thread(new ThreadStart(Run));
            //if (Mode != CPUMode.Debug) runThread.Start();
            if (Mode != CPUMode.Debug) clock.Start();
        }

        protected override void _Stop()
        {
            clock.Stop();
        }

        private int GetRegisterId(Instructions register)
        {
            return (int)register - 1;
        }

        #region Control Unit

        private void RaiseStateChanged()
        {
            if (StateChanged != null) StateChanged(this, new EventArgs());
        }

        public void Run()
        {
            while (Running)
            {
                BeginCycle();
            }
        }

        public void BeginCycle()
        {
            //lock (lockObject)
            {
                if (!Busy)
                {
                    //bool sample = programCounter == 0;
                    //if (sample) stopwatch.Start();
                    stopwatch.Start();

                    Busy = true;
                    instructionByte = Fetch();
                    if (instructionByte != 0)
                    {
                        RaiseStateChanged();
                        Decode();
                        Execute();

                        cycleCount++;
                        cycleLengths += stopwatch.ElapsedMicroseconds;
                        lastCycleLength = (int)stopwatch.ElapsedMicroseconds;
                        if (lastCycleLength > maxCycleLength) maxCycleLength = lastCycleLength;
                    }

                    stopwatch.Reset();
                    Busy = false;

                    //if (sample)
                    //{
                    //    stopwatch.Stop();
                    //    clock.Interval = stopwatch.ElapsedMicroseconds;
                    //}
                }
            }
        }

        byte Fetch()
        {
            byte value = memory.ReadByte(programCounter);
            programCounter++;
            return value;
        }

        int FetchWord()
        {
            int value = memory.ReadWord(programCounter);
            programCounter += WORD_LENGTH;
            return value;
        }

        public void Decode()
        {
            int argCount = 0;

            instruction = (Instructions)instructionByte;
            switch (instruction)
            {
                case Instructions.jfalse:
                case Instructions.jmp:
                case Instructions.jtrue:
                case Instructions._goto:
                case Instructions.gototrue:
                case Instructions.gotofalse:
                case Instructions.inp:
                case Instructions.outp:
                case Instructions.push:
                case Instructions.pop:
                case Instructions.pushi:
                case Instructions.popi:
                case Instructions.peeki:
                case Instructions.pushr:
                case Instructions.cpuid:
                case Instructions.incrr:
                case Instructions.decrr:
                case Instructions.inthnd:
                case Instructions.jfalser:
                case Instructions.jtruer:
                case Instructions.jmpr:
                case Instructions.gototruer:
                case Instructions.gotofalser:
                case Instructions.gotor:
                    argCount = 1;
                    break;

                case Instructions.mov:
                case Instructions.movp:
                case Instructions.rmem:
                case Instructions.wmem:
                case Instructions.rmemr:
                case Instructions.wmemr:
                case Instructions.wvmemr:
                case Instructions.wvmem:
                    argCount = 2;
                    break;
            }
            
            arg1 = 0;
            arg2 = 0;

            if (argCount >= 1) arg1 = FetchWord();
            if (argCount == 2) arg2 = FetchWord();
            RaiseStateChanged();
        }

        public void Execute()
        {
            if ((instruction >= Instructions.pushi) & (instruction <= Instructions.lequal))
            {
                systemBoard[IOPorts.ALU].WriteToPort(instructionByte);
                if (instruction == Instructions.pushi)
                    systemBoard[IOPorts.ALU].WriteToPort(arg1);
                else if (instruction == Instructions.pushr)
                    systemBoard[IOPorts.ALU].WriteToPort(this[arg1 - 1]);
                else if ((instruction == Instructions.popi) | (instruction == Instructions.peeki))
                    generalRegisters[arg1 - 1] = systemBoard[IOPorts.ALU].OutPort;

                return;
            }

            switch (instruction)
            {
                case Instructions.jtrue:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] != 0)
                    {
                        callStack.Push(programCounter);
                        programCounter = arg1;
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;
                    
                case Instructions.jtruer:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] != 0)
                    {
                        callStack.Push(programCounter);
                        programCounter = generalRegisters[GetRegisterId((Instructions)arg1)];
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;

                    
                case Instructions.jfalse:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] == 0)
                    {
                        callStack.Push(programCounter);
                        programCounter = arg1;
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;

                case Instructions.jfalser:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] == 0)
                    {
                        callStack.Push(programCounter);
                        programCounter = generalRegisters[GetRegisterId((Instructions)arg1)];
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;

                case Instructions.jmp:
                    callStack.Push(programCounter);
                    programCounter = arg1;
                    break;

                case Instructions.jmpr:
                    callStack.Push(programCounter);
                    programCounter = generalRegisters[GetRegisterId((Instructions)arg1)];
                    break;

                case Instructions._goto:
                    programCounter = arg1;
                    break;

                case Instructions.gotor:
                    programCounter = generalRegisters[GetRegisterId((Instructions)arg1)];
                    break;

                case Instructions.gototrue:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] != 0)
                    {
                        programCounter = arg1;
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;

                case Instructions.gototruer:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] != 0)
                    {
                        programCounter = generalRegisters[GetRegisterId((Instructions)arg1)];
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;

                case Instructions.gotofalse:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] == 0)
                    {
                        programCounter = arg1;
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;

                case Instructions.gotofalser:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] == 0)
                    {
                        programCounter = generalRegisters[GetRegisterId((Instructions)arg1)];
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;
                   
                case Instructions.ret:
                    programCounter = callStack.Pop();
                    break;

                case Instructions.rettrue:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] != 0)
                    {
                        programCounter = callStack.Pop();
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;

                case Instructions.retfalse:
                    systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.peeki);
                    this[Instructions.regg] = systemBoard[IOPorts.ALU].OutPort;
                    if (generalRegisters[GetRegisterId(Instructions.regg)] == 0)
                    {
                        programCounter = callStack.Pop();
                        systemBoard[IOPorts.ALU].WriteToPort((int)Instructions.popi);
                    }
                    break;

                case Instructions.outp:
                    systemBoard[arg1].WriteToPort(this[Instructions.rega]);
                    break;

                case Instructions.inp:
                    this[Instructions.rega] = systemBoard[arg1].OutPort;
                    break;

                case Instructions.mov:
                case Instructions.movp:
                    this[arg1 - 1] = arg2;
                    break;

                case Instructions.movr:
                    this[arg1 - 1] = this[arg2 - 1];
                    break;

                case Instructions.push:
                    callStack.Push(this[arg1 - 1]);
                    break;

                case Instructions.pop:
                    this[arg1 - 1] = callStack.Pop();
                    break;

                case Instructions.poprx:
                    callStack.Pop();
                    break;

                case Instructions.pusha:
                    callStack.Push(this[Instructions.rega]);
                    callStack.Push(this[Instructions.regb]);
                    callStack.Push(this[Instructions.regc]);
                    callStack.Push(this[Instructions.regd]);
                    callStack.Push(this[Instructions.regg]);
                    callStack.Push(this[Instructions.regh]);
                    break;

                case Instructions.popa:
                    this[Instructions.regh] = callStack.Pop();
                    this[Instructions.regg] = callStack.Pop();
                    this[Instructions.regd] = callStack.Pop();
                    this[Instructions.regc] = callStack.Pop();
                    this[Instructions.regb] = callStack.Pop();
                    this[Instructions.rega] = callStack.Pop();
                    break;

                case Instructions.rmem:
                    this[arg1 - 1] = ((RAM)systemBoard[IOPorts.RAM]).ReadWord(arg2);
                    break;

                case Instructions.wmem:
                    ((RAM)systemBoard[IOPorts.RAM]).WriteWord(arg1, this[arg2 - 1]);
                    break;

                case Instructions.rmemr:
                    this[arg1 - 1] = ((RAM)systemBoard[IOPorts.RAM]).ReadWord(this[arg2 - 1]);
                    break;

                case Instructions.wmemr:
                    ((RAM)systemBoard[IOPorts.RAM]).WriteWord(this[arg1 - 1], this[arg2 - 1]);
                    break;

                case Instructions.wvmem:
                    ((GraphicsAdapters.DisplayAdapter)systemBoard[IOPorts.Video]).WriteWord(arg1, arg2);
                    break;

                case Instructions.wvmemr:
                    ((GraphicsAdapters.DisplayAdapter)systemBoard[IOPorts.Video]).WriteWord(this[arg1 - 1], this[arg2 - 1]);
                    break;

                case Instructions.inthnd:
                    interruptHandlerPtr = arg1;
                    break;

                case Instructions.hlt:
                    Stop();
                    break;

                case Instructions.cpuid:
                    switch (arg1)
                    {
                        case 0:
                            this[Instructions.regb] = BitConverter.ToInt32(Encoding.ASCII.GetBytes("KScr"), 0);
                            this[Instructions.regc] = BitConverter.ToInt32(Encoding.ASCII.GetBytes("ipt "), 0);
                            this[Instructions.regd] = BitConverter.ToInt32(Encoding.ASCII.GetBytes("CPU\0"), 0);
                            break;

                        case 1:
                            this[Instructions.regb] = WORD_LENGTH;
                            break;

                        case 2:
                            this[Instructions.regb] = ClockSpeed;
                            break;
                    }
                    break;

                case Instructions.incrr:
                    this[arg1 - 1] = this[arg1 - 1] + 1;
                    break;

                case Instructions.decrr:
                    this[arg1 - 1] = this[arg1 - 1] - 1;
                    break;
            }
        }

#endregion 

        /// <summary>
        /// Uses the IN port for hardware interrupts
        /// </summary>
        protected override void NewDataInPort()
        {
            switch (interruptState)
            {
                case InterruptState.Standby:
                    currentInterrupt = new InterruptInfo() { interruptType = InPort };
                    interruptState = InterruptState.WaitForDeviceId;
                    break;

                case InterruptState.WaitForDeviceId:
                    currentInterrupt.deviceId = InPort;
                    switch (systemBoard[InPort].Type)
                    {
                        case HardwareType.CPU:
                        case HardwareType.ALU:
                        case HardwareType.FPU:
                        case HardwareType.HumanInterfaceDevice:
                            interruptState = InterruptState.WaitForObjectId;
                            break;

                        default:
                            interruptState = InterruptState.Standby;
                            break;
                    }
                    break;

                case InterruptState.WaitForObjectId:
                    currentInterrupt.objectId = InPort;
                    if (systemBoard[currentInterrupt.deviceId].Type == HardwareType.HumanInterfaceDevice)
                        interruptState = InterruptState.WaitForObjectState;
                    else
                        interruptState = InterruptState.Standby;
                    break;

                case InterruptState.WaitForObjectState:
                    currentInterrupt.objectState = InPort;
                    interruptState = InterruptState.Standby;
                    break;
            }

            ClearInPort();
        }
    }
}
