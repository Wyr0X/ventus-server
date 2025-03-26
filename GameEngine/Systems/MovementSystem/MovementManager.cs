using System;
using Protos.Game.Movement;
using Protos.Game.Common;
using System.Collections.Generic;

public class MovementManager
{
    // Este es el constructor donde se inyectan otros servicios necesarios.
    // Ejemplo: Servicio de colisiones, servicio de actualización de estado del jugador, etc.
    public MovementManager(/* INyectar dependencias necesarias, ejemplo: ICollisionService, IPlayerService */)
    {
        // Inicializar servicios
    }

    // Función para manejar el mensaje de solicitud de movimiento
    public void HandleMoveRequest(UserMessagePair messagePair)
    {
        // Extracción de la solicitud de movimiento del mensaje recibido
        ClientMessageMovement movementMessage = messagePair.ClientMessage.MessageOauth.ClientMessageGame.MessageMovement;
        
        if (movementMessage.MessageTypeCase == ClientMessageMovement.MessageTypeOneofCase.MoveRequest)
        {
            MoveRequest moveRequest = movementMessage.MoveRequest;
            
            // Validar si el movimiento es posible (ejemplo: chequeo de colisiones, límites del mapa)
            if (ValidateMovement(moveRequest.x, moveRequest.y))
            {
                // Si la validación es correcta, se actualiza la posición del jugador
                UpdatePlayerPosition(messagePair.PlayerId, moveRequest.x, moveRequest.y);

                // Notificar a otros jugadores de que este jugador se ha movido
                BroadcastPlayerMoved(messagePair.PlayerId, moveRequest.x, moveRequest.y);

                // Enviar respuesta positiva al cliente
                SendMoveResponse(moveRequest.requestId, moveRequest.x, moveRequest.y, true, "Movimiento exitoso");
            }
            else
            {
                // Si la validación falla, enviar respuesta de error al cliente
                SendMoveResponse(moveRequest.requestId, moveRequest.x, moveRequest.y, false, "Colisión detectada o fuera de límites");
            }
        }
        else
        {
            // Si el mensaje recibido no es un MoveRequest, logueamos un error
            Console.WriteLine("❌ Tipo de mensaje de movimiento no reconocido.");
        }
    }

    // Función que valida si el movimiento es posible
    private bool ValidateMovement(int x, int y)
    {
        // Lógica de validación: aquí debemos verificar si la posición x, y es válida
        // Este es un lugar donde podrías delegar la validación de colisiones, límites del mapa, etc.
        
        // Ejemplo hardcodeado: se acepta todo movimiento dentro del rango [-100, 100] en X y Y
        if (x >= -100 && x <= 100 && y >= -100 && y <= 100)
        {
            // Llamar al servicio de colisiones, por ejemplo, verificar si hay algún obstáculo
            // return _collisionService.IsCollision(x, y);
            
            return true; // Movimiento válido
        }

        return false; // Movimiento inválido
    }

    // Función que actualiza la posición del jugador
    private void UpdatePlayerPosition(int playerId, int x, int y)
    {
        // Lógica de actualización de la posición del jugador
        // En un sistema real, se actualizaría la base de datos o el estado en memoria de los jugadores
        Console.WriteLine($"Jugador {playerId} movido a posición ({x}, {y})");

        // Este es el lugar donde interactuarías con un servicio de actualización de estado del jugador
        // Ejemplo: _playerService.UpdatePlayerPosition(playerId, x, y);
    }

    // Función que envía la respuesta al cliente indicando si el movimiento fue exitoso
    private void SendMoveResponse(string requestId, int x, int y, bool success, string reason)
    {
        // Crear el mensaje de respuesta con el estado del movimiento
        MoveResponse response = new MoveResponse
        {
            RequestId = requestId,
            X = x,
            Y = y,
            Success = success,
            Reason = reason
        };

        // Aquí enviamos la respuesta de vuelta al cliente. Este es un ejemplo hardcodeado:
        Console.WriteLine($"Enviando respuesta de movimiento al cliente. Success: {success}, Reason: {reason}");

        // En un caso real, se enviaría a través de WebSocket al cliente correspondiente.
        // Ejemplo: _webSocketService.SendMessageToClient(playerId, response);
    }

    // Función que notifica a todos los jugadores que un jugador se ha movido
    private void BroadcastPlayerMoved(int playerId, int x, int y)
    {
        // Crear el mensaje para notificar a los demás jugadores
        PlayerMoved playerMoved = new PlayerMoved
        {
            PlayerId = playerId,
            X = x,
            Y = y,
            Timestamp = DateTime.UtcNow.Ticks // Establecer un timestamp en el que ocurrió el movimiento
        };

        // Notificar a todos los jugadores en el área relevante (este es un ejemplo hardcodeado)
        Console.WriteLine($"Notificando a otros jugadores que el jugador {playerId} se movió a ({x}, {y})");

        // Este es el lugar donde interactuarías con un servicio que maneja la difusión a otros jugadores
        // Ejemplo: _broadcastService.SendToAllPlayers(playerMoved);
    }
}

