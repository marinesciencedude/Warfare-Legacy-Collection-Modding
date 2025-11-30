# Warfare Legacy Collection Modding

A set of mods for the [Warfare Legacy Collection](https://store.steampowered.com/app/2745870/Warfare_Legacy_Collection/)

# Warfare Legacy Collection 'Hacked'

Based on the 'Hacked' versions of the original Flash Games, this mod sets resupply time and resource point costs to zero for all units, fire support, command actions (grenades, officer actions etc.)

NOTE: Tanks in Warfare 1917 have a 12.5s cooldown to ensure they spawn in with a rolling start, this also addresses an issue where tanks would block each other from moving forward  
WARNING: Tanks in Warfare 1944 have instant cooldowns like all other units and WILL deadlock each other from moving forward if spamming too quickly in the same lane

### Installation
* Set up [MelonLoader](https://melonloader.co/)
* Move warfare_cheats.dll to the Mods folder in the game directory

### Building
* The game's installation directory is assumed to be at C:\Program Files (x86)\Steam\steamapps\common\Warfare Legacy Collection, if it isn't then you'll need to change Assembly references and the PostBuildEvent (see .csproj files)
* Building is set up to automatically copy the .dll to the game directory's Mods folder and the launch profile runs a Steam command (in warfare.bat) to automatically play the game
* Set-up for the demo is provided, however for this mod it doesn't seem to be necessary to build separate versions