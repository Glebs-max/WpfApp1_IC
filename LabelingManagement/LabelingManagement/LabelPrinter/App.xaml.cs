using LabelDesigner;
using LabelPrinter.Data;
using LabelPrinter.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace LabelPrinter
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var conn = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<AppDbContext>(options => options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<ProductsViewModel>();
                    services.AddTransient<LabelPreviewViewModel>();

                    services.AddTransient<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (AppHost != null)
            {
                await AppHost.StartAsync();

                MainViewModel mainVm = AppHost.Services.GetRequiredService<MainViewModel>();
                mainVm.CurrentViewModel = AppHost.Services.GetRequiredService<ProductsViewModel>();

                MainWindow mw = AppHost.Services.GetRequiredService<MainWindow>();
                mw.DataContext = mainVm;
                mw.Show();

                base.OnStartup(e);
            }
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            if (AppHost != null)
            {
                await AppHost.StopAsync();
                AppHost.Dispose();
                base.OnExit(e);
            }
        }
    }
}
