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
        PlayerSpell = 6,
        PlayerSpawn = 7,
        PlayerSpawnError = 8,
        PlayerUpdateData = 9,
        SelfSpawn = 10,
        TimeSyncResponse = 11,
        PlayerState = 12,
        UpdateWorld = 13,
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
            { ServerPacket.PlayerSpell, PlayerSpell.Descriptor.Parser },
            { ServerPacket.PlayerSpawn, PlayerSpawn.Descriptor.Parser },
            { ServerPacket.PlayerSpawnError, PlayerSpawnError.Descriptor.Parser },
            { ServerPacket.PlayerUpdateData, PlayerUpdateData.Descriptor.Parser },
            { ServerPacket.SelfSpawn, SelfSpawn.Descriptor.Parser },
            { ServerPacket.TimeSyncResponse, TimeSyncResponse.Descriptor.Parser },
            { ServerPacket.PlayerState, PlayerState.Descriptor.Parser },
            { ServerPacket.UpdateWorld, UpdateWorld.Descriptor.Parser },
        };
    }
}
