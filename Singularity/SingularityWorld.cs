using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SingularityMod.Singularity;

public class SingularityWorld : ModSystem
{
    public static bool downedCygnusBoss; // Cygnus boss (Pre-Hardmode)

    public override void OnWorldLoad()
    {
        downedCygnusBoss = false;
    }

    public override void OnWorldUnload()
    {
        downedCygnusBoss = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["downedCygnusBoss"] = downedCygnusBoss;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        downedCygnusBoss = tag.GetBool("downedCygnusBoss");
    }
}