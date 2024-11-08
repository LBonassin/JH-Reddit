
namespace ConsoleApp1
{
    public class StatisticsTracker
    {
        private Dictionary<string, Child> _topPosts = new Dictionary<string, Child>();
        private Dictionary<string, HashSet<string>> _userPosts = new Dictionary<string, HashSet<string>>(); //Automatically makes unique/dis    tinct
        private readonly object _lock = new object();

        public void ProcessPosts(Child[] posts)
        {
            lock (_lock)
            {
                foreach (var post in posts)
                {
                    if (_topPosts.ContainsKey(post.Data.Title))                    
                        _topPosts[post.Data.Title] = post;                    
                    else
                        _topPosts.Add(post.Data.Title, post);                    


                    if (_userPosts.ContainsKey(post.Data.Author))
                        _userPosts[post.Data.Author].Add(post.Data.Title);
                    else
                        _userPosts[post.Data.Author] = new HashSet<string> { post.Data.Title};
                }
            }
        }

        public void PrintStatistics()
        {
            lock (_lock)
            {
                if (_topPosts.Count == 0 && _userPosts.Count() == 0)
                {
                    Console.WriteLine("Gathering Data...");
                    return;
                }

                var maxLength = _topPosts.OrderByDescending(x => x.Value.Data.Ups).First().Value.Data.Ups.ToString().Length;

                Console.WriteLine("\n\nTop Posts by Updoots:");
                foreach (var pair in _topPosts.OrderByDescending(x => x.Value.Data.Ups))
                    Console.WriteLine($"{pair.Value.Data.Ups.ToString().PadLeft(maxLength)}    {pair.Value.Data.Title}  by  {pair.Value.Data.Author}");

                maxLength = _userPosts.OrderByDescending(x => x.Key.Length).First().Key.Length;

                Console.WriteLine("\n\nUsers with Most Posts:");
                foreach (var user in _userPosts.OrderByDescending(x => x.Value.Count()))
                    Console.WriteLine($"{user.Key.PadRight(maxLength)}  {user.Value.Count().ToString().PadLeft(4)} {(user.Value.Count() == 1 ? "post":"posts")}");
            }
        }
    }

}
