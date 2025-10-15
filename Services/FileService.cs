using FluentFTP;
using Inmosat.ListenerDatFilesIridium.WorkerService.Models;
using Inmosat.ListenerDatFilesIridium.WorkerService.Repositories.Interfaces;
using Inmosat.ListenerDatFilesIridium.WorkerService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private string host = "host";
        private readonly IFileCdrIridiumRepository _repository;
        private string user = "user";
        private string pass = "pass";
        private string remotePath = "/";
        private string localPath = @"C:\FtpCdrFiles";
        
        public FileService(ILogger<FileService> logger, IFileCdrIridiumRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task DownloadCdrFiles(CancellationToken stoppingToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
                _logger.LogInformation("Se creó la carpeta local: {path}", localPath);
            }

            var client = new AsyncFtpClient(host, user, pass, 21, new FtpConfig
            {
                ConnectTimeout = 120000,
                ReadTimeout = 120000,
                DataConnectionConnectTimeout = 120000,
                DataConnectionReadTimeout = 120000,
                EncryptionMode = FtpEncryptionMode.None,
                DataConnectionType = FtpDataConnectionType.PASV,
                ValidateAnyCertificate = true
            });

            try
            {
                await client.Connect(stoppingToken);
                _logger.LogInformation("Conexion establecida con exito.");
                DateTime hoy = DateTime.Today;
                //DateTime ayer = hoy.AddDays(-1);
                var archivos = await client.GetListing(remotePath, FtpListOption.AllFiles, stoppingToken);

                foreach (var item in archivos.Where(x => x.Type == FtpObjectType.File))
                {
                    if (item.Modified != DateTime.MinValue)
                    {
                        var fecha = item.Modified.Date;
                        if (fecha == hoy)
                        {
                            _logger.LogInformation($"Archivo válido: {item.FullName} - Fecha: {item.Modified}");
                            // Descargar archivo
                            string localFilePath = Path.Combine(localPath, item.Name);
                            if (File.Exists(localFilePath))
                            {
                                _logger.LogInformation($"El archivo {localFilePath} ya existe y no sera descargado");
                            }
                            else
                            {
                                await client.DownloadFile(localFilePath, item.FullName, FtpLocalExists.Overwrite, FtpVerify.None);
                                _logger.LogInformation("Archivo {archivo} descargado en {path}", item.FullName, localPath);
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"Archivo omitido: {item.FullName} - Fecha: {item.Modified}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"No se pudo obtener fecha de {item.FullName}");
                    }
                }
                await client.Disconnect(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectarse al servidor FTP.");
            }
        }

        public async Task ReadFilesFromFolder(CancellationToken stoppingToken)
        {
            if (!Directory.Exists(localPath))
                _logger.LogError($"La carpeta no existe: {localPath}");
            string[] archivos = Directory.GetFiles(localPath, "*.dat");

            try
            {
                foreach (string archivo in archivos) // Por cada archivo
                {
                    
                        string contenido = await File.ReadAllTextAsync(archivo, stoppingToken);
                        List<string> bloques = new List<string>();

                        for (int i = 0; i < contenido.Length; i += 160) //Por cada 160 caracteres
                        {
                            string bloque = contenido.Substring(i, Math.Min(160, contenido.Length - i));
                            if (bloque.Length < 2) continue;

                            string recordType = bloque.Substring(0, 2);
                            //string MscId = bloque.Substring(88,15);

                            //if (MscId == "CELLULAR")
                            //{
                                
                            //}

                            switch (recordType)
                            {
                                case "20":
                                    string chargingDateParam = bloque.Substring(114, 6);
                                    string yy = chargingDateParam.Substring(0, 2);
                                    string MM = chargingDateParam.Substring(2, 2);
                                    string dd = chargingDateParam.Substring(4, 2);
                                    int year = 2000 + int.Parse(yy);
                                    int month = int.Parse(MM);
                                    int day = int.Parse(dd);

                                    DateTime fecha = new DateTime(year, month, day);
                                    string chargingDateFormateado = fecha.ToString("yyyy-MM-dd");

                                    var moc = new MocRecord
                                    {
                                        RecordType = recordType,
                                        IMEI = bloque.Substring(9, 15).Trim(),
                                        TypeOfNumber = bloque.Substring(41, 1),
                                        NumberingPlan = bloque.Substring(43, 1),
                                        CalledNumber = bloque.Substring(43, 21).Trim(),
                                        ServiceType = bloque.Substring(64, 1),
                                        ServiceCode = bloque.Substring(65, 2).Trim(),
                                        DualServiceType = bloque.Substring(67, 1),
                                        DualServiceCode = bloque.Substring(68, 2),
                                        RadioChannelRequested = bloque.Substring(70, 1),
                                        RadioChannelUsed = bloque.Substring(71, 1),
                                        MscId = bloque.Substring(88, 15),
                                        LocationAreaCode = bloque.Substring(103, 5),
                                        CellId = bloque.Substring(108, 5).Trim(),
                                        ChargingDate = chargingDateFormateado,
                                        ChargeStartTime = bloque.Substring(120, 6),
                                        UtcOffsetCode = bloque.Substring(126, 1),
                                        ChargeableUnits = bloque.Substring(127, 6),
                                        DataVolumenReference = bloque.Substring(133, 6),
                                        Charge = bloque.Substring(139, 9),
                                        ChargeItem = bloque.Substring(148, 1),
                                        TaxRateCode = bloque.Substring(149, 1),
                                        ExchangeRateCode = bloque.Substring(150, 1),
                                        OriginatingNetwork = bloque.Substring(151, 6),
                                        SDFId = bloque.Substring(157, 3)
                                    };
                                    //await _repository.SaveMocRecordAsync(moc);
                                break;

                                case "30":
                                          string chargingDateParam2 = bloque.Substring(114, 6);
                                        var moc2 = new MocRecord
                                        {
                                            RecordType = recordType,
                                            IMEI = bloque.Substring(9, 15).Trim(),
                                            TypeOfNumber = bloque.Substring(41, 1),
                                            NumberingPlan = bloque.Substring(43, 1),
                                            CalledNumber = bloque.Substring(43, 21).Trim(),
                                            ServiceType = bloque.Substring(64, 1),
                                            ServiceCode = bloque.Substring(65, 2).Trim(),
                                            DualServiceType = bloque.Substring(67, 1),
                                            DualServiceCode = bloque.Substring(68, 2),
                                            RadioChannelRequested = bloque.Substring(70, 1),
                                            RadioChannelUsed = bloque.Substring(71, 1),
                                            MscId = bloque.Substring(88, 15),
                                            LocationAreaCode = bloque.Substring(103, 5),
                                            CellId = bloque.Substring(108, 5).Trim(),
                                            ChargingDate = "2025-10-10",
                                            ChargeStartTime = bloque.Substring(120, 6),
                                            UtcOffsetCode = bloque.Substring(126, 1),
                                            ChargeableUnits = bloque.Substring(127, 6),
                                            DataVolumenReference = bloque.Substring(133, 6),
                                            Charge = bloque.Substring(139, 9),
                                            ChargeItem = bloque.Substring(148, 1),
                                            TaxRateCode = bloque.Substring(149, 1),
                                            ExchangeRateCode = bloque.Substring(150, 1),
                                            OriginatingNetwork = bloque.Substring(151, 6),
                                            SDFId = bloque.Substring(157, 3)
                                        };
                                        await _repository.SaveMocRecordAsync(moc2);
                                break;
                            }
                        }
                }
            }
            catch(Exception ex) 
            {
                _logger.LogError($"Ocurrio un error en el metodo {nameof(ReadFilesFromFolder)}, detalle del error: {ex.Message}");
            }
        }

        public async Task DeleteCdrFiles(CancellationToken stopppingToken)
        {
            try
            {
                // Eliminar archivos de forma asíncrona
                string[] archivos = Directory.GetFiles(localPath);
                foreach (string archivo in archivos)
                {
                    await Task.Run(() => File.Delete(archivo));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error en el metodo {nameof(DeleteCdrFiles)}, detalle del error: {ex.Message}");
            }

        }
    }
}
