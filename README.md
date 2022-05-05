# AVR32
ITC Forth in C for the AVR 32u4 with 32 bit stacks, 16 bit program memory. Porting ARM code to AVR. The VM is written with Arduino Wiring, and Forth core and app are written using a target compiler written in gforth. The Forth compiler's output is a memory array that's used when compiling the VM. I hope that makes sense.

# How the Keyboard Firmware Works
We'll start with explaining the top level file, main.fs, where the source code for the keyboard firmware is.

line 1 
**\ main.fs**
That's a comment with the name of the file in it.

Line 2
**target**
changes search order so that target words are found first. The word "host" causes host words, which are compiler words, to be found first.

Lines 4-7 define three constants, used to initialize the IO ports. A constant is a word (function) which pushes a number on to the data stack when executed.

