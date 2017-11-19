using AiTech.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace AiTech.ToolsTests
{
    [TestClass]
    public class UnitTest1
    {
        private NetworkCredential GetCredential()
        {
            return new NetworkCredential
            {
                Domain = "sbomexico.aitech.solutions",
                UserName = "anonymous",         //AiTech.LiteOrm.Database.Connection.MyDbCredential.Username;
                Password = "user@yahoo.com" //AiTech.LiteOrm.Database.Connection.MyDbCredential.Password;
            };

        }

        [TestMethod]
        public void TestFtpDownload()
        {
            var credential = GetCredential();

            var ftp = new FtpClass(credential);

            var testFileName = "idms.exe";//"downloadtest.jpg";
            var testFilePath = "d:\\";

            Debug.Print("Removing local file if exists");

            if (File.Exists(Path.Combine(testFilePath, testFileName)))
                File.Delete(Path.Combine(testFilePath, testFileName));

            Debug.Print("Start downloading File");

            var result = ftp.DownloadFile(testFilePath, testFileName,"/idms/installer");//"/idms/pictures");

            Assert.AreEqual(true, result);

            Assert.AreEqual(true, System.IO.File.Exists(Path.Combine(testFilePath, testFileName)));
        }


        [TestMethod]
        public void TestFtpUpload()
        {
            var credential = GetCredential();

            var ftp = new FtpClass(credential);
            ftp.Progress += Ftp_Progress;

            var testFileName = "uploadtest.jpg";
            var testFilePath = "d:\\";


            if (!File.Exists(Path.Combine(testFilePath, testFileName)))
                throw new System.Exception("Local File Not Exists!");


            Debug.Print("Start Uploading File");

            var result = ftp.UploadFile(Path.Combine(testFilePath, testFileName), "idms/pictures");

            Assert.AreEqual(true, result);
        }

        private void Ftp_Progress(object sender, cFTPEventHandlerArgs e)
        {
            Debug.Print(e.CompletedBytes.ToString());
        }

        [TestMethod]
        public void TestMessage()
        {
            try
            {
                throw new System.Exception("Error Ako");

            }
            catch (System.Exception ex)
            {

                Tools.Winform.MessageDialog.ShowError(ex, null);
            }
        }
    }
}
