# Registers 2.0

### Note that this is still in the planning phase

This is the current plan for reworking the register system. Unlike the x86 architecture, there will not be any segment registers. All major registers will be 32 bits wide, 
which lets us access a lot more ram. There will be 6 integer registers, as well as 3 floating point registers and 2 processor-restricted registers. The remaining registers will be for program keeping. 
As well, some values will be accessible from within the assembler, including the instruction pointer and stack pointers. As well, not really a part of registers, but the processor flags will also be upgraded to 32 bits.

|  Register Name       | Width (bits) | Usage                                                       |
|----------------------|--------------|-------------------------------------------------------------|
| ***Indexes and Pointers*** |        |                                                             |
| IP                   |      32      | Stores the instruction pointer in RAM                       |
| SP                   |      32      | Stores the pointer to the end of the stack                  |
| BP                   |      32      | Stores the base position of the application's stack         |
| DI                   |      32      | Stores a destination index                                  |
| SI                   |      32      | Stores a source index                                       |
| ***General Use(Integer)*** |        | ***(Note that these are recommended uses)***                |
| IAX                  |      32      | Stores an accumulator value                                 |
| IBX                  |      32      | Stores memory addresses                                     |
| ICX                  |      32      | Stores counters                                             |
| IDX                  |      32      | Stores any data for program                                 |
| IEX                  |      32      | Stores any data for program                                 |
| IFX                  |      32      | Stores any data for program                                 |
| ***General Use(Floats)***  |        | ***(Note that these are recommended uses)***                |
| FAX                  |      32      | Stores any data for program                                 |
| FBX                  |      32      | Stores any data for program                                 |
| FCX                  |      32      | Stores any data for program                                 |
| *** Processor Restricted*** |       | ***(Note that these are hard-coded uses)***                 |
| RIA                  |      32      | Used for temporary storage for executing instructions       |
| RFA                  |      32      | Used for temporary storage for executing instruction        |