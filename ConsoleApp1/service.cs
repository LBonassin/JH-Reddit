using System.Net.Http.Headers;
using ConsoleApp1;
using Newtonsoft.Json;

public class RedditService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _subreddit;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string _accessToken;
    public RedditService(string subreddit, string clientId, string clientSecret)
    {
        _subreddit = subreddit;
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    public async Task InitializeAsync()
    {
        _accessToken = await GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", _clientId);
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", _clientId);

        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });


        //following commented out code was to do the initial call to set up auth
        //string redirect = "http://localhost:8080";
        //string scope = "identity, edit, flair, history, modconfig, modflair, modlog, modposts, modwiki, mysubreddits, privatemessages, read, report, save, submit, subscribe, vote, wikiedit, wikiread".Replace(" ", "").Replace(",", " ");
        //var path = $"https://www.reddit.com/api/v1/authorize?client_id={_clientId}&response_type=code&state=RANDOM_STRING&redirect_uri={redirect}&duration=permanent&scope={scope}";
        //var t = new UriBuilder();
        //var uri = new Uri(path);


        var response = await _httpClient.PostAsync("https://www.reddit.com/api/v1/access_token", requestData);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to retrieve access token from Reddit. Status Code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        dynamic? tokenResponse = JsonConvert.DeserializeObject(jsonResponse);
        return tokenResponse?.access_token ?? "Failed to retrieve access token from Reddit.";
    }

    public async Task<Child[]> FetchPostsAsync()
    {
        // Check if access token needs to be refreshed (if it has expired, in a production scenario)
        if (string.IsNullOrEmpty(_accessToken))
        {
            _accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        var response = await _httpClient.GetAsync($"https://oauth.reddit.com/r/{_subreddit}");

        if (!response.IsSuccessStatusCode) throw new Exception($"Failed to get data from r/{_subreddit}.");

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var redditData = JsonConvert.DeserializeObject<RedditResponse>(jsonResponse);

        return redditData.Data.Children;
    }
}
