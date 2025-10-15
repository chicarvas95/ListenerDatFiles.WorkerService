using FluentFTP;
using Inmosat.ListenerDatFilesIridium.WorkerService.Services.Interfaces;
using System.Net;

namespace Inmosat.ListenerDatFilesIridium.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IFileService _fileService;

        public Worker(ILogger<Worker> logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _fileService.DownloadCdrFiles(stoppingToken);
            await _fileService.ReadFilesFromFolder(stoppingToken);
            await _fileService.DeleteCdrFiles(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
