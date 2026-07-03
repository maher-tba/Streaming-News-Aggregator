using System.Threading.Channels;
using LiveNewsAggregator.Models;

namespace LiveNewsAggregator.Services;

public class NewsGeneratorService : BackgroundService
{
    private readonly Channel<NewsItem> _channel = Channel.CreateUnbounded<NewsItem>();
    private readonly List<NewsItem> _allNews = new();
    private readonly Random _random = new Random();
    private readonly object _lock = new object();

    private readonly string[] _categories = { "سياسة", "اقتصاد", "تكنولوجيا", "رياضة", "صحة", "علوم", "عالم", "محلي", "ثقافة" };

    public ChannelReader<NewsItem> Reader => _channel.Reader;

    public List<NewsItem> GetAllNews()
    {
        lock (_lock) return _allNews.ToList();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int id = 1;
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(4000, stoppingToken);

            var category = _categories[_random.Next(_categories.Length)];
            var title = GetRandomTitle(category);

            var news = new NewsItem
            {
                Id = id++,
                Title = title,
                Summary = $"خبر عاجل: {title} في مجال {category}",
                Category = category,
                PublishedAt = DateTime.UtcNow,
                ImageUrl = $"https://picsum.photos/420/240?random={id}"
            };

            lock (_lock) _allNews.Add(news);
            await _channel.Writer.WriteAsync(news, stoppingToken);
        }
    }

    private string GetRandomTitle(string category)
    {
        return category switch
        {
            "سياسة" => "اجتماع طارئ لمجلس الوزراء حول الأزمة الاقتصادية",
            "اقتصاد" => "ارتفاع كبير في أسعار الذهب والدولار",
            "تكنولوجيا" => "إطلاق تقنية ذكاء اصطناعي جديدة",
            "رياضة" => "المنتخب السوري يحقق فوزاً مهماً",
            "صحة" => "تحذير من انتشار مرض جديد",
            _ => $"خبر هام في {category}"
        };
    }
}