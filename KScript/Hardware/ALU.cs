using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KScript.Interrupts;

namespace KScript.Hardware
{
    /// <summary>
    /// Performs 32-bit integer arithmetic and logic operations.
    /// </summary>
    public class ALU : Hardware
    {
        /// <summary>
        /// Stack for ALU values
        /// </summary>
        Stack<int> dataStack;

        Instructions instruction;

        /// <summary>
        /// Gets the current ALU stack as an array.
        /// </summary>
        public int[] Stack { get { return dataStack.ToArray(); } }

        /// <summary>
        /// Instantiates the ALU.
        /// </summary>
        public ALU() : base(HardwareType.ALU)
        {
            dataStack = new Stack<int>();
            instruction = Instructions._null;
        }

        /// <summary>
        /// Pushes the value onto the stack.
        /// </summary>
        /// <param name="value"></param>
        public void Push(int value)
        {
            dataStack.Push(value);
        }

        /// <summary>
        /// Pops the value from the stack and returns it. If the stack is empty, an ArithmeticStackUnderflow interrupt is raised.
        /// </summary>
        /// <returns></returns>
        public int Pop()
        {
            if (dataStack.Count == 0)
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
                return 0;
            }
            else
            {
                return dataStack.Pop();
            }
        }

        /// <summary>
        /// Peeks the value from the stack and returns it. If the stack is emopty, an ArithmeticStackUnderflow interrupt is raised.
        /// </summary>
        /// <returns></returns>
        public int Peek()
        {
            if (dataStack.Count == 0)
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
                return 0;
            }
            else
            {
                return dataStack.Peek();
            }
        }

        public void Add()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                Push(a + b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Subtract()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                Push(a - b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Multiply()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                Push(a * b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Divide()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                if (b == 0) 
                    RaiseError(HardwareInterruptType.DivideByZero);
                else
                    Push(a / b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Modulus()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                if (b == 0)
                    RaiseError(HardwareInterruptType.DivideByZero);
                else
                    Push(a % b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Increment()
        {
            if (dataStack.Count >= 1)
            {
                int a = Pop();
                Push(a + 1);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Decrement()
        {
            if (dataStack.Count >= 1)
            {
                int a = Pop();
                Push(a - 1);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void And()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                Push(a & b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Or()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                Push(a | b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Not()
        {
            if (dataStack.Count >= 2)
            {
                int a = Pop();
                Push(~a);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Negative()
        {
            if (dataStack.Count >= 2)
            {
                int a = Pop();
                Push(-a);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Xor()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                Push(a ^ b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void LeftShift()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                Push(a << b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void RightShift()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                Push(a >> b);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void Equal()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                if (a == b)
                    Push(1);
                else
                    Push(0);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void NotEqual()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                if (a != b)
                    Push(1);
                else
                    Push(0);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void GreaterThan()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                if (a > b)
                    Push(1);
                else
                    Push(0);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void LessThan()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                if (a < b)
                    Push(1);
                else
                    Push(0);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void GreaterThanOrEqual()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                if (a >= b)
                    Push(1);
                else
                    Push(0);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        public void LessThanOrEqual()
        {
            if (dataStack.Count >= 2)
            {
                int b = Pop();
                int a = Pop();
                if (a <= b)
                    Push(1);
                else
                    Push(0);
            }
            else
            {
                RaiseError(HardwareInterruptType.ArithmeticStackUnderflow);
            }
        }

        protected override void NewDataInPort()
        {
            if ((instruction == Instructions.pushi) | (instruction == Instructions.pushr))
            {
                Push(InPort);
                instruction = Instructions._null;
            }
            else
                instruction = (Instructions)InPort;

            ExecuteInstruction();
            ClearInPort();
        }

        private void ExecuteInstruction()
        {
            switch (instruction)
            {
                case Instructions.popx:
                    Pop();
                    break;

                case Instructions.popi:
                    WriteToOut(Pop());
                    break;

                case Instructions.peeki:
                    WriteToOut(Peek());
                    break;

                case Instructions.add:
                    Add(); break;

                case Instructions.sub:
                    Subtract(); break;

                case Instructions.mult:
                    Multiply(); break;

                case Instructions.div:
                    Divide(); break;

                case Instructions.mod:
                    Modulus(); break;

                case Instructions.and:
                    And(); break;

                case Instructions.or:
                    Or(); break;

                case Instructions.not:
                    Not(); break;

                case Instructions.xor:
                    Xor(); break;

                case Instructions.neg:
                    Negative(); break;

                case Instructions.lsh:
                    LeftShift(); break;

                case Instructions.rsh:
                    RightShift(); break;

                case Instructions.equal:
                    Equal(); break;

                case Instructions.nequal:
                    NotEqual(); break;

                case Instructions.great:
                    GreaterThan(); break;

                case Instructions.gequal:
                    GreaterThanOrEqual(); break;

                case Instructions.less:
                    LessThan(); break;

                case Instructions.lequal:
                    LessThanOrEqual(); break;

            }
        }

        protected override void _Start()
        {
            dataStack.Clear();
        }

        protected override void _Stop()
        {
            dataStack.Clear();
        }
    }
}
