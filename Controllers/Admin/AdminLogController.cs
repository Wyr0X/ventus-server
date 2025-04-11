using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[ApiController]
[Route("api/logs")]
public class AdminLogController : ControllerBase
{
    [HttpGet("{tag}")]
    public IActionResult GetLogsByTag(
    string tag,
    [FromQuery] string? date = null,
    [FromQuery] string? contains = null)
    {
        if (!Enum.TryParse(typeof(LoggerUtil.LogTag), tag, out var parsedTag))
            return BadRequest("Invalid tag.");

        var tagFolder = Path.Combine("Logs", tag);
        if (!Directory.Exists(tagFolder))
            return NotFound("No logs for this tag.");

        List<string> allLines = new();

        if (string.IsNullOrEmpty(date))
        {
            var files = Directory.GetFiles(tagFolder, "*.log");
            foreach (var file in files)
            {
                var lines = System.IO.File.ReadAllLines(file);
                allLines.AddRange(lines);
            }
        }
        else
        {
            var logFile = Path.Combine(tagFolder, $"{date}.log");
            if (!System.IO.File.Exists(logFile))
                return NotFound("No log found for this date.");

            var lines = System.IO.File.ReadAllLines(logFile);
            allLines.AddRange(lines);
        }

        if (!string.IsNullOrEmpty(contains))
            allLines = allLines
                .Where(l => l.Contains(contains, StringComparison.OrdinalIgnoreCase))
                .ToList();

        return Ok(new
        {
            Tag = tag,
            Date = date ?? "All",
            Entries = allLines
        });
    }

    [HttpGet("available-tags")]
    public IActionResult GetAvailableTags()
    {
        var folders = Directory.GetDirectories("Logs");
        var tags = folders.Select(Path.GetFileName).ToList();
        return Ok(tags);
    }

    [HttpGet("{tag}/dates")]
    public IActionResult GetAvailableDates(string tag)
    {
        var tagFolder = Path.Combine("Logs", tag);
        if (!Directory.Exists(tagFolder))
            return NotFound("No logs for this tag.");

        var files = Directory.GetFiles(tagFolder, "*.log");
        var dates = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToList();
        return Ok(dates);
    }
    [HttpGet("all-tags")]
    public IActionResult GetAllDefinedTags()
    {
        var allTags = Enum.GetNames(typeof(LoggerUtil.LogTag));
        return Ok(allTags);
    }
}
