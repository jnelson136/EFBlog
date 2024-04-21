using NLog;
using System.ComponentModel.Design;
using System.Linq;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");
string choice;

try
{
    do
    {
        Console.WriteLine("1) Display All Blogs");
        Console.WriteLine("2) Add Blog");
        Console.WriteLine("3) Create Post");
        Console.WriteLine("4) Display Posts");

        Console.WriteLine("Enter q To Quit");

        choice = Console.ReadLine();

        if (choice == "1")
        {
            var db = new BloggingContext();

            // Display all Blogs from the database
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine(item.Name);
            }
        }

        else if (choice == "2")
        {
            // Create and save a new Blog
            Console.Write("Enter a name for a new Blog: ");
            var name = Console.ReadLine();

            var blog = new Blog { Name = name };

            var db = new BloggingContext();
            db.AddBlog(blog);
            logger.Info("Blog added - {name}", name);

        }

        else if (choice == "3")
        {
            using (var db = new BloggingContext())
            {
                var blogs = db.Blogs.OrderBy(b => b.Name).ToList();
                if (blogs.Any())
                {
                    Console.WriteLine("Select a blog to post to:");
                    for (int i = 0; i < blogs.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}) {blogs[i].Name}");
                    }

                    Console.Write("Choose which blog you wish to write a post to (select the number): ");
                    if (int.TryParse(Console.ReadLine(), out int blogNumber) && blogNumber >= 1 && blogNumber <= blogs.Count) // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/out
                    {
                        var selectedBlog = blogs[blogNumber - 1];

                        Console.Write("Enter the Title ");
                        var title = Console.ReadLine();
                        Console.Write("Enter the Content: ");
                        var content = Console.ReadLine();

                        var post = new Post { Title = title, Content = content, BlogId = selectedBlog.BlogId };
                        db.Posts.Add(post);
                        db.SaveChanges();
                        logger.Info($"Posted to: '{selectedBlog.Name}': {title}");
                    }
                    else
                    {
                        Console.WriteLine("Please Select from the blog list.");
                    }
                }
                else
                {
                    Console.WriteLine("Please add a blog first.");
                }
            }
        }

        else if (choice == "4")
        {
            using (var db = new BloggingContext())
            {
                var blogs = db.Blogs.OrderBy(b => b.Name).ToList();
                if (blogs.Any())
                {
                    Console.WriteLine("Select a blog to view posts:");
                    for (int i = 0; i < blogs.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}) {blogs[i].Name}");
                    }

                    Console.Write("Choose which blog you wish to view posts on (select the number): ");
                    if (int.TryParse(Console.ReadLine(), out int blogNumber) && blogNumber >= 1 && blogNumber <= blogs.Count)
                    {
                        var selectedBlog = blogs[blogNumber - 1];

                        var posts = db.Posts.Where(p => p.BlogId == selectedBlog.BlogId).ToList();
                        if (posts.Any())
                        {
                            foreach (var post in posts)
                            {
                                Console.WriteLine($"Blog: {selectedBlog.Name}\n Title: {post.Title}\n Content: {post.Content}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("There are no posts.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid blog selection.");
                    }
                }
                else
                {
                    Console.WriteLine("No blogs available. Please add a blog first.");
                }
            }
        }

        else if (choice == "q")
        {
            break;
        }

        else
        {
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Invalid Option. Please Select From the Menu");
            Console.WriteLine("---------------------------------------------");
        }

    } while (choice != "q");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");
