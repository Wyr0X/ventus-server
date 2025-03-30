using ProtosCommon;
using Webrtc;

public class WebRTCCallSignalingService
{
    private readonly WebSocketServerController _webSocketServerController;

    public WebRTCCallSignalingService(WebSocketServerController webSocketServerController)
    {
        _webSocketServerController = webSocketServerController;
    }

    public void HandleWebRTCSignalingMessage(UserMessagePair messagePair)
    {
        try
        {
            ClientMessage clientMessage = messagePair.ClientMessage;
            ClientMessageWebRTC webrtcMessage = clientMessage.MessageRtc; // Asumiendo que ya tienes un MessageWebRTC

            switch (webrtcMessage.MessageTypeCase)
            {
                case ClientMessageWebRTC.MessageTypeOneofCase.Offer:
                    HandleOffer(messagePair, webrtcMessage.Offer);
                    break;
                case ClientMessageWebRTC.MessageTypeOneofCase.IceCandidate:
                    HandleIceCandidate(messagePair, webrtcMessage.IceCandidate);
                    break;
                default:
                    Console.WriteLine("❌ Mensaje WebRTC no reconocido.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al procesar el mensaje WebRTC: {ex.Message}");
        }
    }

    private void HandleOffer(UserMessagePair messagePair, WebRTCOffer offer)
    {
        // Aquí procesas la oferta de conexión WebRTC
        Console.WriteLine($"📞 Oferta recibida de {messagePair.ConnectionId}");
        // Obtener userId
        // Enviar la respuesta de la oferta (se puede enviar a un cliente específico)
        WebRTCAnswer webRTCAnswer = new WebRTCAnswer{
            UserId = offer.UserId,
            Sdp = offer.Sdp,
            Candidate = offer.Candidate
        };
        ServerMessageWebRTC serverMessageWebRTC = new ServerMessageWebRTC{
            Answer = webRTCAnswer
        };
        //Buscar todos los jugadores del mundo y enviarle
        _webSocketServerController.SendServerPacketBySocket(messagePair.ConnectionId, serverMessageWebRTC);
    }


    private void HandleIceCandidate(UserMessagePair messagePair, WebRTCICECandidate iceCandidate)
    {
        // Procesar los ICE candidates
        Console.WriteLine($"❄️ ICE Candidate recibido de {messagePair.ConnectionId}");

        // Enviar el ICE candidate a la otra parte si es necesario

        ServerMessageWebRTC serverMessageWebRTC = new ServerMessageWebRTC{
            IceCandidate = iceCandidate
        };
        //Buscar todos los jugadores del mundo y enviarle
        _webSocketServerController.SendServerPacketBySocket(messagePair.ConnectionId, serverMessageWebRTC);
    }

}
