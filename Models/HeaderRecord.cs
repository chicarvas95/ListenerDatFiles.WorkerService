using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Models
{
    public class HeaderRecord
    {
        public string RecordType { get; set; } = "";
        public string Sender { get; set; } = "";
        public string Recipient { get; set; } = "";
        public string FileSequenceNumber { get; set; } = "";
        public string FileCreationDate { get; set; } = "";
        public string FileTransmissionDate { get; set; } = "";
        public string CutOffTimeStamp { get; set; } = "";
        public string UtcTimeOffset { get; set; } = "";
        public string CountryCode { get; set; } = "";
    }
}
