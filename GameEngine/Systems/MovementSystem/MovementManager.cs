using System;
using Protos.Game.Movement;
using Protos.Game.Common;
using System.Collections.Generic;

public class MovementManager
{
    // Inyectar dependencias necesarias, como servicios de validación, actualización de jugadores, etc.
    private readonly IPlayerService _playerService;
    private readonly IMapService _mapService;
    private readonly IPlayerWorldRelationService _playerWorldRelationService;
    private readonly ICollisionService _collisionService;
    private readonly IWebSocketService _webSocketService;

    public MovementManager(
        IPlayerService playerService,
        IMapService mapService,
        IPlayerWorldRelationService playerWorldRelationService,
        ICollisionService collisionService,
        IWebSocketService webSocketService)
    {
        _playerService = playerService;
        _mapService = mapService;
        _playerWorldRelationService = playerWorldRelationService;
        _collisionService = collisionService;
        _webSocketService = webSocketService;
    }

    // Función para manejar el mensaje de solicitud de movimiento
    public void HandleMoveRequest(UserMessagePair messagePair)
    {
        // Extracción de la solicitud de movimiento del mensaje recibido
        ClientMessageMovement movementMessage = messagePair.ClientMessage.MessageOauth.ClientMessageGame.MessageMovement;

        if (movementMessage.MessageTypeCase == ClientMessageMovement.MessageTypeOneofCase.MoveRequest)
        {
            MoveRequest moveRequest = movementMessage.MoveRequest;

            // Obtener jugador actual ¿Que identificador usaremos? creoq ue el userId
            var player = _playerService.GetCurrentPlayer(messagePair.PlayerId);
            var playerWorldRelation = _playerWorldRelationService.GetPlayerWorldRelation(player.Id);

            // Guardamos la posición anterior del jugador para revertir en caso de movimiento inválido
            var previousPosition = playerWorldRelation.Position;

            // Validar si el movimiento es posible
            if (ValidateMovement(playerWorldRelation, moveRequest.x, moveRequest.y))
            {
                // Si la validación es correcta, se actualiza la posición del jugador
                UpdatePlayerPosition(playerWorldRelation, moveRequest.x, moveRequest.y);

                // Notificar a otros jugadores de que este jugador se ha movido
                BroadcastPlayerMoved(player.Id, moveRequest.x, moveRequest.y);

                // Enviar respuesta positiva al cliente
                SendMoveResponse(moveRequest.requestId, moveRequest.x, moveRequest.y, true, "Movimiento exitoso");
            }
            else
            {
                // Si la validación falla, revertimos al jugador a su posición anterior
                RevertPlayerPosition(playerWorldRelation, previousPosition.x, previousPosition.y);

                // Enviar respuesta de error al cliente
                SendMoveResponse(moveRequest.requestId, previousPosition.x, previousPosition.y, false, "Colisión detectada o fuera de límites");
            }
        }
        else
        {
            // Si el mensaje recibido no es un MoveRequest, logueamos un error
            Console.WriteLine("❌ Tipo de mensaje de movimiento no reconocido.");
        }
    }

    // Función que valida si el movimiento es posible
    private bool ValidateMovement(PlayerWorldRelation playerWorldRelation, int x, int y)
    {
        // Lógica de validación: verificar si la nueva posición es válida dentro de los límites del mapa
        var map = _mapService.GetMap(playerWorldRelation.MapId);
        
        // Llamar al servicio de colisiones para verificar si hay obstáculos
        if (!_collisionService.IsValidMove(map, x, y))
        {
            return false; // Movimiento inválido debido a una colisión
        }

        // Lógica adicional de validación (por ejemplo, límites del mapa)
        if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
        {
            return false; // Fuera de los límites del mapa
        }

        return true; // Movimiento válido
    }

    // Función que actualiza la posición del jugador
    private void UpdatePlayerPosition(PlayerWorldRelation playerWorldRelation, int x, int y)
    {
        // Actualizar la posición del jugador en el mundo
        playerWorldRelation.Position = new Position { x = x, y = y };
        _playerWorldRelationService.UpdatePlayerWorldRelation(playerWorldRelation);

        // Aquí interactuaríamos con un servicio de actualización de estado del jugador
        _playerService.UpdatePlayerPosition(playerWorldRelation.PlayerId, x, y);
        Console.WriteLine($"Jugador {playerWorldRelation.PlayerId} movido a posición ({x}, {y})");
    }

    // Función que revierte la posición del jugador a la anterior
    private void RevertPlayerPosition(PlayerWorldRelation playerWorldRelation, int x, int y)
    {
        // Volver a la posición anterior del jugador
        playerWorldRelation.Position = new Position { x = x, y = y };
        _playerWorldRelationService.UpdatePlayerWorldRelation(playerWorldRelation);

        // Aquí también actualizamos la posición del jugador en el servicio de jugadores
        _playerService.UpdatePlayerPosition(playerWorldRelation.PlayerId, x, y);
        Console.WriteLine($"Posición revertida para el jugador {playerWorldRelation.PlayerId} a ({x}, {y})");
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

        // Enviar la respuesta de vuelta al cliente
        Console.WriteLine($"Enviando respuesta de movimiento al cliente. Success: {success}, Reason: {reason}");

        // Aquí se enviaría a través de WebSocket al cliente correspondiente
        _webSocketService.SendMessageToClient(requestId, response);
    }

    // Función que notifica a otros jugadores que un jugador se ha movido
    private void BroadcastPlayerMoved(int playerId, int x, int y)
    {
        // Crear el mensaje para notificar a los demás jugadores
        PlayerMoved playerMoved = new PlayerMoved
        {
            PlayerId = playerId,
            X = x,
            Y = y,
            Timestamp = DateTime.UtcNow.Ticks
        };

        // Notificar a otros jugadores en el área relevante
        Console.WriteLine($"Notificando a otros jugadores que el jugador {playerId} se movió a ({x}, {y})");

        // Este es el lugar donde interactuarías con un servicio que maneja la difusión a otros jugadores
        _webSocketService.BroadcastToAllPlayers(playerMoved);
    }
}
