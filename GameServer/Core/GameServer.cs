using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Game.Models;

namespace Game.Server
{
    /// <summary>
    /// Servidor principal: procesa eventos, actualiza mundos y gestiona la sesión.
    /// </summary>
    public class GameServer
    {
        private readonly CancellationTokenSource _cts = new(); private readonly Channel<Action> _actionChannel = Channel.CreateUnbounded<Action>();

        public readonly SessionHandler _sessionHandler;
        public readonly TaskScheduler taskScheduler;
        public readonly GameEventHandler _gameEventHandler;
        public ConcurrentDictionary<int, PlayerModel> PlayerModels { get; } = new();
        public readonly GameServiceMediator _gameServiceMediator;
        public readonly WorldManager worldManager;
        public readonly WebSocketServerController _webSocketServerController;
        public ConcurrentDictionary<int, PlayerObject> playersInTheGame { get; } = new();
        public ConcurrentDictionary<Guid, PlayerObject> playersByAccountId { get; } = new();

        public GameServer(
            WebSocketServerController webSocketServerController,
            TaskScheduler taskScheduler,
            GameServiceMediator gameServiceMediator)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Inicializando GameServer...");
            this.taskScheduler = taskScheduler;
            _gameEventHandler = new GameEventHandler(this);
            _sessionHandler = new SessionHandler(this, _gameEventHandler);
            _gameServiceMediator = gameServiceMediator;
            _webSocketServerController = webSocketServerController;
            worldManager = new WorldManager(this);
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "GameServer inicializado correctamente.");
        }

        /// <summary>
        /// Inicia el bucle de actualización a ~60 FPS.
        /// </summary>
        public async Task RunAsync(CancellationToken externalToken)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Iniciando bucle principal...");
            var sw = Stopwatch.StartNew();

            try
            {
                while (!externalToken.IsCancellationRequested)
                {
                    var tickStart = sw.ElapsedMilliseconds;

                    await ProcessScheduledActionsAsync();
                    ProcessEvents();
                    _ = ProcessPlayerInputs();
                    _ = worldManager.UpdateWorlds();

                    // El tiempo 0 , 60 , 120, 

                    var elapsed = sw.ElapsedMilliseconds - tickStart;
                    var delay = Math.Max(0, 16 - (int)elapsed);
                    await Task.Delay(delay, externalToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Shutdown solicitado
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer,
                    $"Error inesperado en el bucle principal: {ex.Message}", isError: true);
            }
            finally
            {
                sw.Stop();
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Bucle principal terminado.");
            }
        }
        /// <summary>
        /// Agenda una acción para el hilo principal.
        /// </summary>
        public void Schedule(Action action)
        {
            if (!_actionChannel.Writer.TryWrite(action))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer,
                    "No se pudo agendar la acción.", isError: true);
            }
        }

        private Task ProcessScheduledActionsAsync()
        {
            int count = 0;
            while (_actionChannel.Reader.TryRead(out var action))
            {
                try
                {
                    action();
                    count++;
                }
                catch (Exception ex)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer,
                        $"Error ejecutando acción agendada: {ex.Message}", isError: true);
                }
            }
            if (count > 0)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer,
                    $"Ejecutadas {count} acciones agendadas.");
            }

            return Task.CompletedTask;
        }

        private void ProcessEvents()
        {
            int handled = 0;
            while (taskScheduler.eventBuffer.DequeueEvent() is GameEvent gameEvent)
            {
                try
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer,
                        $"Procesando evento: {gameEvent.Type}.");
                    _gameEventHandler.HandlePacket(gameEvent);
                    handled++;
                }
                catch (Exception ex)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer,
                        $"Error procesando evento {gameEvent.Type}: {ex.Message}", isError: true);
                }
            }
            if (handled > 0)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer,
                    $"Se procesaron {handled} eventos del buffer.");
            }
        }
        private Task ProcessPlayerInputs()
        {
            foreach (var player in playersInTheGame.Values)
            {
                player.ProcessInputs();
            }

            return Task.CompletedTask;
        }

        public void RemovePlayerFromGame(int playerId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] Intentando eliminar jugador {playerId}...");
            worldManager.RemovePlayerFromWorld(playerId);

            if (playersInTheGame.TryRemove(playerId, out var player))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] Jugador {playerId} eliminado de playersInTheGame.");

                // Verifica el contenido de player por si viene null por error
                if (player == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] Advertencia: PlayerObject de {playerId} es null tras TryRemove.", isError: true);
                }
                else
                {
                    var accountId = player.PlayerModel.AccountId;
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] AccountId asociado: {accountId}");

                    if (accountId != Guid.Empty)
                    {
                        if (playersByAccountId.TryRemove(accountId, out _))
                        {
                            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] Jugador con accountId {accountId} eliminado de playersByAccountId.");
                        }
                        else
                        {
                            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] No se encontró el accountId {accountId} en playersByAccountId.", isError: true);
                        }
                    }
                    else
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] AccountId está vacío para el jugador {playerId}.", isError: true);
                    }

                    if (PlayerModels.TryRemove(playerId, out _))
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] Modelo del jugador {playerId} eliminado de PlayerModels.");
                    }
                    else
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] No se encontró modelo del jugador {playerId} en PlayerModels.", isError: true);
                    }

                    // Limpieza del mundo
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] Intentando eliminar al jugador {playerId} del mundo...");
                }

                player = null;
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"[RemovePlayerFromGame] No se encontró al jugador {playerId} en playersInTheGame.", isError: true);
            }
        }


    }
}
