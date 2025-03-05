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
* Navigate to your game installation (Browse Local Files on Steam)
* Make a backup of the Assembly-CSharp.dll file, usually found in your installation at Warfare_Data\Managed
* Use [xdelta](https://github.com/jmacd/xdelta-gpl) or a UI frontend to apply the Assembly-CSharp.xdelta patch to Assembly-CSharp.dll

### Building
* Use [ILSpy](https://github.com/icsharpcode/ILSpy) or a similar tool to decompile Assembly-CSharp.dll
* Either
    * Apply .xdelta patches to each decompiled .cs file
	* Use patch.exe from e.g. your Git installation to apply Mod.Patch on all files in the directory
* Build a new Assembly-CSharp.dll from the patched .cs files, you can do this with the Visual Studio Project that ILSpy provides

### Contributing
* Note that you need to use dos2unix after creating a patch with diff.exe otherwise in Windows it will not work with patch.exe