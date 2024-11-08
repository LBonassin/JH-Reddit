namespace ConsoleApp1
{

    public class RedditResponse
    {
        public required Data Data { get; set; }
    }

    public class Data
    {
        public required Child[] Children { get; set; }
    }

    public class Child
    {
        public required Data1 Data { get; set; }
    }

    public class Data1
    {
        public required string Title { get; set; }       
        public required int Ups { get; set; }       
        public required string Author { get; set; }
       
    }
}
