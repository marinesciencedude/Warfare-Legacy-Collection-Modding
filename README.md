# Warfare Legacy Collection Modding

A set of mods for the [Warfare Legacy Collection](https://store.steampowered.com/app/2745870/Warfare_Legacy_Collection/)

# Permanent Corpses

Stops soldiers' bodies from disappearing after a time, like how craters never disappear during the match.

![warfare-permanent-corpses](https://github.com/user-attachments/assets/92aecbea-d837-4ff1-8556-4636d3fc569d)

https://www.youtube.com/watch?v=YlXqJ85Dltg

### Installation
* Navigate to your game installation (Browse Local Files on Steam)
* Make a backup of the Assembly-CSharp.dll file, usually found in your installation at Warfare_Data\Managed
* Use [xdelta](https://github.com/jmacd/xdelta-gpl) or a UI frontend to apply the Assembly-CSharp.xdelta patch to Assembly-CSharp.dll

### Building
* Use [ILSpy](https://github.com/icsharpcode/ILSpy) or a similar tool to decompile Assembly-CSharp.dll
* Either
  * Apply .xdelta patches to each decompiled .cs file
  * Use patch.exe from e.g. your Git installation to apply Mod.Patch on all files in the directory
* Add Unity.VisualScripting.Core.dll as a dependency
* Build a new Assembly-CSharp.dll from the patched .cs files, you can do this with the Visual Studio Project that ILSpy provides

### Contributing
* Note that you need to use dos2unix after creating a patch with diff.exe otherwise in Windows it will not work with patch.exe