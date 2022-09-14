using FluentFTP;
using Microsoft.Extensions.Options;
using System.Net.Security;
using VerificaFTP.Settings;

namespace VerificaFTP.Factories
{
    public class FactoryFtpClient
    {
        private readonly FtpSettings? _ftp;
        private readonly PathSettings? _settings;

        public FactoryFtpClient(IOptions<FtpSettings> optionsFtpSettings, IOptions<PathSettings> optionsSettings)
        {
            ArgumentNullException.ThrowIfNull(optionsFtpSettings, nameof(optionsFtpSettings));
            ArgumentNullException.ThrowIfNull(optionsSettings, nameof(optionsSettings));

            this._ftp = optionsFtpSettings!.Value;
            this._settings = optionsSettings!.Value;
        }

        public FtpClientWrapper GetFactory()
        {

            if (string.IsNullOrEmpty(_ftp!.Host)
             || string.IsNullOrEmpty(_ftp!.Username)
             || string.IsNullOrEmpty(_ftp!.Password))
            {
                throw new Exception("Dados de conexão com o FTP estão inválidos.");
            }

            FtpClient ftp = new(_ftp!.Host, _ftp.Port ?? 21, _ftp!.Username!, _ftp!.Password!)
            {
                EncryptionMode = FtpEncryptionMode.Explicit,
                DataConnectionEncryption = true,
            };

            ftp.ValidateCertificate += FluentFTP_ValidateCertificate;

            return new FtpClientWrapper(ftp, _settings!);
        }

        private static void FluentFTP_ValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            e.Accept = true;
            e.PolicyErrors = SslPolicyErrors.None;
        }
    }
}
