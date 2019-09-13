Game engine written in Unity for Look Mum No Computer's 8x6 Gameboy Megamachine display (later to be 8x8).

It simulates the Gameboy displays as well as outputting the appropriate midi notes to drive the actual display (16), mini-display (15) and machine itself (1). MIDI output only works on Windows at the moment, unless someone can find a free MIDI driver for OSX that works with Unity.

Currently this has Snake and Tetris implemented.

Easy to add other games, just program your logic in a monobehaviour script and drive the pixels using ScreenController.


