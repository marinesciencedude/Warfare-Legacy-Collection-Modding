# Warfare Legacy Collection Modding

A set of mods for the [Warfare Legacy Collection](https://store.steampowered.com/app/2745870/Warfare_Legacy_Collection/)

# Disable Morale

Stops morale from depleting, preventing that victory/loss condition from ever being triggered

### Installation
* Set up [MelonLoader](https://melonloader.co/)
* Move warfare_morale_disable.dll to the Mods folder in the game directory

### Building
* The game's installation directory is assumed to be at C:\Program Files (x86)\Steam\steamapps\common\Warfare Legacy Collection, if it isn't then you'll need to change Assembly references and the PostBuildEvent (see .csproj files)
* Building is set up to automatically copy the .dll to the game directory's Mods folder and the launch profile runs a Steam command (in warfare.bat) to automatically play the game
* Set-up for the demo is provided, however for this mod it doesn't seem to be necessary to build separate versions