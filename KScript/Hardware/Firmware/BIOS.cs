using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Hardware.Firmware
{
    public class BIOS
    {
        internal static string POST = @"; POST
#define string unsigned byte array
#define IO.CPU 3
#define IO.Video 7
#define IO.Sound 9

; unsigned byte array helloworld ""Hello World!\0""
string intro ""KScript BIOS v1.0\n\n\0""
string mainProcessor ""Main Processor: \0""
string memoryTesting ""Memory Testing: \0""
string detectingDrives ""Detecting Drives:\n\0""
string 12 cpuName

main:
	; clear the registers
	mov a 0
	mov b 0
	mov c 0
	mov d 0
	
	; we don't have an FPU yet
	; mov e 0
	; mov f 0
	
	; check that they are all zero
	pushi 1
	pushi 0
	pushr a
	equal
	pushi 0
	pushr b
	equal
	pushi 0
	pushr c
	equal
	pushi 0
	pushr d
	equal
	equal
	equal
	equal
	equal
	peeki a
	pushr a
	jfalse badcpu
	jtrue checkok
	
loop:
	hlt
	goto loop

; repeating long beeps	
badcpu:
	mov a 128
	outp 0
	mov a 500
	outp IO.Sound
	goto badcpu
	
checkok:
	mov a 0
	outp 0
	mov a 250
	outp IO.Sound
	goto printInfo
	
printInfo:
	; print helloworld
    jmp clearscreen
	movp b intro
	jmp printstr
	movp b mainProcessor
	jmp printstr
	
	; print CPU name
	pusha
	cpuid 0
	movp a cpuName
	wmemr a b
	pushi 4
	pushi 4
	pushr a
	add
	popi a
	wmemr a c
	pushr a
	add
	popi a
	wmemr a d
	popa
	
	movp b cpuName
	jmp printstr
	
	jmp newline
	
	movp b memoryTesting
	jmp printstr
	
	; TODO: Add memory testing
	; TODO: Add drive detection
	; TODO: Add device detection
	; TOOD: Add load/jump to bootstrap
	
	goto loop
	
printchar:
    wvmemr c a
	
	; increment the VRAM pointer
	incrr c
	ret
	
; prints a string
; character is located in A
; memory address is located in B
; VRAM address is located in C
printstr:
	; get the character. It's the first byte in the word, so we'll have to get rid of the rest of the word.
	rmemr a b
	pushr a
	pushi 24
	lsh
	pushi 24
	rsh
	popi a
	
	; check if the character is null. if it is, return
	pushr a
	pushi 0
	equal
	rettrue
	
	; check if the character is new line
	pushr a
	pushi 10
	equal
	jfalse printchar
	jtrue newline
	
	; increment the memory pointer
	incrr b
	goto printstr
	
newline:
	pushr c
	pushi 80
	div
	pushi 1
	add
	pushi 80
	mult
	popi c
	ret
	
clearscreen:
	mov c 0
clearscreen_loop:
	mov a 32
	jmp printchar
	pushr c
	incr
	peeki c
	pushi 2000
	less
	gototrue clearscreen_loop
	mov c 0
	ret

interrupt:
	inp IO.CPU
	ret
";
        public static string POSTSource { get { return POST; } }

        public static string[] Errors { get; set; }
        public static string[] Warnings { get; set; }
    }
}
