using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerCambioRestricao.APPLICATION.Services
{
    public interface IFtpClientWrapper
    {
        void Connect();
        bool FileExists(string path);
        void DownloadFile(string localPath, string remotePath);
        void UploadFile(string localPath, string remotePath);
        void DeleteFile(string path);
    }
}
