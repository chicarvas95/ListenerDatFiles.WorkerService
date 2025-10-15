using Inmosat.ListenerDatFilesIridium.WorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Services.Interfaces
{
    public interface IFileService
    {
        Task ReadFilesFromFolder(CancellationToken stoppingToken);
        Task DownloadCdrFiles(CancellationToken stoppingToken);
        Task DeleteCdrFiles(CancellationToken stopppingToken);
    }
}
