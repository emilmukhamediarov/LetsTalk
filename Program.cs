using LetsTalk.Hubs;

namespace LetsTalk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.MapStaticAssets();
            app.MapHub<ChatHub>("/ChatHub");
            app.MapGet("/", () => Results.File("index.html", "text/html"));

            app.Run();
        }
    }
}
