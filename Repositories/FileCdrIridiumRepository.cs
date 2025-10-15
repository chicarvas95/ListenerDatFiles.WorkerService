using Inmosat.ListenerDatFilesIridium.WorkerService.Models;
using Inmosat.ListenerDatFilesIridium.WorkerService.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Repositories
{
    public class FileCdrIridiumRepository : IFileCdrIridiumRepository
    {
        //private readonly ILogger<FileCdrIridiumRepository> _loggerRepo;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileCdrIridiumRepository> _logger;
        private readonly string _connectionString;
        public FileCdrIridiumRepository(IOptions<ConnectionStrings> options, ILogger<FileCdrIridiumRepository> logger)
        {
            _connectionString = "Server=10.1.1.204;Database=Inmosat;User Id=sa;Password=Admin$;TrustServerCertificate=True;";
            _logger = logger;
        }


        public async Task SaveMocRecordAsync(MocRecord dataMocRecord)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("_spInsMocRecordCdrFiles", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@RecordType", System.Data.SqlDbType.NVarChar, 5) { Value = dataMocRecord.RecordType });
                cmd.Parameters.Add(new SqlParameter("@Imei", System.Data.SqlDbType.NVarChar, 15) { Value = dataMocRecord.IMEI });
                cmd.Parameters.Add(new SqlParameter("@TypeOfNumber", System.Data.SqlDbType.NVarChar, 1) { Value = dataMocRecord.TypeOfNumber });
                cmd.Parameters.Add(new SqlParameter("@NumberingPlan", System.Data.SqlDbType.NVarChar, 1) { Value = dataMocRecord.NumberingPlan });
                cmd.Parameters.Add(new SqlParameter("@CalledNumber", System.Data.SqlDbType.NVarChar, 21) { Value = dataMocRecord.CalledNumber });
                cmd.Parameters.Add(new SqlParameter("@ServiceType", System.Data.SqlDbType.NVarChar, 2) { Value = dataMocRecord.ServiceType });
                cmd.Parameters.Add(new SqlParameter("@ServiceCode", System.Data.SqlDbType.NVarChar, 3) { Value = dataMocRecord.ServiceCode });
                cmd.Parameters.Add(new SqlParameter("@DualServiceType", System.Data.SqlDbType.NVarChar, 1) { Value = dataMocRecord.DualServiceType });
                cmd.Parameters.Add(new SqlParameter("@DualServiceCode", System.Data.SqlDbType.NVarChar, 2) { Value = dataMocRecord.DualServiceCode });
                cmd.Parameters.Add(new SqlParameter("@RadioChannelRequested", System.Data.SqlDbType.NVarChar, 2) { Value = dataMocRecord.RadioChannelRequested });
                cmd.Parameters.Add(new SqlParameter("@RadioChannelUsed", System.Data.SqlDbType.NVarChar, 2) { Value = dataMocRecord.RadioChannelUsed });
                cmd.Parameters.Add(new SqlParameter("@MscId", System.Data.SqlDbType.NVarChar, 15) { Value = dataMocRecord.MscId });
                cmd.Parameters.Add(new SqlParameter("@LocationAreaCode", System.Data.SqlDbType.NVarChar, 6) { Value = dataMocRecord.LocationAreaCode });
                cmd.Parameters.Add(new SqlParameter("@CellId", System.Data.SqlDbType.NVarChar, 6) { Value = dataMocRecord.CellId });
                cmd.Parameters.Add(new SqlParameter("@ChargingDate", System.Data.SqlDbType.NVarChar, 12) { Value = dataMocRecord.ChargingDate });
                cmd.Parameters.Add(new SqlParameter("@ChargeStartTime", System.Data.SqlDbType.NVarChar, 6) { Value = dataMocRecord.ChargeStartTime });
                cmd.Parameters.Add(new SqlParameter("@UtcOffsetCode", System.Data.SqlDbType.NVarChar, 2) { Value = dataMocRecord.UtcOffsetCode });
                cmd.Parameters.Add(new SqlParameter("@ChargeableUnits", System.Data.SqlDbType.NVarChar, 7) { Value = dataMocRecord.ChargeableUnits });
                cmd.Parameters.Add(new SqlParameter("@DataVolumenReference", System.Data.SqlDbType.NVarChar, 7) { Value = dataMocRecord.DataVolumenReference });   
                cmd.Parameters.Add(new SqlParameter("@Charge", System.Data.SqlDbType.NVarChar, 10) { Value = dataMocRecord.Charge });
                cmd.Parameters.Add(new SqlParameter("@ChargeItem", System.Data.SqlDbType.NVarChar, 2) { Value = dataMocRecord.ChargeItem });
                cmd.Parameters.Add(new SqlParameter("@TaxRateCode", System.Data.SqlDbType.NVarChar, 2) { Value = dataMocRecord.TaxRateCode });
                cmd.Parameters.Add(new SqlParameter("@ExchangeRateCode", System.Data.SqlDbType.NVarChar, 2) { Value = dataMocRecord.ExchangeRateCode });
                cmd.Parameters.Add(new SqlParameter("@OriginatingNetwork", System.Data.SqlDbType.NVarChar, 7) { Value = dataMocRecord.OriginatingNetwork });
                cmd.Parameters.Add(new SqlParameter("@SDFId", System.Data.SqlDbType.NVarChar, 4) { Value = dataMocRecord.SDFId });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error en el metodo: {nameof(SaveMocRecordAsync)}, en la clase de repositorio: {nameof(FileCdrIridiumRepository)}, detalle del error: {ex.Message}");
                throw;
            }
        }
    }
}
