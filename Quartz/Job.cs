using Inmosat.ListenerDatFilesIridium.WorkerService.Services.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Quartz
{
    public class Job : IJob
    {
        private readonly ILogger<Job> _logger;
        private readonly IFileService _fileService;

        public Job(ILogger<Job> logger,
                   IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Descargando archivos CDR a la Hora: {Hora}", DateTime.Now);
            await _fileService.DownloadCdrFiles(context.CancellationToken);
            _logger.LogInformation("Archivos CDR descargados a la Hora: {Hora}", DateTime.Now);
            _logger.LogInformation("Empezando la lectura de los archivos a la Hora: {Hora}", DateTime.Now);
            await _fileService.ReadFilesFromFolder(context.CancellationToken);
            _logger.LogInformation("Terminando la lectura de los archivos a la Hora: {Hora}", DateTime.Now);
            await _fileService.DeleteCdrFiles(context.CancellationToken);
            _logger.LogInformation("Archivos CDR eliminados en la fecha: {Hora}", DateTime.Now);
        }
    }
}
