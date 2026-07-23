using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SingularityMod.Singularity;

public class SingularityGlobalNPCSpawns : GlobalNPC
{
    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
    {
        if (!SingularityWorld.downedCygnusBoss)
        {
            pool.Remove(NPCID.VoodooDemon); // Important to prevent WOF from spawning before fighting Cygnus
        }
    }
}

public static partial class Extensions
{
    /// <summary>
    /// Includes all modded stats of an NPC
    /// </summary>
    /// <param name="npc"></param>
    /// <returns></returns>
    public static SingularityGlobalNPC Mod(this NPC npc) => npc.GetGlobalNPC<SingularityGlobalNPC>();
}

public class SingularityGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    /// <summary>
    /// How much damage taken gets reduced
    /// </summary>
    public float DamageResist { get; set; } = 0f;

    /// <summary>
    /// Whether Damage Resistance can be reduced
    /// </summary>
    public bool nonReduceableDamageResist = false;

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        ApplyDamageResist(npc, ref modifiers);
    }

    private void ApplyDamageResist(NPC npc, ref NPC.HitModifiers modifiers)
    {
        float DamageResistSim = DamageResist;

        DamageResistSim = float.Clamp(DamageResistSim, 0f, 1f);

        ApplyDamageResistReduction(npc, ref DamageResistSim);

        modifiers.FinalDamage *= 1f - DamageResistSim;
        if (DamageResistSim == 1f)
        {
            modifiers.HideCombatText();
            npc.life += 1;
        }
    }

    private void ApplyDamageResistReduction(NPC npc, ref float DamageResist)
    {
        if (nonReduceableDamageResist)
            return;

        DamageResist = float.Clamp(DamageResist, 0f, 1f);
    }
}
