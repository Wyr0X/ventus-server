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

        public readonly SessionHandler _sessionHandler;
        public readonly TaskScheduler taskScheduler;
        public readonly GameEventHandler _gameEventHandler;
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
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Inicializando GameServer...");
            this.taskScheduler = taskScheduler;
            this._gameEventHandler = new GameEventHandler(this);
            _sessionHandler = new SessionHandler(this, _gameEventHandler);
            _gameServiceMediator = gameServiceMediator;
            _webSocketServerController = webSocketServerController;
            worldManager = new WorldManager(this);

            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "GameServer inicializado correctamente.");
        }

        /// <summary>
        /// Inicia el bucle de actualización a ~60 FPS.
        /// </summary>
        public async Task Run(CancellationToken cancellationToken)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Iniciando bucle principal...");
            await Loop(cancellationToken);
        }

        private async Task Loop(CancellationToken cancellationToken)
        {
            int tickCount = 0;
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Comenzando loop de actualizaciones...");

            while (!cancellationToken.IsCancellationRequested)
            {
                Update();
                tickCount++;

                if (tickCount % 600 == 0) // Cada 10 segundos aprox
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Tick: {tickCount} - El bucle sigue en ejecución.");
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
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Agendando una nueva acción...");
            _scheduledActions.Enqueue(action);
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Acción programada en la cola.");
        }

        /// <summary>
        /// Cancela el bucle de ejecución.
        /// </summary>
        public void Stop()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Deteniendo servidor manualmente...");
            _loopCancellation?.Cancel();
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Servidor detenido.");
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
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Ejecutando acción agendada...");
                    action();
                    processedActions++;
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Acción ejecutada con éxito.");
                }
                catch (Exception ex)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Error ejecutando acción agendada: {ex.Message}", isError: true);
                }
            }

            if (processedActions > 0)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Ejecutadas {processedActions} acciones agendadas.");
            }


            HandleEvents();
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
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Procesando evento: {gameEvent.Type}.");
                _gameEventHandler.HandlePacket(gameEvent);
            }

            if (handledEvents > 0)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Se procesaron {handledEvents} eventos del buffer.");
            }

        }
    }
}
