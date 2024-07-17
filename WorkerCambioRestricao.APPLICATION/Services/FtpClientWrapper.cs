using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerCambioRestricao.APPLICATION.Services
{
    public class FtpClientWrapper : IFtpClientWrapper
    {
        private readonly FtpClient _ftpClient;

        public FtpClientWrapper(string host, string user, string pass)
        {
            _ftpClient = new FtpClient(host, user, pass);
        }

        public void Connect() => _ftpClient.Connect();
        public bool FileExists(string path) => _ftpClient.FileExists(path);
        public void DownloadFile(string localPath, string remotePath) => _ftpClient.DownloadFile(localPath, remotePath);
        public void UploadFile(string localPath, string remotePath) => _ftpClient.UploadFile(localPath, remotePath);
        public void DeleteFile(string path) => _ftpClient.DeleteFile(path);
    }
}
