using MelonLoader;
using HarmonyLib;
using System.Reflection.Emit;
using static HarmonyLib.Code;
using static Soldier;
using static BattleController;
using static Warfare;
using static UnityEngine.GraphicsBuffer;
using System.Diagnostics;
using UnityEngine;

[assembly: MelonInfo(typeof(warfare_resolution_unlock.Core), "warfare-resolution-unlock", "1.0.0", "marinesciencedude", "https://github.com/marinesciencedude/Warfare-Legacy-Collection-Modding/tree/warfare-resolution-unlock")]
[assembly: MelonGame("Armor Games Studios", "Warfare")]
namespace warfare_resolution_unlock;

public class Core : MelonMod
{
    public override void OnInitializeMelon()
    {
        HarmonyLib.Harmony harmony = this.HarmonyInstance;

        harmony.PatchAll();

        LoggerInstance.Msg("Initialized.");
    }
}

[HarmonyPatch(typeof(PlayerOptionsController), "ReloadResolutions")]
class PlayerOptionsControllerReloadResolutions
{
    public static bool Prefix(ref List<Vector2Int> ___resolutions)
    {
        /*float subMinAspectRatio = 1.3333334f;
        float maxAspectRatio = 2.3333333f;*/
        ___resolutions = (from resolution in (from resolution in Screen.resolutions
                                           select new Vector2Int(resolution.width, resolution.height)/* into resolution
                                           where IsValidResolution(resolution)
                                           select resolution*/).Distinct()
                       orderby resolution.x * 100000 + resolution.y
                       select resolution).ToList();
        /*bool IsValidResolution(Vector2Int resolution)
        {
            float num = (float)resolution.x / (float)resolution.y;
            bool flag = num > subMinAspectRatio && num <= maxAspectRatio;
            Debug.Log($"IsValidResolution({resolution}) with ratio {num} returns {flag}");
            return flag;
        }*/
        return false;
    }
}