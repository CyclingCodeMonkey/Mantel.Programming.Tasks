using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Mantel.Programming.Tasks.Models;
using Mantel.Programming.Tasks.Repository;
using Mantel.Programming.Tasks.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Mantel.Programming.Tasks.Services.UnitTests
{
    [TestClass]
    public class LogFileAnalyserServiceTests
    {
        [TestMethod]
        public async Task ProcessFile_ReadFileMissingFile_ReturnsException()
        {
            var mockFileHandling = new Mock<IFileHandlingService>();
            var mockRepository = new Mock<IDataStore>();
            mockFileHandling.Setup(m => m.ReadLogFileAsync(It.IsAny<string>()))
                .Throws<ArgumentException>();
            var target = new LogFileAnalyserService(mockFileHandling.Object, mockRepository.Object);

            var result = await target.ProcessFileAsync(@"c:\temm\missingFile.txt");
            Assert.IsFalse(result.IsSuccessful);
        }

        [TestMethod]
        public async Task ProcessFile_ReadFile_ReturnsData()
        {
            var logData = new List<LogDataRaw>
            {
                new LogDataRaw
                {
                    IpAddress = "10.0.0.1",
                    Url = "/",
                    Protocol = "HTTP/1.1"
                }
            };

            var mockFileHandling = new Mock<IFileHandlingService>();
            var mockRepository = new Mock<IDataStore>();
            mockFileHandling.Setup(m => m.ReadLogFileAsync(It.IsAny<string>()))
                .ReturnsAsync(logData);
            var target = new LogFileAnalyserService(mockFileHandling.Object, mockRepository.Object);
            var result = await target.ProcessFileAsync(@"c:\temm\File.txt");
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsTrue(result.Data);

        }

        [TestMethod]
        public void TopVisitedUrls_EmptyList_ReturnsEmptyList()
        {
            var mockFileHandling = new Mock<IFileHandlingService>();
            var mockRepository = new Mock<IDataStore>();
            mockRepository.Setup(x => x.GetLogDataRaws()).Returns(new List<LogDataRaw>());

            var target = new LogFileAnalyserService(mockFileHandling.Object, mockRepository.Object);
            var urlResult = target.FindMostVisitedUrls(3);

            Assert.IsTrue(urlResult.IsSuccessful);
            Assert.AreEqual(0, urlResult.Data.Count());
        }

        [TestMethod]
        public void TopVisitedUrls_FourItems_ReturnsRecords()
        {
            var logData = new List<LogDataRaw>
            {
                new LogDataRaw
                {
                    IpAddress = "10.0.0.1",
                    Url = "/active",
                    Protocol = "HTTP/1.1"
                },
                new LogDataRaw
                {
                    IpAddress = "10.0.0.2",
                    Url = "/lost",
                    Protocol = "HTTP/1.1"
                },
                new LogDataRaw
                {
                    IpAddress = "10.0.0.3",
                    Url = "/found",
                    Protocol = "HTTP/1.1"
                },
                new LogDataRaw
                {
                    IpAddress = "10.0.0.2",
                    Url = "/Active",
                    Protocol = "HTTP/1.1"
                },
            };
            var mockFileHandling = new Mock<IFileHandlingService>();
            var mockRepository = new Mock<IDataStore>();
            mockRepository.Setup(x => x.GetLogDataRaws()).Returns(logData);

            var target = new LogFileAnalyserService(mockFileHandling.Object, mockRepository.Object);
            var urlResult = target.FindMostVisitedUrls(3);

            Assert.IsTrue(urlResult.IsSuccessful);
            Assert.AreEqual(3, urlResult.Data.Count());
            Assert.AreEqual("/active", urlResult.Data[0].Key);
            Assert.AreEqual(2, urlResult.Data[0].Value);
        }

        [TestMethod]
        public void TopActiveAddresses_FourItems_ReturnsRecords()
        {
            var logData = new List<LogDataRaw>
            {
                new LogDataRaw
                {
                    IpAddress = "10.0.0.1",
                    Url = "/active",
                    Protocol = "HTTP/1.1"
                },
                new LogDataRaw
                {
                    IpAddress = "10.0.0.2",
                    Url = "/lost",
                    Protocol = "HTTP/1.1"
                },
                new LogDataRaw
                {
                    IpAddress = "10.0.0.3",
                    Url = "/found",
                    Protocol = "HTTP/1.1"
                },
                new LogDataRaw
                {
                    IpAddress = "10.0.0.2",
                    Url = "/Active",
                    Protocol = "HTTP/1.1"
                },
            };
            var mockFileHandling = new Mock<IFileHandlingService>();
            var mockRepository = new Mock<IDataStore>();
            mockRepository.Setup(x => x.GetLogDataRaws()).Returns(logData);

            var target = new LogFileAnalyserService(mockFileHandling.Object, mockRepository.Object);
            var result = target.FindMostActiveAddresses(2);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(2, result.Data.Count());
            Assert.AreEqual("10.0.0.2", result.Data[0].Key);
            Assert.AreEqual(2, result.Data[0].Value);
        }

        [TestMethod]
        public void UniqueAddresses_FourItems_ReturnsThreeRecords()
        {
            var logData = new List<LogDataRaw>
            {
                new LogDataRaw
                {
                    IpAddress = "10.0.0.1",
                    Url = "/active",
                    Protocol = "HTTP/1.1"
                },
                new LogDataRaw
                {
                    IpAddress = "10.0.0.2",
                    Url = "/lost",
                    Protocol = "HTTP/1.1"
                },
                new LogDataRaw
                {
                    IpAddress = "10.0.0.3",
                    Url = "/found",
                    Protocol = "HTTP/1.1"
                },
                new LogDataRaw
                {
                    IpAddress = "10.0.0.2",
                    Url = "/Active",
                    Protocol = "HTTP/1.1"
                },
            };
            var mockFileHandling = new Mock<IFileHandlingService>();
            var mockRepository = new Mock<IDataStore>();
            mockRepository.Setup(x => x.GetLogDataRaws()).Returns(logData);

            var target = new LogFileAnalyserService(mockFileHandling.Object, mockRepository.Object);
            var result = target.CountUniqueIpAddresses();

            Assert.AreEqual(3, result);
        }
    }
}