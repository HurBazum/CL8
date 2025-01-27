using CL8.UI.Infrastructure.Stores;
using CL8.UI.ViewModels.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CL8.DAL.Entities;
using System.Windows;
using CL8.DAL.Tools;

namespace CL8.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? _host;
        public static IHost Host => _host ?? Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

        public static User? CurrentUser { get; set; } 
        public static NavigationStore Store { get; set; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var host = Host;
            host.StartAsync();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var host = Host;
            base.OnExit(e);
            host.StopAsync().ConfigureAwait(false);
            host.Dispose();
        }

        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services) => services
            .AddDatabase()
            .AddRepositories()
            .AddViewModels();
    }
}