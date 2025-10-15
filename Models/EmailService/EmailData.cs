using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Models.EmailService
{
    public class EmailData
    {
        public string recipientEmail { get; set; } = string.Empty;
        public string recipientName { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;
        public string htmlMessage { get; set; } = string.Empty;
    }
}
