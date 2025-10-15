using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Models
{
    public class MocRecord
    {
        public string RecordType { get; set; } = "";
        public string IMEI { get; set; } = "";
        public string TypeOfNumber { get; set; } = "";
        public string NumberingPlan { get; set; } = "";
        public string CalledNumber { get; set; } = "";
        public string ServiceType { get; set; } = "";
        public string ServiceCode { get; set; } = "";
        public string DualServiceType { get; set; } = "";
        public string DualServiceCode { get; set; } = "";
        public string RadioChannelRequested { get; set; } = "";
        public string RadioChannelUsed { get; set; } = "";
        public string MscId { get; set; } = "";
        public string LocationAreaCode { get; set; } = "";
        public string CellId { get; set; } = "";
        public string ChargingDate { get; set; } = "";
        public string ChargeStartTime { get; set; } = "";
        public string UtcOffsetCode { get; set; } = "";
        public string ChargeableUnits { get; set; } = "";
        public string DataVolumenReference { get; set; } = "";
        public string Charge { get; set; } = "";
        public string ChargeItem { get; set; } = "";
        public string TaxRateCode { get; set; } = "";
        public string ExchangeRateCode { get; set; } = "";
        public string OriginatingNetwork { get; set; } = "";
        public string SDFId { get; set; } = "";


    }
}
