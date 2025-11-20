# Warfare Legacy Collection Modding

A set of mods for the [Warfare Legacy Collection](https://store.steampowered.com/app/2745870/Warfare_Legacy_Collection/)

# Unlock Resolution Options

Removes display resolution validation checks so that you can e.g. play above 720p on 1280×1024

UI elements on the main menu **will** get cut off

<img width="1280" height="1024" alt="1280_1024_ingame" src="https://github.com/user-attachments/assets/7af70208-ec63-4221-9556-53530125a373" />
<img width="1280" height="1024" alt="1280_1024_custombattle" src="https://github.com/user-attachments/assets/7d4a7380-7d7f-4727-be1a-d235a599943a" />

This mod was made to address an issue raised here: https://steamcommunity.com/app/2745870/discussions/0/680734492841776051/

### Installation
* Set up [MelonLoader](https://melonloader.co/)
* Move warfare_resolution_unlock.dll to the Mods folder in the game directory

### Building
* The game's installation directory is assumed to be at C:\Program Files (x86)\Steam\steamapps\common\Warfare Legacy Collection, if it isn't then you'll need to change Assembly references and the PostBuildEvent (see .csproj files)
* Building is set up to automatically copy the .dll to the game directory's Mods folder and the launch profile runs a Steam command (in warfare.bat) to automatically play the game
* Set-up for the demo is provided, however for this mod it doesn't seem to be necessary to build separate versions