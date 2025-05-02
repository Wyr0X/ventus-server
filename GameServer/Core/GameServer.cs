using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Game.Models;

namespace Game.Server
{
    /// <summary>
    /// Servidor principal: procesa eventos, actualiza mundos y gestiona la sesión.
    /// </summary>
    public class GameServer
    {
        private CancellationTokenSource? _loopCancellation;

        private readonly SessionSystem _sessionSystem;
        public readonly SystemHandler systemHandler;
        public readonly TaskScheduler taskScheduler;
        public readonly PacketHandler packetHandler;
        public Dictionary<int, PlayerModel> PlayerModels { get; set; } = new Dictionary<int, PlayerModel>();
        public readonly GameServiceMediator _gameServiceMediator;
        public readonly WorldManager worldManager;
        private readonly ConcurrentQueue<Action> _scheduledActions = new();
        public readonly WebSocketServerController _webSocketServerController;

        public GameServer(
            WebSocketServerController webSocketServerController,
            TaskScheduler taskScheduler,
            GameServiceMediator gameServiceMediator)
        {
            this.taskScheduler = taskScheduler;
            this.systemHandler = new SystemHandler();
            _sessionSystem = new SessionSystem(this, systemHandler);
            this.packetHandler = new PacketHandler(this);
            _gameServiceMediator = gameServiceMediator;
            _webSocketServerController = webSocketServerController;
            worldManager = new WorldManager(this);

            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "GameServer inicializado.");
        }

        /// <summary>
        /// Inicia el bucle de actualización a ~60 FPS.
        /// </summary>
        public async Task Run(CancellationToken cancellationToken)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Iniciando bucle principal.");
            await Loop(cancellationToken);
        }

        private async Task Loop(CancellationToken cancellationToken)
        {
            int tickCount = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                Update();
                tickCount++;
                if (tickCount % 600 == 0) // cada 10 segundos aprox
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Tick: {tickCount} - Loop en ejecución.");
                }

                await Task.Delay(16, cancellationToken); // Aproximadamente 60fps
            }

            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Loop cancelado.");
        }

        /// <summary>
        /// Permite que otros threads agenden acciones para el hilo principal.
        /// </summary>
        public void Schedule(Action action)
        {
            _scheduledActions.Enqueue(action);
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Acción programada.");
        }

        /// <summary>
        /// Cancela el bucle de ejecución.
        /// </summary>
        public void Stop()
        {
            _loopCancellation?.Cancel();
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Servidor detenido manualmente.");
        }

        /// <summary>
        /// Actualiza la lógica del servidor en cada tick.
        /// </summary>
        private void Update()
        {
            int processedActions = 0;

            while (_scheduledActions.TryDequeue(out var action))
            {
                try
                {
                    action();
                    processedActions++;
                }
                catch (Exception ex)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Error ejecutando acción agendada: {ex.Message}", isError: true);
                }
            }

            if (processedActions > 0)
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Ejecutadas {processedActions} acciones agendadas.");

            HandleEvents();

            // worldManager.Update(); // si se usa
            // _syncSystem.Flush(); // si se usa
        }

        /// <summary>
        /// Despacha todos los eventos encolados.
        /// </summary>
        private void HandleEvents()
        {
            int handledEvents = 0;
            GameEvent? gameEvent;

            while ((gameEvent = taskScheduler.eventBuffer.DequeueEvent()) != null)
            {
                handledEvents++;
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Handle event: {gameEvent.Type}.");

                switch (gameEvent.Type)
                {
                    case GameEventType.CustomGameEvent:
                        LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Handle event: {gameEvent.Type}.");

                        if (gameEvent.Data != null)
                        {
                            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Handle event: {gameEvent.Type}.");

                            systemHandler.HandlePacket(gameEvent.Data);
                        }
                        break;

                    case GameEventType.ClientPacket:
                        if (gameEvent.Data is UserMessagePair userMessagePair)
                            packetHandler.HandlePacket(userMessagePair);
                        break;
                }
            }

            if (handledEvents > 0)
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Procesados {handledEvents} eventos del buffer.");
        }
    }
}
