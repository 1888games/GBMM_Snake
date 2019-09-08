# GBMM_Snake

Snake written for Look Mum No Computer's Gameboy Megamachine 8x6 display in Unity3D.

As well as simulating the Game Boy screens, it outputs to combinations of midi channels 1, 15, and 16.

Should be fairly easy to adapt it to run other games. Just switch the pixels/screens on or off and use the 'GetPixelName' method in ScreenController to read what a given pixel represents (i.e. SnakeHead, SnakeTail, Food, None...)

Runs on Windows only unless someone can find some free OSX midi drivers that work with Unity....


