# Mnemonics 2.0

01111111 11111111 11111111 11111111

This is the current plan for replacing the instruction set from the ground up. I also plan on adding more registers, loosely based on the x86 architecture.

|  Code   | Bytecode   | Description                                                       |                        Example                                 |
|---------|------------|-------------------------------------------------------------------|----------------------------------------------------------------|
| END     | 0x01       | Indicates the end of the program, followed by start point label                      | `END START`
| LOD     | 0x02, 0x03 | Loads a hard-coded value into the given register                                     | `LOD ,X #0xFF00` <br> `LOD ,X ,Y`
| STM     | 0x04, 0x05 | Stores a literal value in RAM at the given location                                  | `STM #100 #0xA000` <br> `STM #100 ,A`
| STOR    | 0x06, 0x07 | Stores a register in RAM at the location given by another register                   | `STOR ,A #0xA000` <br> `STOR ,A ,X`
| CMP     | 0x08, 0x09 <br> 0x0A, 0x0B| Compares the value in a register to a literal                         | `CMP ,A #100` <br> `CMP ,A ,X`
| JMP     | 0x0C       | Moves instruction pointer to the given label                       				    | `JMP #Loop1`
| JEQ     | 0x0D       | Moves instruction pointer to the given label if comparison flag has equality set     | `JEQ #Loop1`
| JNE     | 0x0E       | Moves instruction pointer to the given label if comparison flag has inequality set   | `JNE #Loop1`
| JGT     | 0x0F       | Moves instruction pointer to the given label if comparison flag has greater than set | `JGT #Loop1`
| JLT     | 0x10       | Moves instruction pointer to the given label if comparison flag has less than set    | `JLT #Loop1`
| INC     | 0x11       | Increments the given register, setting overflow flag if required                     | `INC ,A`
| DEC     | 0x12       | Decrements the given register, setting overflow flag if required                     | `DEC ,A`
| ROL     | 0x13       | Permorms a left bitwise roll on the given register, setting carry flag if required   | `ROL ,B`
| ROR     | 0x14       | Permorms a right bitwise roll on the given register, setting carry flag if required  | `ROR ,A`
| ADDC    | 0x15, 0x16 | Adds the literal to the register if the carry flag is set                            | `ADDC ,X #0x000F` <br> `ADDC ,Y ,X`

| NOT     | 0x17       | Performs a bitwise inverse on the given register                                     | `NOT ,A`
| AND     | 0x18, 0x19 | Performs a bitwise and on the register with the given value                          | `AND ,B 0xFF` <br> `AND ,B ,A`

| OR      | 0x1A, 0x1B | Performs a bitwise or on the register with the given value                           | `OR ,B 0xFF` <br> `OR ,B ,A`
| XOR     | 0x1C, 0x1D | Performs a bitwise xor on the register with the given value                          | `XOR ,B 0xFF` <br> `XOR ,B ,A`
| ADD     | 0x1E, 0x1F | Adds a value to the given register, setting the overflow flag if required            | `ADD ,X #0xFF` <br> `ADD ,X ,Y`
| ADDS    | 0x20, 0x21 | Same as ADD, but stores result in a third register                                   | `ADDS ,X #0xFF ,Y` <br> `ADDS ,X ,Y, Z`
| SUB     | 0x22, 0x23 | Subtracts a value to the given register, setting the overflow flag if required       | `SUB ,X #0xFF` <br> `SUB ,X ,Y`
| SUBS    | 0x24, 0x25 | Same as SUB, but stores result in a third register                                   | `SUBS ,X #0xFF ,Y` <br> `SUBS ,X ,Y, Z`
| MUL     | 0x26, 0x27 | Multiplies a value to the given register, setting the overflow flag if required      | `MUL ,X #5` <br> `MUL ,X ,Y`
| MULS    | 0x28, 0x29 | Same as MUL, but stores result in a third register                                   | `MULS ,X #10 ,Y` <br> `MULS ,X ,Y, Z`
| DIV     | 0x2A, 0x2B | Divides a value to the given register, setting the overflow flag if required         | `DIV ,X #0x0F` <br> `DIV ,X ,Y`
| DIVS    | 0x2C, 0x2D | Same as DIV, but stores result in a third register                                   | `DIVS ,X #0x0F ,Y` <br> `DIVS ,X ,Y, Z`

| POLL    | 0x2E, 0x2F | Polls a device wil the given index and passes the result to the register             | `POLL #0x01 ,X` <br> `POLL ,A ,X`
| PASS    | 0x30, 0x31, <br> 0x32, 0x33 | Passes a value to a given device                                    | `PASS #0xFF #0x01` <br> `PASS ,A #0x01` <br> `PASS #0xFF ,A` <br> `PASS ,X ,A`
| PUSH    | 0x34, 0x35 | Pushes a value onto the stack														  | `PUSH #0x00FF` <br> `PUSH ,A`
| POP     | 0x36       | Pops a value off the stack and into a register                                       | `POP ,A`
| MOV     | 0x37       | Moves the value of the right register into the left register                         | `MOV ,X ,Y`

| RET     |            | Returns to the previous function, this must be followed by the size of local memory | `RET` <br> `RET #12` | * this will be implemented at assembly time