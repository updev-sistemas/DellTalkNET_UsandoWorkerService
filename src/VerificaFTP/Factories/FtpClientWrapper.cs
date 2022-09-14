using FluentFTP;
using VerificaFTP.Settings;

namespace VerificaFTP.Factories
{
    public class FtpClientWrapper
    {
        private readonly FtpClient _ftp;
        private readonly PathSettings _settings;

        public FtpClientWrapper(FtpClient ftp, PathSettings settings)
        {
            this._ftp = ftp;
            this._settings = settings;
        }

        public FtpClient Ftp => this._ftp;
        public PathSettings Settings => this._settings;
    }
}
