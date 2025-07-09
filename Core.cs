using MelonLoader;
using HarmonyLib;
using System.Reflection.Emit;
using static HarmonyLib.Code;
using static Soldier;
using static BattleController;

[assembly: MelonInfo(typeof(warfare_permanent_corpses.Core), "warfare-permanent-corpses", "1.0.0", "marinesciencedude", "https://github.com/marinesciencedude/Warfare-Legacy-Collection-Modding/tree/warfare-permanent-corpses")]
[assembly: MelonGame("Armor Games Studios", "Warfare")]
namespace warfare_permanent_corpses;

public class Core : MelonMod
{
    public override void OnInitializeMelon()
    {
        HarmonyLib.Harmony harmony = this.HarmonyInstance;

        harmony.Patch(
            original: AccessTools.Method(typeof(Soldier), nameof(Soldier.Die)),
            transpiler: new HarmonyMethod(typeof(SoldierDie), nameof(SoldierDie.Transpiler))
        );

        LoggerInstance.Msg("Initialized.");
    }
}

[HarmonyPatch(typeof(Soldier), nameof(Soldier.Die))]
class SoldierDie
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
    {
        CodeMatcher matcher = new(instructions, gen);
        matcher.MatchStartForward(
            //battleController.OnSoldierDied(this, lethalDamage);
            Callvirt[AccessTools.Method(typeof(BattleController), nameof(BattleController.OnSoldierDied))]
            )
            .ThrowIfNotMatch($"Could not find entry point into Soldier::Die")
            .Advance(1)
            .RemoveInstructions(5)
                                    //Remove:
                                    /*StartCoroutine(DoDeathDance());
                                    IEnumerator DoDeathDance()
                                    {
                                        yield return new WaitForSeconds(6f);
                                        yield return Async.LerpAsync(1f, 0f, 10f, delegate(float alpha)
                                        {
                                            Opacity = alpha;
                                        });
                                        Object.Destroy(base.gameObject);
                                    }*/
            .Insert(
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop)
            );
                                    

        return matcher.InstructionEnumeration();
    }
}