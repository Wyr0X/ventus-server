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
        PlayerFinishToLoadGame = 9,
        PlayerSpawnError = 10,
        PlayerUpdateData = 11,
        SelfSpawn = 12,
        UpdateWorld = 13,
        PingPlayerGame = 14,
        PongPlayerGame = 15,
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
            { ServerPacket.PlayerFinishToLoadGame, PlayerFinishToLoadGame.Descriptor.Parser },
            { ServerPacket.PlayerSpawnError, PlayerSpawnError.Descriptor.Parser },
            { ServerPacket.PlayerUpdateData, PlayerUpdateData.Descriptor.Parser },
            { ServerPacket.SelfSpawn, SelfSpawn.Descriptor.Parser },
            { ServerPacket.UpdateWorld, UpdateWorld.Descriptor.Parser },
            { ServerPacket.PingPlayerGame, PingPlayerGame.Descriptor.Parser },
            { ServerPacket.PongPlayerGame, PongPlayerGame.Descriptor.Parser },
        };
    }
}
