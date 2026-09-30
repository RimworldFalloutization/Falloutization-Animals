using FCP.Core;
using HarmonyLib;
using Verse.AI;

namespace Falloutization.Animals.Patches;

[HarmonyPatch(typeof(MentalState), nameof(MentalState.ForceHostileTo), typeof(Faction))]
public static class MentalState_HostileAnimal_ForceHostileToFaction
{
    public static bool Prefix(MentalState __instance, Faction f, ref bool __result)
    {
        if (__instance is not MentalState_HostileAnimal) return true;
        if (f == null)
        {
            __result = false;
            return false;
        }

        if (ModsConfig.AnomalyActive && f == Faction.OfEntities)
        {
            __result = true;
            return false;
        }

        __result = f.def.humanlikeFaction || f == Faction.OfMechanoids;
        return false;
    }
}

[HarmonyPatch(typeof(JobGiver_HostileAnimal), "FindTarget")]
public static class JobGiver_HostileAnimal_FindTarget
{
    public static bool Prefix(Pawn pawn, JobGiver_HostileAnimal __instance, ref Pawn __result)
    {
        Pawn attacker = RetaliationTarget(pawn);
        if (attacker == null) return true;
        if (!attacker.Spawned || attacker.Map != pawn.Map || attacker.Downed) return true;
        if (!pawn.HostileTo(attacker)) return true;
        if (pawn.Position.DistanceTo(attacker.Position) > __instance.maxRetaliationDistance) return true;

        __result = attacker;
        return false;
    }

    private static Pawn RetaliationTarget(Pawn pawn)
    {
        HediffSet hediffs = pawn.health?.hediffSet;
        if (hediffs == null) return null;

        foreach (Hediff hediff in hediffs.hediffs)
        {
            HediffComp_HostileAnimalRetaliation comp = hediff.TryGetComp<HediffComp_HostileAnimalRetaliation>();
            if (comp?.LastAttacker != null)
                return comp.LastAttacker;
        }

        return null;
    }
}
