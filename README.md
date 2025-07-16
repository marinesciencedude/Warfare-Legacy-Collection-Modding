# Warfare Legacy Collection Modding

A set of mods for the [Warfare Legacy Collection](https://store.steampowered.com/app/2745870/Warfare_Legacy_Collection/)

## Halt in place!

Adds the cut/unused Halt command to Warfare 1944.

![HaltButton](https://github.com/user-attachments/assets/88e4db28-1a1d-4503-81f8-ee2d778cd3db)

This mod adds limitations to the Halt command in order to better fit the balance of the game.

Squads can only halt a certain distance from cover:

![DistanceToCover](https://github.com/user-attachments/assets/aee8882a-d8c2-44f0-b42e-f47c9b91fd0f)

Squads can only halt a certain distance from other halted squads:

![DistanceBetweenSquads](https://github.com/user-attachments/assets/54fbd446-9628-4231-b285-64e4b1adfa97)

### Installation
* Set up [MelonLoader](https://melonloader.co/)
* Move warfare_halt.dll to the Mods folder in the game directory

### Building
* The game's installation directory is assumed to be at C:\Program Files (x86)\Steam\steamapps\common\Warfare Legacy Collection, if it isn't then you'll need to change Assembly references and the PostBuildEvent (see .csproj files)
* Building is set up to automatically copy the .dll to the game directory's Mods folder and the launch profile runs a Steam command (in warfare.bat) to automatically play the game
* Set-up for the demo is provided, this is required due to the different squad commands list set-ups in Warfare 1944