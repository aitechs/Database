using AiTech.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace AiTech.ToolsTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var credential = new NetworkCredential
            {
                Domain = "172.16.0.16",
                UserName = "anonymous",         //AiTech.LiteOrm.Database.Connection.MyDbCredential.Username;
                Password = "user@yahoo.com" //AiTech.LiteOrm.Database.Connection.MyDbCredential.Password;
            };

            var ftp = new FtpClass(credential);

            var result = ftp.DownloadFile(@"d:\", "171128041112.jpg", "/idms/pictures");

            Assert.AreEqual(true, result);

            Assert.AreEqual(true, System.IO.File.Exists("D:\\171128041112.jpg"));
        }
    }
}
