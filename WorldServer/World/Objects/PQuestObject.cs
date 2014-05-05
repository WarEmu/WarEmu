
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;
namespace WorldServer
{
    public class PQuestObject : Object
    {
        // Forteresse, Stage 2, BLEU
        /*|00 9E C1 00 00 00 BC 00 02 01 00 00 14 48 6F 6C |.............Hol|
|6D 73 74 65 69 6E 6E 20 52 65 76 69 73 69 74 65 |msteinn Revisite|
|64 02 00 00 04 43 00 01 00 00 05 00 00 00 13 4E |d....C.........N|
|6F 72 64 6C 61 6E 64 20 56 65 72 6B 65 6E 6E 65 |ordland Verkenne|
|72 73 00 00 08 53 74 61 67 65 20 49 49 00 41 44 |rs...Stage II.AD|
|65 66 65 61 74 20 74 68 6F 73 65 20 77 68 6F 20 |efeat those who |
|77 6F 75 6C 64 20 63 68 65 61 74 20 74 68 65 20 |would cheat the |
|52 61 76 65 6E 20 48 6F 73 74 20 6F 66 20 74 68 |Raven Host of th|
|65 20 73 70 6F 69 6C 73 20 6F 66 20 77 61 72 21 |e spoils of war!|
|00 00 02 58 00 00 02 58 00 00 00 00 43 00 00 00 |...X...X....C...|
|00                                              |.               |*/

        /* // Affiche le nom en bleu au milieu de l'écran, réinitialisation dans 12:47 en haut a droite
         * |00 26 C1 00 00 00 BC 01 02 01 00 00 14 48 6F 6C |.&...........Hol|
|6D 73 74 65 69 6E 6E 20 52 65 76 69 73 69 74 65 |msteinn Revisite|
|64 00 00 00 B4 00 00 00 00                      |.........       |
         * 
         * // Stage deux , BLANC
         * |00 9B C1 00 00 00 C0 00 02 01 00 00 0D 53 61 63 |.............Sac|
|72 65 64 20 47 72 6F 75 6E 64 02 00 00 04 4F 00 |red Ground....O.|
|01 00 00 0A 00 00 00 13 48 6F 77 6C 69 6E 67 20 |........Howling |
|57 6F 6C 66 20 43 6C 65 72 67 79 00 00 08 53 74 |Wolf Clergy...St|
|61 67 65 20 49 49 00 38 54 68 65 20 52 61 76 65 |age II.8The Rave|
|6E 20 47 6F 64 20 73 70 65 61 6B 73 21 20 20 54 |n God speaks!  T|
|68 65 20 66 6F 6C 6C 6F 77 65 72 73 20 6F 66 20 |he followers of |
|55 6C 72 69 63 20 6D 75 73 74 20 66 61 6C 6C 21 |Ulric must fall!|
|00 00 02 58 00 00 02 58 00 00 0D 53 61 63 72 65 |...X...X...Sacre|
|64 20 47 72 6F 75 6E 64 00 45 00 00 00 00       |..............  |
         * 
         * // Stage 3 Blanc
         * |00 C4 C1 00 00 00 C0 00 02 01 00 00 0D 53 61 63 |.............Sac|
|72 65 64 20 47 72 6F 75 6E 64 02 00 00 04 50 00 |red Ground....P.|
|02 00 00 01 00 00 00 0B 53 6F 6E 20 52 61 6C 67 |........Son Ralg|
|65 69 72 00 00 01 00 00 00 14 54 68 65 20 47 72 |eir.......The Gr|
|65 61 74 20 57 68 69 74 65 20 57 6F 6C 66 00 00 |eat White Wolf..|
|09 53 74 61 67 65 20 49 49 49 00 4D 53 6C 61 79 |.Stage III.MSlay|
|20 74 68 65 20 43 68 61 6D 70 69 6F 6E 20 6F 66 | the Champion of|
|20 55 6C 72 69 63 20 61 6E 64 20 72 65 76 65 61 | Ulric and revea|
|6C 20 74 68 69 73 20 27 57 6F 6C 66 20 47 6F 64 |l this 'Wolf God|
|27 20 66 6F 72 20 74 68 65 20 77 65 61 6B 6C 69 |' for the weakli|
|6E 67 20 68 65 20 69 73 21 00 00 03 84 00 00 03 |ng he is!.......|
|84 00 00 0D 53 61 63 72 65 64 20 47 72 6F 75 6E |....Sacred Groun|
|64 00 45 00 00 00 00                            |.......         |
         * */
        public PQuest_Info Info;

        public PQuestObject()
            : base()
        {

        }

        public PQuestObject(PQuest_Info Info)
            : this()
        {
            this.Info = Info;
            Name = Info.Name;
        }

        public void SendReinitTime(Player Plr, ushort Time)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_INFO);
            Out.WriteUInt32(Info.Entry);
            Out.WriteByte(1);
            Out.WriteByte(2);
            Out.WriteByte(1);
            Out.WriteUInt16(0);
            Out.WritePascalString(Info.Name);
            Out.WriteUInt16(0);
            Out.WriteUInt16(Time); // Time in seconds
            Out.WriteUInt16(0);
            Out.WriteUInt16(0);
            Plr.SendPacket(Out);
        }

        public void SendStageInfo(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_UPDATE);
            Out.WriteUInt32(Info.Entry);
            Out.WriteByte(0);
            Out.Fill(0, 3);
            Plr.SendPacket(Out);
        }

        public void SendCurrentStage(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_INFO);
            Out.WriteUInt32(Info.Entry);
            Out.WriteByte(0); // Type
            Out.WriteByte(Info.Type);
            Out.WriteByte(Info.Type); // Forteresse, rien du tout ,etc
            Out.WriteUInt16(0);
            Out.WritePascalString(Info.Name);
            Out.WriteByte(1);
            Out.WriteUInt16(0); // Time in second
            Out.WriteUInt32(0x034B0201);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteByte(1);
            Out.WriteUInt16(0);
            Out.WriteByte(0);
            Out.WritePascalString("Un Nom");
            Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteStringToZero("Stage III");
            Out.WritePascalString("Test d'un message quelque soit");
            Out.WriteUInt16(0);
            Out.WriteUInt16(0x012C);
            Out.WriteUInt32(0x9E);
            Out.WriteUInt32(0);
            Out.WriteByte(0x48);
            Out.WriteUInt32(0);
            //Out.WriteHexStringBytes(Str.Replace(" ", string.Empty));
            Plr.SendPacket(Out);
        }

        public override void OnLoad()
        {
            Log.Success("PQ", "Loading :" + Info.Name);

            X = (int)Info.PinX;
            Y = (int)Info.PinY;
            Z = 16384;
            SetOffset(Info.OffX, Info.OffY);

            base.OnLoad();
            IsActive = true;
        }

        public override void SendMeTo(Player Plr)
        {
            SendCurrentStage(Plr);
            SendStageInfo(Plr);
            // TODO
            // Send Quest Info && Current Stage && Current Players
        }
    }
}
