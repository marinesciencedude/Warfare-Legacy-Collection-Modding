using MelonLoader;
using HarmonyLib;
using static HarmonyLib.Code;
using System.Reflection;
using SingularityGroup.HotReload;
using System.Reflection.Emit;
using Unity.VisualScripting;
using static Warfare;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using static BattleController;
using static Squad;
using static Soldier;
using static MelonLoader.MelonLogger;
using UnityEngine;
using Steamworks;
using System.Drawing;
using System.Diagnostics;
using System;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.Rendering;
using System.Collections;
using Unity.Properties;

[assembly: MelonInfo(typeof(warfare_halt.Core), "warfare-halt", "1.0.0", "marinesciencedude", "https://github.com/marinesciencedude/Warfare-Legacy-Collection-Modding/tree/warfare-halt")]
[assembly: MelonGame("Armor Games Studios", "Warfare")]
namespace warfare_halt;

public class Core : MelonMod
{
    public override void OnInitializeMelon()
    {
        HarmonyLib.Harmony harmony = this.HarmonyInstance;

        harmony.PatchAll();
        
        LoggerInstance.Msg("Initialized.");
    }
}

[HarmonyPatch(typeof(BattleController), nameof(BattleController.SpawnSquad))]
class BattleControllerSpawnSquad
{
    public static void Postfix(ref Squad __result)
    {
        Variables.Object(__result.gameObject)["halted"] = false;
    }
}

[HarmonyPatch(typeof(Squad), nameof(Squad.Halt))]
class SquadHalt
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.MatchStartForward(
            //First two lines of: activity = Activity.Halted;
            Ldarg_0,
            Ldc_I4_3
            )
            .ThrowIfNotMatch($"Could not find entry point into Squad::Halt")
            .Advance(-2)
            .RemoveInstructions(2) //Remove: StopAllCoroutines();
            .Insert(
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop)
            );

        return matcher.InstructionEnumeration();
    }

    public static void Postfix(Squad __instance)
    {
        for (int i = 0; i < __instance.transform.childCount; i++)
            __instance.transform.GetChild(i).GetComponent<Soldier>().SetPosture(Soldier.Posture.Standing, moving: false);
        //need to do instead of
        /*EachSoldier(delegate(Soldier soldier)
        {
            soldier.SetPosture(Soldier.Posture.Standing, moving: false);
        });*/
    }
}

[HarmonyPatch(typeof(Squad), "DoEngageEnemy", MethodType.Enumerator)]
class SquadDoEngageEnemy
{
    public static bool checkHalted(Squad instance) { return (bool)Variables.Object(instance.gameObject)["halted"]; }
    public static void setActivity(Squad instance) { Traverse.Create(instance).Property("activity").SetValue(Squad.Activity.Halted); }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator/*, Squad.Activity ___activity*/)
    {
        CodeMatcher matcher = new(instructions, generator);
        matcher.MatchStartForward(
            //MaybeSay(SoldierSpeech.SpeechType.EnemyDown, 5f);
            Ldloc_1,
            Ldc_I4_4,
            Ldc_R4[5.0f],
            Ldc_R4[0.6f],
            Call[AccessTools.Method(typeof(Squad), nameof(Squad.MaybeSay))]
        )
        .ThrowIfNotMatch($"Could not find entry point into Squad::DoEngageEnemy")
        .CreateLabel(out System.Reflection.Emit.Label label)
        .Insert(
          //if ((bool)Variables.Object(base.gameObject)["halted"])
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SquadDoEngageEnemy), nameof(checkHalted))),
            /*new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Soldier), nameof(Soldier.squad))),                                          //soldier.squad
            new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnityEngine.Component), nameof(UnityEngine.Component.gameObject))),     //squad.gameObject
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Variables), nameof(Variables.Object))),                                             //Variables.Object()
            new CodeInstruction(OpCodes.Ldstr, "halted"),
            new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(VariableDeclarations), name: "Item").GetGetMethod()),                         //["halted"]
            new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Boolean)),*/
            new CodeInstruction(OpCodes.Brfalse_S, label),
          //    activity = Activity.Halted;
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SquadDoEngageEnemy), nameof(setActivity)))
            /*new CodeInstruction(OpCodes.Ldc_I4_3),
            new CodeInstruction(OpCodes.Stfld, ___activity)*/
        );

        return matcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(Squad), "GetPossibleCommands")]
class SquadGetPossibleCommands
{
    public static bool Prefix(Squad __instance, ref List<Warfare.CommandType> __result, ref Warfare.SquadType ___type)
    {
        BattleController battleController = BattleController.For(__instance.gameObject);
#if RELEASE
        List<Warfare.CommandType> commands = new List<Warfare.CommandType>();
        commands.Add(Warfare.CommandType.Advance);
        commands.Add(Warfare.CommandType.Halt);
        switch (___type)
        {
        case Warfare.SquadType.Assault:
            commands.AddRange(new Warfare.CommandType[1] { Warfare.CommandType.Suppression });
            AddGrenadeCommands();
            break;
        case Warfare.SquadType.MachineGun:
            commands.AddRange(new Warfare.CommandType[1] { Warfare.CommandType.Suppression });
            break;
        case Warfare.SquadType.Mortar:
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, "SmokeMortar"))
            {
                commands.Add(Warfare.CommandType.SmokeMortar);
            }
            break;
        case Warfare.SquadType.Officer:
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, "AirSupport"))
            {
                commands.Add(Warfare.CommandType.AirSupport);
            }
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, "Artillery"))
            {
                commands.Add(Warfare.CommandType.Artillery);
            }
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, "CommandMortars"))
            {
                commands.Add(Warfare.CommandType.HeavyMortar);
            }
            commands.AddRange(new Warfare.CommandType[1] { Warfare.CommandType.SmokeGrenade });
            break;
        case Warfare.SquadType.Riflemen:
            AddGrenadeCommands();
            break;
        case Warfare.SquadType.Sniper:
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, "SmokeGrenades"))
            {
                commands.Add(Warfare.CommandType.SmokeGrenade);
            }
            break;
        }
        List<Warfare.CommandType> possibleFireSupports = battleController.PossibleFireSupports;
        commands = commands.Intersect(possibleFireSupports).ToList();
        __result = commands;
        void AddGrenadeCommands()
        {
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, "Grenades"))
            {
                commands.Add(Warfare.CommandType.Grenade);
            }
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, "SmokeGrenades"))
            {
                commands.Add(Warfare.CommandType.SmokeGrenade);
            }
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, "AntiTankGrenades"))
            {
                commands.Add(Warfare.CommandType.AntiTankGrenade);
            }
        }
#else
        List<Warfare.CommandType> list = new List<Warfare.CommandType>();
        list.Add(Warfare.CommandType.Advance);
        list.Add(Warfare.CommandType.Halt);
        switch (___type)
        {
        case Warfare.SquadType.Assault:
            list.AddRange(new Warfare.CommandType[1] { Warfare.CommandType.Suppression });
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, PlayerAbilities.Ability.Grenades))
            {
                list.Add(Warfare.CommandType.Grenade);
            }
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, PlayerAbilities.Ability.SmokeGrenades))
            {
                list.Add(Warfare.CommandType.SmokeGrenade);
            }
            break;
        case Warfare.SquadType.MachineGun:
            list.AddRange(new Warfare.CommandType[1] { Warfare.CommandType.Suppression });
            break;
        case Warfare.SquadType.Mortar:
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, PlayerAbilities.Ability.SmokeMortar))
            {
                list.Add(Warfare.CommandType.SmokeMortar);
            }
            break;
        case Warfare.SquadType.Officer:
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, PlayerAbilities.Ability.AirSupport))
            {
                list.Add(Warfare.CommandType.AirSupport);
            }
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, PlayerAbilities.Ability.Artillery))
            {
                list.Add(Warfare.CommandType.Artillery);
            }
            list.AddRange(new Warfare.CommandType[1] { Warfare.CommandType.SmokeGrenade });
            break;
        case Warfare.SquadType.Riflemen:
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, PlayerAbilities.Ability.Grenades))
            {
                list.Add(Warfare.CommandType.Grenade);
            }
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, PlayerAbilities.Ability.SmokeGrenades))
            {
                list.Add(Warfare.CommandType.SmokeGrenade);
            }
            break;
        case Warfare.SquadType.Sniper:
            if (battleController.IsSkirmish || battleController.abilities.IsAbilityActive(__instance.army, PlayerAbilities.Ability.SmokeGrenades))
            {
                list.Add(Warfare.CommandType.SmokeGrenade);
            }
            break;
        }
        List<Warfare.CommandType> possibleFireSupports = battleController.PossibleFireSupports;
        __result = list.Intersect(possibleFireSupports).ToList();
#endif
        return false;
    }
}

[HarmonyPatch(typeof(Squad), "SpecialAttackAdvance")]
class SquadSpecialAttackAdvance
{
    public static void Prefix(Squad __instance, object[] __args, ref WFCover ___cover)
    {
        Variables.Object(__instance.gameObject)["halted"] = false;

        //EachSoldier(delegate (Soldier soldier) { soldier.SetPosture(Soldier.Posture.Standing, moving: true); });
        for (int i = 0; i < __instance.transform.childCount; i++)
            __instance.transform.GetChild(i).GetComponent<Soldier>().SetPosture(Soldier.Posture.Standing, moving: true);
    }
}

[HarmonyPatch(typeof(Squad), "MayHalt")]
class SquadMayHalt
{
    public static void Postfix(Squad __instance, ref bool __result, ref Squad.Activity ___activity)
    {
        if (__result)
        {
            if (__instance.isInCover)
                __result = false;
            else
            {
                BattleController battleController = BattleController.For(__instance.gameObject);

                float mindist = 2f;

                bool DistancePredicate(Squad other)
                {
                    return (float)Mathf.Abs(BattleController.SquadDelta(__instance, other)) <= mindist;
                }
                Func<Squad, float> comparator = (Squad other) => BattleController.SquadDelta(__instance, other);
                Squad squadTooNear =
                    (from other in battleController.AllSquads()
                     where other.army == __instance.army && other.lane == __instance.lane && (bool)Variables.Object(other.gameObject)["halted"] && DistancePredicate(other)
                     orderby comparator(other) descending
                     select other).FirstOrDefault();

                if (!(__instance.GetCoverNearPosition(__instance.x, mindist) == null && battleController.GetCoverNearPosition(__instance, __instance.x, __instance.lane, -__instance.direction, mindist) == null && squadTooNear == null))
                    __result = false;
            }
        }
    }
}

[HarmonyPatch(typeof(Squad), "SpecialAttackHalt")]
class SquadSpecialAttackHalt
{
    public static void Prefix(Squad __instance)
    {
        Variables.Object(__instance.gameObject)["halted"] = true;
    }
}

class HaltedChecker
{
    public static bool checkHalted(Soldier instance) { return (bool)Variables.Object(instance.squad.gameObject)["halted"]; }
}

[HarmonyPatch]
class SoldierAdvance
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.EnumeratorMoveNext(typeof(Soldier).GetNestedTypes(AccessTools.all).SelectMany(t => t.GetMethods(AccessTools.all)).FirstOrDefault(x => x.Name.Contains("DoAdvance")));
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        CodeMatcher matcher = new(instructions, generator);
        matcher.MatchStartForward(
            //StopFiring();
            Ldloc_1,
            Callvirt[AccessTools.Method(typeof(Soldier), "Soldier.StopFiring")]
            )
            .ThrowIfNotMatch($"Could not find entry point into IEnumerator DoAdvance inside Soldier::Advance")
            .Advance(12) //skip to after SetPosture(Posture.Standing, moving: true, WeaponPosition.Middle, suppressed: false, null);
            .CreateLabel(out System.Reflection.Emit.Label label)
            .Advance(-12)
            .Insert(    
                //if (!(bool)Variables.Object(squad.gameObject)["halted"])
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HaltedChecker), nameof(HaltedChecker.checkHalted))),
                /*new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Soldier), "Soldier.get_squad")),                                                    //soldier.squad
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(UnityEngine.Component), "UnityEngine.Component.get_gameObject")),               //squad.gameObject
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Variables), nameof(Variables.Object))),                                             //Variables.Object()
                new CodeInstruction(OpCodes.Ldstr, "halted"),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(VariableDeclarations), "Unity.VisualScripting.VariableDeclarations.set_Item")), //["halted"]
                new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Boolean)),*/
                new CodeInstruction(OpCodes.Brtrue_S, label)
            );
        matcher.MatchStartForward(
            //MoveForward();
            Ldloc_0,
            Ldc_I4_0,
            Ldc_I4_0,
            Ldc_R4[1],
            Callvirt[AccessTools.Method(typeof(Soldier), "Soldier.MoveForward")]
            )
            .ThrowIfNotMatch($"Could not find entry point into IEnumerator DoAdvance inside Soldier::Advance")
            .Advance(5) 
            .CreateLabel(out System.Reflection.Emit.Label label2)
            .Advance(-5)
            .Insert(
                //if (!(bool)Variables.Object(squad.gameObject)["halted"])
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HaltedChecker), nameof(HaltedChecker.checkHalted))),
                /*new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Soldier), nameof(Soldier.squad))),                                          //soldier.squad
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnityEngine.Component), nameof(UnityEngine.Component.gameObject))),     //squad.gameObject
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Variables), nameof(Variables.Object))),                                             //Variables.Object()
                new CodeInstruction(OpCodes.Ldstr, "halted"),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(VariableDeclarations), name: "Item").GetGetMethod()),                         //["halted"]
                new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Boolean)),*/
                new CodeInstruction(OpCodes.Brtrue_S, label2)
            );

        return matcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(Soldier), "DoEngagement", MethodType.Enumerator)]
class SoldierDoEngagement
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        CodeMatcher matcher = new(instructions, generator);
        matcher.MatchStartForward(
            //UpdateEngagedMovement();
            Ldloc_1,
            Callvirt[AccessTools.Method(typeof(Soldier), "UpdateEngagedMovement")]
            )
            .ThrowIfNotMatch($"Could not find entry point into Soldier::DoEngagement")
            .Advance(2)
            .CreateLabel(out System.Reflection.Emit.Label label)
            .Advance(-2)
            .Insert(
                //if (!(bool)Variables.Object(squad.gameObject)["halted"])
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HaltedChecker), nameof(HaltedChecker.checkHalted))),
                /*new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Soldier), nameof(Soldier.squad))),                                          //soldier.squad
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnityEngine.Component), nameof(UnityEngine.Component.gameObject))),     //squad.gameObject
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Variables), nameof(Variables.Object))),                                             //Variables.Object()
                new CodeInstruction(OpCodes.Ldstr, "halted"),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(VariableDeclarations), name: "Item").GetGetMethod()),                         //["halted"]
                new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Boolean)),*/
                new CodeInstruction(OpCodes.Brtrue_S, label)
            );

        return matcher.InstructionEnumeration();
    }
}

[HarmonyPatch(typeof(Squad), "MayAdvance")]
class SquadMayAdvance
{
    public static void Postfix(ref bool __result, ref Warfare.SquadType ___type)
    {
        if(___type == Warfare.SquadType.Tank)
            __result = true;
    }
}