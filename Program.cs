using Inmosat.ListenerDatFilesIridium.WorkerService;
using Inmosat.ListenerDatFilesIridium.WorkerService.Models;
using Inmosat.ListenerDatFilesIridium.WorkerService.Quartz;
using Inmosat.ListenerDatFilesIridium.WorkerService.Repositories;
using Inmosat.ListenerDatFilesIridium.WorkerService.Repositories.Interfaces;
using Inmosat.ListenerDatFilesIridium.WorkerService.Services;
using Inmosat.ListenerDatFilesIridium.WorkerService.Services.Interfaces;
using Quartz;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
                    @"C:\LogsListenerDatFilesIridium\ListenerDatFilesIridium.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true
                 )
    .CreateLogger();

builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IFileCdrIridiumRepository, FileCdrIridiumRepository>();
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<ConnectionStrings>(
    builder.Configuration.GetSection("ConnectionStrings"));


//var host = builder.Build();

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "ListenerCdrFilesIridium";
    })
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>(); // Tu clase Worker
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IFileCdrIridiumRepository, FileCdrIridiumRepository>();
        //services.AddQuartz(q =>
        //{
        //    var jobKey = new JobKey("JobLecturaEliminacionArchivosCDR");

        //    q.AddJob<Job>(opts => opts.WithIdentity(jobKey));

        //    q.AddTrigger(opts => opts
        //        .ForJob(jobKey)
        //        .WithIdentity("Trigger_LecutraEliminacionArchivosCDR")
        //        // Ejemplo: Cada 5 minutos
        //        .WithCronSchedule("0 0 6 * * ?")
        //    );
        //});
        //services.AddQuartzHostedService(options =>
        //{
        //    options.WaitForJobsToComplete = true;
        //});
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSerilog();
        logging.AddConsole(); // o Serilog, si usas Serilog
    })
    .Build();
host.Run();
