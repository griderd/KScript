using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Hardware
{
    public enum Instructions : byte
    {
        _null,
        #region Registers
        rega,
        regb,
        regc,
        regd,
        regg,
        regh,
        rege,
        regf,
        #endregion
        #region Reserved
        res1,
        res2,
        res3,
        res4,
        res5,
        res6,
        res7,
        res8,
        #endregion

        #region ALU instructions
        /// <summary>
        /// Pushes a value onto the ALU stack: pushi [literal]
        /// </summary>
        pushi,
        /// <summary>
        /// Pops a value from the ALU stack and stores it in a register: popi [register]
        /// </summary>
        popi,
        /// <summary>
        /// Pops a value from the ALU stack, but doesn't store it anywhere.
        /// </summary>
        popx,
        /// <summary>
        /// Peeks a value from the ALU stack: peeki [register]
        /// </summary>
        peeki,
        /// <summary>
        /// Pushes a value onto the ALU stack from a register: pushr [register]
        /// </summary>
        pushr,
        /// <summary>
        /// Clears the ALU stack
        /// </summary>
        cls,
        /// <summary>
        /// Adds the top two values on the ALU stack: add
        /// </summary>
        add,
        /// <summary>
        /// Subtracts the top two values on the ALU stack: sub
        /// </summary>
        sub,
        /// <summary>
        /// Multiplies the top two values on the ALU stack: mult
        /// </summary>
        mult,
        /// <summary>
        /// Divides the top two values on the ALU stack: div
        /// </summary>
        div,
        /// <summary>
        /// Performs modulus on the top two values of the ALU stack: mod
        /// </summary>
        mod,
        /// <summary>
        /// Performs a bitwise-AND on the top two values of the ALU stack: and
        /// </summary>
        and,
        /// <summary>
        /// Increments the value at the top of the stack by 1.
        /// </summary>
        incr,
        /// <summary>
        /// Decrements the value at the top of the stack by 1.
        /// </summary>
        decr,
        /// <summary>
        /// Performs a bitwise-OR on the top two values of the ALU stack: or
        /// </summary>
        or,
        /// <summary>
        /// Performs a bitwise-NOT on the value at the top of the ALU stack: not
        /// </summary>
        not,
        /// <summary>
        /// Performs a bitwise-XOR on the top two values of the ALU stack: xor
        /// </summary>
        xor,
        /// <summary>
        /// Negates the value at the top of the ALU stack: neg
        /// </summary>
        neg,
        /// <summary>
        /// Performs a left bitshift: lsh
        /// </summary>
        lsh,
        /// <summary>
        /// Performs a right bitshift: rsh
        /// </summary>
        rsh,
        /// <summary>
        /// Compares the top two values for equality: equal
        /// </summary>
        equal,
        /// <summary>
        /// Compares the top two values for inequality: nequal
        /// </summary>
        nequal,
        /// <summary>
        /// Compares the top two values for greater-than: great
        /// </summary>
        great,
        /// <summary>
        /// Compares the top two values for greater-than or equality: gequal
        /// </summary>
        gequal,
        /// <summary>
        /// Compares the top two values for less-than: less
        /// </summary>
        less,
        /// <summary>
        /// Compares the top two values for less-than or equality: lequal
        /// </summary>
        lequal,
        #endregion
        #region CPU Operations
        /// <summary>
        /// Jumps if the value at the top of the ALU stack is non-zero: jtrue [label|address]
        /// </summary>
        jtrue,
        /// <summary>
        /// Jumps if the value at the top of the ALU stack is zero: jfalse [label|address]
        /// </summary>
        jfalse,
        /// <summary>
        /// Unconditional jump: jmp [label|address]
        /// </summary>
        jmp,
        /// <summary>
        /// Performs a jump without pushing to the call stack: goto [label|address]
        /// </summary>
        _goto,
        /// <summary>
        /// Performs a jump without pushing to the call stack when true: gototrue [label|address]
        /// </summary>
        gototrue,
        /// <summary>
        /// Performs a jump without pushing to the call stack when false: gotofalse [label|address]
        /// </summary>
        gotofalse,
        /// <summary>
        /// Pops the value from the call stack and goes to that location: ret
        /// </summary>
        ret,
        /// <summary>
        /// Outputs the content of register A to the provided port: outp [portID]
        /// </summary>
        outp,
        /// <summary>
        /// Outputs the content of register A to the port in register B: outr
        /// </summary>
        outr,
        /// <summary>
        /// Inputs the value at the provided port to register A: inp [portID]
        /// </summary>
        inp,
        /// <summary>
        /// Inputs the value at the port in register B to register A: inr
        /// </summary>
        inr,
        /// <summary>
        /// Writes a literal into a register: mov [register] [literal]
        /// </summary>
        mov,
        /// <summary>
        /// Copies a register into another register: mov [destRegister] [srcRegister]
        /// </summary>
        movr,
        /// <summary>
        /// Copies a pointer or address into a register: movp [register] [symbol|address]
        /// </summary>
        movp,
        /// <summary>
        /// Pushes the content of a register onto the stack: push [register]
        /// </summary>
        push,
        /// <summary>
        /// Pops the top value from the stack and stores it in a register: pop [register]
        /// </summary>
        pop,
        /// <summary>
        /// Pops the top value from the stack and gets rid of it.
        /// </summary>
        poprx,
        /// <summary>
        /// Pushes the content of all the registers onto the stack: pusha
        /// </summary>
        pusha,
        /// <summary>
        /// Pops the content of all of the registers from the stack: popa
        /// </summary>
        popa,
        /// <summary>
        /// Reads a value from memory and stores it in a register: rmem [register] [symbol|address]
        /// </summary>
        rmem,
        /// <summary>
        /// Writes a value to memory from a register [symbol|address] [register]
        /// </summary>
        wmem,
        /// <summary>
        /// Writes a literal to memory: wmem [symbol|address] [literal]
        /// </summary>
        wmeml,
        /// <summary>
        /// Reads the memory at an address stored in a register and writes the value to a register: rmemr [destRegister] [addressRegister]
        /// </summary>
        rmemr,
        /// <summary>
        /// Writes the value in a given register to the memory address in another register: wmemr [addressRegister] [srcRegister]
        /// </summary>
        wmemr,
        /// <summary>
        /// Writes a literal to video memory: wvmem [addressRegister] [literal]
        /// </summary>
        wvmem,
        /// <summary>
        /// Writes a register to video memory: wvmemr [addressRegister] [srcRegister]
        /// </summary>
        wvmemr,
        /// <summary>
        /// Sets the interrupt handler pointer: inthnd [symbol|address]
        /// </summary>
        inthnd,
        /// <summary>
        /// Stops the CPU.
        /// </summary>
        hlt,
        /// <summary>
        /// Returns if true: rettrue
        /// </summary>
        rettrue,
        /// <summary>
        /// Returns if false: retfalse
        /// </summary>
        retfalse,
        /// <summary>
        /// Returns information related to the CPU.
        /// </summary>
        cpuid,
        /// <summary>
        /// Increments the register provided by 1. incrr [register]
        /// </summary>
        incrr,
        /// <summary>
        /// Decrements the register provided by 1. decrr [register]
        /// </summary>
        decrr,
        /// <summary>
        /// Jumps if the value at the top of the ALU stack is non-zero: jtrue [pointer]
        /// </summary>
        jtruer,
        /// <summary>
        /// Jumps if the value at the top of the ALU stack is zero: jfalse [pointer]
        /// </summary>
        jfalser,
        /// <summary>
        /// Unconditional jump: jmp [pointer]
        /// </summary>
        jmpr,
        /// <summary>
        /// Performs a jump without pushing to the call stack: goto [pointer]
        /// </summary>
        gotor,
        /// <summary>
        /// Performs a jump without pushing to the call stack when true: gototrue [pointer]
        /// </summary>
        gototruer,
        /// <summary>
        /// Performs a jump without pushing to the call stack when false: gotofalse [pointer]
        /// </summary>
        gotofalser
        #endregion
    }


}
