// 🛑 Archivo generado automáticamente. No editar.
using Google.Protobuf;

namespace Ventus.Network.Packets
{
    public enum ServerPacket
    {
        LoginResponse = 1,
        MessagePong = 2,
        StatusMessage = 3,
        OutgoingChatMessage = 4,
        SystemMessage = 5,
        PlayerPosition = 6,
        PlayerSpell = 7,
        PlayerSpawn = 8,
        PlayerSpawnError = 9,
    }

    public static class ServerPacketDecoder
    {
        public static readonly Dictionary<ServerPacket, MessageParser> Parsers = new()
        {
            { ServerPacket.LoginResponse, LoginResponse.Descriptor.Parser },
            { ServerPacket.MessagePong, MessagePong.Descriptor.Parser },
            { ServerPacket.StatusMessage, StatusMessage.Descriptor.Parser },
            { ServerPacket.OutgoingChatMessage, OutgoingChatMessage.Descriptor.Parser },
            { ServerPacket.SystemMessage, SystemMessage.Descriptor.Parser },
            { ServerPacket.PlayerPosition, PlayerPosition.Descriptor.Parser },
            { ServerPacket.PlayerSpell, PlayerSpell.Descriptor.Parser },
            { ServerPacket.PlayerSpawn, PlayerSpawn.Descriptor.Parser },
            { ServerPacket.PlayerSpawnError, PlayerSpawnError.Descriptor.Parser },
        };
    }
}
