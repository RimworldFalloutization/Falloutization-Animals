using Verse;
using Verse.AI;

namespace Falloutization.Animals;

public class HediffCompProperties_HostileAnimalRetaliation : HediffCompProperties
{
    public HediffCompProperties_HostileAnimalRetaliation()
    {
        compClass = typeof(HediffComp_HostileAnimalRetaliation);
    }
}

public class HediffComp_HostileAnimalRetaliation : HediffComp
{
    private Pawn lastAttacker;

    public Pawn LastAttacker => lastAttacker;

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_References.Look(ref lastAttacker, "lastAttacker");
    }

    public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        if (!dinfo.Def.ExternalViolenceFor(Pawn)) return;
        if (dinfo.Instigator is not Pawn attacker || attacker == Pawn || attacker.Dead) return;

        lastAttacker = attacker;
        if (Pawn.Spawned && !Pawn.Downed && Pawn.CurJob?.GetTarget(TargetIndex.A).Thing != attacker)
            Pawn.jobs?.CheckForJobOverride();
    }
}
