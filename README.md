# Warfare Legacy Collection Modding

A set of mods for the [Warfare Legacy Collection](https://store.steampowered.com/app/2745870/Warfare_Legacy_Collection/)

# Permanent Corpses

Stops soldiers' bodies from disappearing after a time, like how craters never disappear during the match.

![warfare-permanent-corpses](https://github.com/user-attachments/assets/92aecbea-d837-4ff1-8556-4636d3fc569d)

https://www.youtube.com/watch?v=YlXqJ85Dltg

### Installation
* Set up [MelonLoader](https://melonloader.co/)
* Move warfare_permanent_corpses.dll to the Mods folder in the game directory

### Building
* The game's installation directory is assumed to be at C:\Program Files (x86)\Steam\steamapps\common\Warfare Legacy Collection, if it isn't then you'll need to change Assembly references and the PostBuildEvent (see .csproj files)
* Building is set up to automatically copy the .dll to the game directory's Mods folder and the launch profile runs a Steam command (in warfare.bat) to automatically play the game
* Set-up for the demo is provided, however for this mod it doesn't seem to be necessary to build separate versions