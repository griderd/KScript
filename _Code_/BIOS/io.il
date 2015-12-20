// io.il
// Handles I/O port data
// Don't use this for video output

function Input(int port) returns int
	asm
		rmem b port
		inr
		push a
		ret
	endasm
endfunction

function Output(int port, int value) returns void
	asm
		rmem b port
		rmem a value
		outr
		ret
	endasm
endfunction 