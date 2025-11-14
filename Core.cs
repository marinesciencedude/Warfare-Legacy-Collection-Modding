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

[assembly: MelonInfo(typeof(warfare_morale_disable.Core), "warfare-morale-disable", "1.0.0", "marinesciencedude", "https://github.com/marinesciencedude/Warfare-Legacy-Collection-Modding/tree/warfare-morale-disable")]
[assembly: MelonGame("Armor Games Studios", "Warfare")]
namespace warfare_morale_disable;

public class Core : MelonMod
{
    public override void OnInitializeMelon()
    {
        HarmonyLib.Harmony harmony = this.HarmonyInstance;

        harmony.PatchAll();

        LoggerInstance.Msg("Initialized.");
    }
}

[HarmonyPatch(typeof(BattleController), nameof(BattleController.OnSoldierDied))]
class BattleControllerOnSoldierDied
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
    {
        CodeMatcher matcher = new(instructions, gen);
        matcher.MatchStartForward(
            //BattleStats.IncrementArmyStat(squad.army, "SoldiersLost");
            Ldstr["SoldiersLost"],
            Callvirt[AccessTools.Method(typeof(PlayStats), nameof(PlayStats.IncrementArmyStat))]
            )
            .ThrowIfNotMatch($"Could not find entry point into BattleController::OnSoldierDied")
            .Advance(3)
                                    //Remove:
                                    /*foreach (KeyValuePair<Warfare.Army, Morale> armyMorale in armyMorales)
		                            {
			                            armyMorale.Value.OnSoldierDied(soldier, lethalDamage);
		                            }*/
            .RemoveInstructions(21)
            .Insert(
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop),
                 new CodeInstruction(OpCodes.Nop)
            );

        return matcher.InstructionEnumeration();
    }
    //ideally remove the entire foreach (KeyValuePair<Warfare.Army, Morale> armyMorale in armyMorales)
    //but dealing with IL keywords .try and finally is too much work for now
}

[HarmonyPatch(typeof(BattleController), "CheckUpperHand")]
class BattleControllerCheckUpperHand
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
    {
        CodeMatcher matcher = new(instructions, gen);
        matcher.MatchStartForward(
            //morale2.OnHadUpperHand();
            Callvirt[AccessTools.Method(typeof(Morale), nameof(Morale.OnHadUpperHand))]
            )
            .ThrowIfNotMatch($"Could not find entry point into BattleController::OnSoldierDied")
            .Advance(1)
            .RemoveInstructions(2)
                                    //Remove:
                                    /*morale2.OnHadLowerHand();*/
            .Insert(
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop)
            );

        return matcher.InstructionEnumeration();
    }
    //could remove all instances of morale2 for alleged performance purposes but not critical
}

[HarmonyPatch(typeof(BattleController), "OnSquadDidFlank")]
class BattleControllerOnSquadDidFlank
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
    {
        CodeMatcher matcher = new(instructions, gen);
        matcher.MatchStartForward(
            // num *= abilities.GetAbilityBonus(squad.army, "SecureFlanks");
            Callvirt[AccessTools.Method(typeof(PlayerAbilities), nameof(PlayerAbilities.GetAbilityBonus))]
            )
            .ThrowIfNotMatch($"Could not find entry point into BattleController::OnSquadDidFlank")
            .Advance(4)
            //don't delete an instruction that brtrue.s is pointing to, hands are tied and cannot remove OnMoraleChanged();
            .SetInstructionAndAdvance(
                new CodeInstruction(OpCodes.Nop)
             )
            .RemoveInstructions(5)
                                    //Remove:
                                    /*armyMorales[army].OnEnemySquadDidFlank(num);
                                    OnMoraleChanged();*/
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