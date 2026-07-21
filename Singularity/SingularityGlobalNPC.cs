using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SingularityMod.Singularity;

public class SingularityGlobalNPC : GlobalNPC
{
    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
    {
        if (!SingularityWorld.downedCygnusBoss)
        {
            pool.Remove(NPCID.VoodooDemon); // Important to prevent WOF from spawning before fighting Cygnus
        }
    }
}