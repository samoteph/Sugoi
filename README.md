
# Project SUGOI

![img](https://github.com/samoteph/Sugoi/blob/master/Assets/Sugoi-GitHub.png)

The project SUGOI is virtual console written in C#.
The console allows the creation of 16/8 bits games very quickly and in a fun way.
It is crossplateform, thanks to .net standard, but there is only one platform supported at this point : UWP.
With this player you can published game for Windows and XBOX.

The UWP Player is written with Win2D. The sound part is explained here :
http://samuelblanchard.com/2019/11/05/lancez-du-son-comme-un-pro-en-uwp/

You can download the game 'Crazy Zone' made as a demo for the SUGOI:
https://www.microsoft.com/en-us/p/crazy-zone/9mv3vhk6z4jk

Here a video of the game:
https://www.youtube.com/watch?v=NmIdnYvIDK0

![img](https://github.com/samoteph/Sugoi/blob/master/Assets/Annotation%202020-06-08%20002631.jpg)
![img](https://github.com/samoteph/Sugoi/blob/master/Assets/Player1-1.png)
![img](https://github.com/samoteph/Sugoi/blob/master/Assets/Player2.png)

The source code contains the SUGOI libraries, 'Crazy Zone' and an empty game containing all the basics commands.
To start a new game or to learn how the SUGOI console works, try to explore the 'EmptyGame.Uwp' solution.

# What SUGOI can do

The SUGOI virtual console is able to do all you need to build a small 16/8 bits game.
In the 'EmptyGame.Uwp' you can learn how to :

![img](https://github.com/samoteph/Sugoi/blob/master/Assets/DemoCartridge.png)

- Make a simple game architecture
- Load assets into VideoMemory
- Draw sprite with horizontal and vertical flip)
- Draw tileSheet
- Draw map of tilesheet
- Scroll a map (infinite scrolling)
- Draw a rectangle
- Draw a text or an integer
- Animate a sprite
- Create a screen in a screen
- set a Clipping
- Manage of the gamepad

In the Crazy Zone game you can learn advanced scenario like :

- navigation by page
- creation of a crossplateform cartridge containing assets
- load tmx map as asset
- spritepool
- path for sprite
- play sound

# What SUGOI can't do

- Transparency (alpha is not managed at this point)
- Zoom
- Rotation
- 3D

You can join me on twitter @samoteph :) 

Enjoy :)
