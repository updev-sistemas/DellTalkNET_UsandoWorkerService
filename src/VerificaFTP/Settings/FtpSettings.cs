namespace VerificaFTP.Settings
{
    public class FtpSettings
    {
        public virtual string? Host { get; set; }
        public virtual int? Port { get; set; }
        public virtual string? Username { get; set; }
        public virtual string? Password { get; set; }
    }
}
