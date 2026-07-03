using Microsoft.AspNetCore.Mvc;
using LiveNewsAggregator.Services;
using LiveNewsAggregator.Models;
using System.Text.Json;

namespace LiveNewsAggregator.Controllers;

public class NewsController : Controller
{
    private readonly NewsGeneratorService _newsService;
    private readonly NewsRankingService _rankingService;

    public NewsController(NewsGeneratorService newsService, NewsRankingService rankingService)
    {
        _newsService = newsService;
        _rankingService = rankingService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/news/stream")]
    public async Task StreamNews([FromQuery] string interests = "سياسة,تكنولوجيا,رياضة")
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var prefs = new UserPreferences
        {
            Interests = interests.Split(',').Select(i => i.Trim()).ToList()
        };

        await using var writer = new StreamWriter(Response.Body);

        while (!HttpContext.RequestAborted.IsCancellationRequested)
        {
            var allNews = _newsService.GetAllNews();
            var ranked = _rankingService.RankNewsForUser(allNews, prefs);

            var json = JsonSerializer.Serialize(ranked.Take(8)); // أفضل 8 أخبار
            await writer.WriteAsync($"data: {json}\n\n");
            await writer.FlushAsync();

            await Task.Delay(3000, HttpContext.RequestAborted);
        }
    }
}