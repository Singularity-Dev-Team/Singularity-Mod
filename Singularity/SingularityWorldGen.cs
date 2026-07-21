using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.IO;

namespace SingularityMod.Singularity
{
    public class SingularityWorldGen : ModSystem
    {
        private int ChangesiteOreAttempts = 2000;

        public override void ModifyWorldGenTasks(
            List<GenPass> tasks,
            ref double totalWeight)
        {
            int index = tasks.FindIndex(
                genpass => genpass.Name.Equals("Shinies") // Shinies = Ores
            );

            if(index != -1)
            {
                tasks.Insert(
                    index + 1,
                    new PassLegacy(
                        "Generating Changesite", // Make sure to notify
                        GenerateOre // Type
                    )
                );
            }
        }


        private void GenerateOre(GenerationProgress progress, GameConfiguration config)
        {
            for(int i = 0; i < ChangesiteOreAttempts; i++)
            {

                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next(
                    (int)Main.worldSurface,
                    Main.maxTilesY
                );

                WorldGen.TileRunner(
                    x,
                    y,
                    WorldGen.genRand.Next(3, 6), // Thickness
                    WorldGen.genRand.Next(3, 6), // Length
                    ModContent.TileType<Tiles.ChangesiteTile>()
                );
            }
        }
    }
}