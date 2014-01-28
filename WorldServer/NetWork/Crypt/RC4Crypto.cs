
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace WorldServer
{
    [Crypt("RC4Crypto")]
    public class RC4Crypto : ICryptHandler
    {
        public byte[] Crypt(CryptKey Key, byte[] packet)
        {
            packet = PacketOut.EncryptMythicRC4(Key.GetbKey(), packet);

            return packet;
        }

        public PacketIn Decrypt(CryptKey Key, byte[] packet)
        {
            PacketIn Packet = new PacketIn(packet, 0, packet.Length);
            Packet = Packet.DecryptMythicRC4(Key.GetbKey());
            return Packet;
        }

        public CryptKey GenerateKey(BaseClient client)
        {
            return new CryptKey(new byte[0]);
        }
    }
}
