word port 6
byte value 65

main:
	jmp output
	hlt

input:
	rmem b port
	inr
	push a
	ret
	
output:
	rmem b port
	rmem a value
	outr
	ret