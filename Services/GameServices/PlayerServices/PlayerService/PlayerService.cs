using System;
using System.Threading.Tasks;
using VentusServer.Models;
using Game.Models;
using System.Collections.Generic;

namespace VentusServer.Services
{
    public class PlayerService : PlayerModuleService<Player, PlayerEntity>  // Usando PlayerModuleService
    {
        private readonly PlayerRepository _playerRepository;  // Repositorio de jugadores
        private readonly PlayerProgressionRepository _progressionRepository; // Repositorio de progresión
        private readonly PlayerEconomyRepository _economyRepository; // Repositorio de economía
        private readonly PlayerLocationRepository _locationRepository; // Repositorio de ubicación
        private readonly PlayerEquipmentRepository _equipmentRepository; // Repositorio de equipamiento
        private readonly PlayerStatusRepository _statusRepository; // Repositorio de estados
        private readonly AccountRepository _accountRepository; // Repositorio de cuentas

        public PlayerService(
            PlayerRepository playerRepository,
            PlayerProgressionRepository progressionRepository,
            PlayerEconomyRepository economyRepository,
            PlayerLocationRepository locationRepository,
            PlayerEquipmentRepository equipmentRepository,
            PlayerStatusRepository statusRepository,
            AccountRepository accountRepository)
            : base(playerRepository)  // Llamada al constructor base
        {
            _playerRepository = playerRepository;
            _progressionRepository = progressionRepository;
            _economyRepository = economyRepository;
            _locationRepository = locationRepository;
            _equipmentRepository = equipmentRepository;
            _statusRepository = statusRepository;
            _accountRepository = accountRepository;
        }

        // Crear un nuevo jugador y asociarlo con la cuenta
        public async Task<Player> CreatePlayerAsync(Account account, string name, string gender, string race, string classType)
        {
            // Validar que la cuenta no exista ya
            if (await _playerRepository.ExistsAsync(account.Id))
            {
                throw new InvalidOperationException("Este jugador ya existe.");
            }

            // Crear la entidad del jugador
            var playerEntity = new PlayerEntity
            {
                AccountId = account.Id,
                Name = name,
                Gender = gender,
                Race = race,
                Class = classType,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Status = "Offline"
            };

            // Guardar jugador en la base de datos
            var createdPlayerEntity = await _playerRepository.CreateAsync(playerEntity);

            // Mapear la entidad a un modelo de dominio
            var player = PlayerMapper.ToDomainModel(createdPlayerEntity);

            // Inicializar otras propiedades del jugador, como economía, progreso, etc.
            await InitializePlayerProgression(player);
            await InitializePlayerEconomy(player);
            await InitializePlayerLocation(player);
            await InitializePlayerStatus(player);

            return player;
        }

        // Obtener un jugador por su ID
        public async Task<Player> GetPlayerByIdAsync(int playerId)
        {
            var playerEntity = await _playerRepository.GetByIdAsync(playerId);
            if (playerEntity == null) throw new KeyNotFoundException("Jugador no encontrado.");
            return PlayerMapper.ToDomainModel(playerEntity);
        }

        // Obtener un jugador por su AccountId
        public async Task<Player> GetPlayerByAccountIdAsync(int accountId)
        {
            var playerEntity = await _playerRepository.GetByAccountIdAsync(accountId);
            if (playerEntity == null) throw new KeyNotFoundException("Jugador no encontrado.");
            return PlayerMapper.ToDomainModel(playerEntity);
        }

        // Actualizar la información básica del jugador
        public async Task<Player> UpdatePlayerAsync(Player player)
        {
            var playerEntity = PlayerMapper.ToEntity(player);
            var updatedPlayerEntity = await _playerRepository.UpdateAsync(playerEntity);
            return PlayerMapper.ToDomainModel(updatedPlayerEntity);
        }

        // Cambiar el estado del jugador (Ej: "Online", "Offline", "Banned", etc.)
        public async Task<Player> UpdatePlayerStatusAsync(int playerId, string newStatus)
        {
            var player = await GetPlayerByIdAsync(playerId);
            player.UpdateStatus(newStatus);

            // Actualizar estado en la base de datos
            var updatedPlayerEntity = await _playerRepository.UpdateAsync(PlayerMapper.ToEntity(player));
            return PlayerMapper.ToDomainModel(updatedPlayerEntity);
        }

        // Eliminar un jugador (puede incluir lógica para no eliminar permanentemente)
        public async Task DeletePlayerAsync(int playerId)
        {
            var player = await GetPlayerByIdAsync(playerId);
            await _playerRepository.DeleteAsync(playerId);

            // Eliminar o marcar como eliminado las relaciones con otros elementos (progresión, economía, etc.)
            await _progressionRepository.DeleteByPlayerIdAsync(playerId);
            await _economyRepository.DeleteByPlayerIdAsync(playerId);
            await _locationRepository.DeleteByPlayerIdAsync(playerId);
            await _equipmentRepository.DeleteByPlayerIdAsync(playerId);
            await _statusRepository.DeleteByPlayerIdAsync(playerId);
        }

        // Inicializar la progresión del jugador (nivel, XP, puntos de habilidad)
        private async Task InitializePlayerProgression(Player player)
        {
            var playerProgression = new PlayerProgression
            {
                PlayerId = player.Id,
                Level = 1, // Nivel inicial
                Experience = 0,
                SkillPoints = 0
            };
            await _progressionRepository.CreateAsync(playerProgression);
        }

        // Inicializar la economía del jugador (oro, hambre, sed, etc.)
        private async Task InitializePlayerEconomy(Player player)
        {
            var playerEconomy = new PlayerEconomy
            {
                PlayerId = player.Id,
                Gold = 100,  // Oro inicial
                Hunger = 100,
                Thirst = 100
            };
            await _economyRepository.CreateAsync(playerEconomy);
        }

        // Inicializar la ubicación del jugador
        private async Task InitializePlayerLocation(Player player)
        {
            var playerLocation = new PlayerLocation
            {
                PlayerId = player.Id,
                PosMap = "StartMap",  // Mapa inicial
                PosX = 0,
                PosY = 0,
                Direction = "North"
            };
            await _locationRepository.CreateAsync(playerLocation);
        }

        // Inicializar el estado del jugador (veneno, fuego, etc.)
        private async Task InitializePlayerStatus(Player player)
        {
            var playerStatus = new PlayerStatus
            {
                PlayerId = player.Id,
                StatusEffects = new List<string>()  // Lista de efectos de estado inicial
            };
            await _statusRepository.CreateAsync(playerStatus);
        }
    }
}
