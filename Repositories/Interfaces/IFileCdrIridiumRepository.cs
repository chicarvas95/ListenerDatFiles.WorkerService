using Inmosat.ListenerDatFilesIridium.WorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Repositories.Interfaces
{
    public interface IFileCdrIridiumRepository
    {
        Task SaveMocRecordAsync(MocRecord dataMocRecord);
    }
}
