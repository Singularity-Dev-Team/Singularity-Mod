using System.IO;
using Terraria.ModLoader;


namespace SingularityMod.Content.NPCs.Bosses.Cygnus
{
    public abstract class CygnusSegment : ModNPC
    {
        public int CygnusID;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(CygnusID);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CygnusID = reader.ReadInt32();
        }
    }
}