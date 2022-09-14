using FluentFTP;
using VerificaFTP.Factories;

namespace VerificaFTP
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly FtpClientWrapper? _ftpClient;

        public Worker(ILogger<Worker> logger, FactoryFtpClient factoryFtpClient)
        {
            this._logger = logger;
            this._ftpClient = factoryFtpClient.GetFactory();
        }

        private void VerifyIfFolderExists()
        {
            try
            {
                DirectoryInfo folder = new(this._ftpClient!.Settings!.PathToStorageFile!);

                if (!folder.Exists)
                {
                    folder!.Create();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problemas na criação de diretórios de controle da importação de arquivos.", ex);
            }
        }

        private async Task<FtpListItem[]>? RecoveryListXMLAsync(CancellationToken stoppingToken)
        {
            _ = await this._ftpClient!.Ftp.AutoConnectAsync(stoppingToken).ConfigureAwait(false);

            FtpListItem[]? ftpListItems = await this._ftpClient!.Ftp.GetListingAsync(stoppingToken).ConfigureAwait(false);

            return ftpListItems;
        }

        private void DownloadFilesAsync(FtpListItem[] itemsToDownload)
        {
            foreach (var item in itemsToDownload)
            {
                try
                {
                    var filename = new FileInfo(item.Name);

                    if (filename.Extension!.ToUpper() != ".JSON")
                        continue;

                    var directory = new DirectoryInfo(item.FullName);

                    var locaPathToSave = string.Format("{0}\\{1}", this._ftpClient!.Settings.PathToStorageFile, item.Name);

                    var result = this._ftpClient!.Ftp.DownloadFile(locaPathToSave, item.FullName);

                    if (result == FtpStatus.Success)
                    {
                        this._logger!.LogInformation($"Download do Arquivo {item.Name} concluido com sucesso.");
                        this._ftpClient!.Ftp.DeleteFile(item.FullName);
                    }
                    else if (result == FtpStatus.Skipped)
                    {
                        this._logger!.LogInformation($"O arquivo {item.Name} já existe no diretório corrente.");
                    }
                    else
                    {
                        this._logger!.LogWarning($"Ocorreu um erro ao processar o download do arquivo {item.Name}");
                    }

                    Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    this._logger!.LogError($"Ocorreu um erro ao processar o download do arquivo {item.Name}", ex);
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando a execução do VerificaFTP.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    this.VerifyIfFolderExists();

                    var list = await this.RecoveryListXMLAsync(stoppingToken)!.ConfigureAwait(false);
                    if (list == null)
                    {
                        throw new Exception("N�o foi possível recurar uma lista de itens no servidor.");
                    }

                    if (list!.Length > 0)
                    {
                        this.DownloadFilesAsync(list);
                    }
                }
                catch (Exception ex)
                {
                    this._logger!.LogError($"Ocorreu um erro geral", ex);
                }
                await Task.Delay(1000, stoppingToken);
            }
            _logger.LogInformation("Finalizando a execução do VerificaFTP.");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando o VerificaFTP.");
#if DEBUG
            return Task.Factory.StartNew(() => ExecuteAsync(cancellationToken), cancellationToken);
#else
            return base.StartAsync(cancellationToken);
#endif
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parando o VerificaFTP.");
            return base.StopAsync(cancellationToken);
        }
    }
}