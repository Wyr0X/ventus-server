// 🛑 Archivo generado automáticamente. No editar.
using Google.Protobuf;

namespace Ventus.Network.Packets
{
    public enum ClientPacket
    {
        LoginRequest = 1,
        MessagePing = 2,
        ChatSend = 3,
        ChatCommand = 4,
        PlayerInput = 5,
        PlayerJoin = 6,
        PlayerExit = 7,
        TimeSyncRequest = 8,
        PlayerFinishToLoadGame = 9,
    }

    public static class ClientPacketDecoder
    {
        public static readonly Dictionary<ClientPacket, MessageParser> Parsers = new()
        {
            { ClientPacket.LoginRequest, LoginRequest.Descriptor.Parser },
            { ClientPacket.MessagePing, MessagePing.Descriptor.Parser },
            { ClientPacket.ChatSend, ChatSend.Descriptor.Parser },
            { ClientPacket.ChatCommand, ChatCommand.Descriptor.Parser },
            { ClientPacket.PlayerInput, PlayerInput.Descriptor.Parser },
            { ClientPacket.PlayerJoin, PlayerJoin.Descriptor.Parser },
            { ClientPacket.PlayerExit, PlayerExit.Descriptor.Parser },
            { ClientPacket.TimeSyncRequest, TimeSyncRequest.Descriptor.Parser },
            { ClientPacket.PlayerFinishToLoadGame, PlayerFinishToLoadGame.Descriptor.Parser },
        };
    }
}
