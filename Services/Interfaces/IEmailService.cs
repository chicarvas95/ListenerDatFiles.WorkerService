using Inmosat.ListenerDatFilesIridium.WorkerService.Models.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsyn(EmailData emailData);
    }
}
