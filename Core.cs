using MelonLoader;
using HarmonyLib;
using static Warfare;
using System.Security.AccessControl;

[assembly: MelonInfo(typeof(warfare_cheats.Core), "warfare-cheats", "1.0.0", "marinesciencedude", "https://github.com/marinesciencedude/Warfare-Legacy-Collection-Modding/tree/warfare-cheats")]
[assembly: MelonGame("Armor Games Studios", "Warfare")]
namespace warfare_cheats;

public class Core : MelonMod
{
    public override void OnInitializeMelon()
    {
        HarmonyLib.Harmony harmony = this.HarmonyInstance;

        harmony.PatchAll();

        LoggerInstance.Msg("Initialized.");
    }
}

[HarmonyPatch(typeof(Warfare), nameof(Warfare.GetSquadTypeInfo))]
class WarfareGetSquadTypeInfo
{
    public static void Postfix(ref SquadTypeInfo __result, Game game, SquadType type)
    {
        //SquadTypeInfo value = __result;
        __result.resourcePointCost = 0;
        if(game == 0 && type == SquadType.Tank) //Make sure enough cooldown so Tanks in Warfare 1917 have a rolling-start, also they deadlock each other if instantly spawned - which is also the case in Warfare 1944 but that will require lane-checking to address
            __result.resupplySecs = 12.5f;
        else
            __result.resupplySecs = 0f;
        //__result = value;
    }
}

[HarmonyPatch(typeof(ButtonFireSupport), nameof(ButtonFireSupport.LockControlForTimeout))]
class ButtonFireSupportLockControlForTimeout
{
    public static void Prefix(ref float timeoutDurationScalar)
    {
        timeoutDurationScalar = 0f;
    }
}

[HarmonyPatch(typeof(CommandButton), nameof(CommandButton.CommandCost))]
class CommandButtonCommandCost
{
    public static bool Prefix(ref int __result)
    {
        __result = 0;
        return false;
    }
}