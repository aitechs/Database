﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Winform
{
    public class cFTPEventHandlerArgs : EventArgs
    {
        public string FileName;
        public long TotalBytes;
        public long CompletedBytes;
    }


    public class FtpClass
    {
        private readonly NetworkCredential _Credential;
        private FileSystemWatcher _FolderWatcher;
        private readonly ICollection<string> ListOfFiles = new List<string>();

        /// <summary>
        /// Use in MonitorFolder. FTP Destination Path
        /// </summary>
        private string _FTPDefaultPath;

        public string LastError { get; set; }
        public event EventHandler<cFTPEventHandlerArgs> Progress;
        public event EventHandler<cFTPEventHandlerArgs> Completed;

        public FtpClass(NetworkCredential credential)
        {
            _Credential = credential;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="FTPAddress">Remote Address. Remove slash (/) on both ends </param>
        /// <returns></returns>
        public bool UploadFile(string filePath, string FTPAddress)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            FtpWebRequest requestFTP = (FtpWebRequest)WebRequest.Create("ftp://" + _Credential.Domain + "/" + FTPAddress + "/" + fileInfo.Name);

            // Credentials
            requestFTP.Credentials = new NetworkCredential(_Credential.UserName, _Credential.Password);


            Debug.WriteLine(requestFTP.RequestUri);

            requestFTP.KeepAlive = true;
            requestFTP.Method = WebRequestMethods.Ftp.UploadFile;
            requestFTP.UseBinary = true;
            requestFTP.ContentLength = fileInfo.Length;

            requestFTP.UsePassive = true;

            // The buffer size is set to 2kb, breaking down into 2kb and uploading
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            long BytesUploaded = 0;
            try
            {
                FileStream fs = fileInfo.OpenRead();

                Stream strm = requestFTP.GetRequestStream();

                // Read from the file stream 2kb at a time
                var contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    BytesUploaded += contentLen;
                    OnProgress(new cFTPEventHandlerArgs() { TotalBytes = fileInfo.Length, CompletedBytes = BytesUploaded });
                }
                // Close the file stream and the Request Stream
                strm.Close();
                fs.Close();

                OnCompleted(new cFTPEventHandlerArgs() { FileName = fileInfo.Name });
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <param name="fileName"></param>
        /// <param name="ServerFolderPath">Remote Address. Add slash(/) in the beginning ONLY</param>
        /// <returns></returns>
        public Boolean DownloadFile(string destinationFolder, string fileName, string ServerFolderPath)
        {
            using (FileStream outputStream = new FileStream(Path.Combine(destinationFolder, fileName), FileMode.Create))
            {
                Debug.WriteLine("Connecting to Server");

                var url = new Uri(new Uri("ftp://" + _Credential.Domain), ServerFolderPath + "/" + fileName);

                // Query size of the file to be downloaded
                // =======================================
                var sizeRequest = (FtpWebRequest)WebRequest.Create(url);
                sizeRequest.Credentials = new NetworkCredential(_Credential.UserName, _Credential.Password);
                sizeRequest.Method = WebRequestMethods.Ftp.GetFileSize;

                var sizeOfFile = sizeRequest.GetResponse().ContentLength;

                //=========================================


                var requestFTP = (FtpWebRequest)WebRequest.Create(url);
                //var requestFTP = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + FTPAddress + "/" + fileName));

                Debug.WriteLine("Server " + requestFTP.RequestUri);
                Debug.WriteLine(requestFTP.RequestUri);

                requestFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                requestFTP.UseBinary = true;
                requestFTP.Credentials = new NetworkCredential(_Credential.UserName, _Credential.Password);

                //Use TRUE for Download
                requestFTP.UsePassive = true;

                var response = (FtpWebResponse)requestFTP.GetResponse();
                var ftpStream = response.GetResponseStream();

                var bufferSize = 2048;
                byte[] buffer = new byte[bufferSize];
                long bytesDownloaded = 0;

                if (ftpStream != null)
                {
                    var readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                        bytesDownloaded += readCount;

                        OnProgress(new cFTPEventHandlerArgs() { TotalBytes = sizeOfFile, CompletedBytes = bytesDownloaded });
                    }
                }

                if (ftpStream != null) ftpStream.Close();
                outputStream.Close();

                response.Close();
            }

            OnCompleted(new cFTPEventHandlerArgs() { FileName = fileName });

            return true;
        }


        protected virtual void OnProgress(cFTPEventHandlerArgs e)
        {
            Progress?.Invoke(this, e);
        }

        protected virtual void OnCompleted(cFTPEventHandlerArgs e)
        {
            Completed?.Invoke(this, e);
        }

        /// <summary>
        /// Watch Folder for any created file. then Automatically Uploads it.
        /// </summary>
        /// <param name="FolderPath">Folder to Watch</param>
        /// <param name="FTPServerPath">FTP Path to where the file will be uploaded</param>
        public void MonitorFolder(string FolderPath, string FTPServerPath)
        {
            _FolderWatcher = new FileSystemWatcher(FolderPath, "*.jpg")
            {
                EnableRaisingEvents = true,
                //SynchronizingObject = (ISynchronizeInvoke) this,
                NotifyFilter = NotifyFilters.FileName
            };
            _FolderWatcher.Created += _FolderWatcher_Created;
            _FolderWatcher.Changed += _FolderWatcher_Changed;
            _FTPDefaultPath = FTPServerPath;
        }

        private void _FolderWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine(e.Name);

            //var filename = ListOfFiles.FirstOrDefault(f => f == e.FullPath) ;
            //if (String.IsNullOrEmpty(filename))
            //{
            //    UploadFile(e.FullPath, _FTPDefaultPath);
            //}
        }

        private void _FolderWatcher_Created(object sender, FileSystemEventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine(e.Name);

            while (true)
            {
                try
                {
                    FileInfo info = new FileInfo(e.FullPath);
                    var stream = File.Open(e.FullPath, FileMode.Open);
                    stream.Close();
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(200);
                }
            }

            UploadFile(e.FullPath, _FTPDefaultPath);
        }
    }
}

