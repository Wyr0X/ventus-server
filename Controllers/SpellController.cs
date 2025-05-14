using Microsoft.AspNetCore.Mvc;
using VentusServer.Auth;
using VentusServer.DTOs;
using VentusServer.Models;
using VentusServer.Services;
using static LoggerUtil;

namespace VentusServer.Controllers.Admin
{
    [ApiController]
    [Route("admin/spells")]
    [JwtAuthRequired]
    [RequirePermission]
    public class SpellController : ControllerBase
    {
        private readonly SpellService _spellService;

        public SpellController(SpellService spellService)
        {
            _spellService = spellService;
        }

        // Endpoint público para obtener todos los hechizos
        [HttpGet]
        public async Task<IActionResult> GetAllSpells()
        {
            Log(LogTag.SpellController, "Solicitando lista de todos los hechizos...");

            var spells = await _spellService.GetAllSpellsAsync();

            if (spells == null || spells.Count() == 0)
            {
                Log(LogTag.SpellController, "No se encontraron hechizos.");
                return NotFound("No se encontraron hechizos.");
            }

            Log(LogTag.SpellController, $"Hechizos obtenidos: {spells.Count()}");
            return Ok(spells);
        }

        // Endpoint público para obtener un hechizo por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpellById(string id)
        {
            Log(LogTag.SpellController, $"Solicitando hechizo con ID: {id}");

            var spell = await _spellService.GetSpellByIdAsync(id);

            if (spell == null)
            {
                Log(LogTag.SpellController, $"Hechizo no encontrado: {id}", isError: true);
                return NotFound("Hechizo no encontrado.");
            }

            Log(LogTag.SpellController, $"Hechizo encontrado: {id}");
            return Ok(spell);
        }

        // Endpoint protegido para crear un nuevo hechizo
        [HttpPost]
        public async Task<IActionResult> CreateSpell([FromBody] SpellDTO request)
        {
            Log(LogTag.SpellController, $"Intentando crear nuevo hechizo: {request.Name}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value?.Errors?.Select(err => err.ErrorMessage).ToList() ?? new List<string>()
                    });

                Log(LogTag.SpellController, $"ModelState inválido: {System.Text.Json.JsonSerializer.Serialize(errors)}", isError: true);
                return BadRequest(ModelState);
            }

            // Validación extra: asegúrate de que los efectos no sean nulos y tengan sentido
            if (request.Effects == null || !request.Effects.Any())
                return BadRequest("El hechizo debe tener al menos un efecto.");

            var spell = new SpellModel(
                id: Guid.NewGuid().ToString(),
                name: request.Name,
                manaCost: request.ManaCost,
                castTime: request.CastTime,
                cooldown: request.Cooldown,
                range: request.Range,
                isChanneled: request.IsChanneled,
                duration: request.Duration,
                castType: request.CastType, // <-- Add this line, ensure SpellDTO has CastType property of type SpellCastType
                targeting: request.Targeting,
                unitEffects: request.Effects,
                terrainEffects: request.TerrainEffects ?? new List<ITerrainEffect>(),
                summonEffects: request.SummonEffects ?? new List<ISummonEffect>(),
                requiresLineOfSight: request.RequiresLineOfSight,
                requiredLevel: request.RequiredLevel,
                targetType: request.TargetType,
                description: request.Description,
                castSound: request.CastSound,
                impactSound: request.ImpactSound,
                vfxCast: request.VfxCast,
                vfxImpact: request.VfxImpact
            );

            await _spellService.CreateSpellAsync(spell);

            Log(LogTag.SpellController, $"Hechizo creado: {request.Name}");
            return Ok("Hechizo creado correctamente.");
        }

        // Endpoint protegido para actualizar un hechizo
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpell(string id, [FromBody] SpellModel request)
        {
            Log(LogTag.SpellController, $"Intentando actualizar hechizo {id}...");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value != null ? e.Value.Errors?.Select(err => err.ErrorMessage).ToList() ?? new List<string>() : new List<string>()
                    });

                Log(LogTag.SpellController, $"ModelState inválido: {System.Text.Json.JsonSerializer.Serialize(errors)}", isError: true);

                return BadRequest(ModelState);
            }

            var spell = await _spellService.GetSpellByIdAsync(id);
            if (spell == null)
            {
                Log(LogTag.SpellController, $"Hechizo no encontrado: {id}", isError: true);
                return NotFound("Hechizo no encontrado.");
            }


            await _spellService.UpdateSpellAsync(request);

            Log(LogTag.SpellController, $"Hechizo actualizado: {id}");
            return Ok("Hechizo actualizado correctamente.");
        }

        // Endpoint protegido para eliminar un hechizo
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpell(string id)
        {
            Log(LogTag.SpellController, $"Intentando eliminar hechizo {id}...");

            var spell = await _spellService.GetSpellByIdAsync(id);
            if (spell == null)
            {
                Log(LogTag.SpellController, $"Hechizo no encontrado: {id}", isError: true);
                return NotFound("Hechizo no encontrado.");
            }

            await _spellService.DeleteSpellAsync(id);

            Log(LogTag.SpellController, $"Hechizo eliminado: {id}");
            return Ok("Hechizo eliminado correctamente.");
        }
    }
}
