using ConsoleApp1;

public class Program
{
    private static readonly StatisticsTracker _tracker = new StatisticsTracker();
    private static RedditService _redditService;
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public static async Task Main(string[] args)
    {
        string  subreddit = "motorcycles"; 
        string  clientId = "VtQSn9Hb_N1lcA6zfFi_bg";
        string  clientSecret = "wTOaOppZCIEjvbTtJ23_BsekM99lvQ";

        _redditService = new RedditService(subreddit, clientId, clientSecret);
        await _redditService.InitializeAsync();

        var fetchingTask = Task.Run(FetchPostsPeriodically);
        var loggingTask = Task.Run(LogStatisticsPeriodically);

        await Task.WhenAll(fetchingTask, loggingTask);
    }

    private static async Task FetchPostsPeriodically()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var posts = await _redditService.FetchPostsAsync();
                _tracker.ProcessPosts(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting posts: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }

    private static async Task LogStatisticsPeriodically()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            _tracker.PrintStatistics();
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}
