using LiveNewsAggregator.Models;

namespace LiveNewsAggregator.Services;

public class NewsRankingService
{
    public List<NewsItem> RankNewsForUser(List<NewsItem> allNews, UserPreferences prefs)
    {
        return allNews
            .OrderByDescending(news =>
            {
                int score = 0;

                if (prefs.Interests.Contains(news.Category))
                    score += 100;

                var ageMinutes = (DateTime.UtcNow - news.PublishedAt).TotalMinutes;
                score += (int)Math.Max(0, 50 - ageMinutes * 2);

                return score;
            })
            .ToList();
    }
}