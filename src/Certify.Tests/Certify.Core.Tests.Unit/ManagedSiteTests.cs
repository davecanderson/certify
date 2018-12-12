﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Certify.Management;
using Certify.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Certify.Core.Tests.Unit
{
    [TestClass]
    public class ManagedCertificateTests
    {
        private ManagedCertificate BuildTestManagedCertificate()
        {
            var testSite = new ManagedCertificate
            {
                Id = Guid.NewGuid().ToString(),
                Name = "TestSite..",
                GroupId = "test",

                RequestConfig = new CertRequestConfig
                {
                    PrimaryDomain = "testsite.com",
                    Challenges = new ObservableCollection<CertRequestChallengeConfig>(
                         new List<CertRequestChallengeConfig>
                         {
                            new CertRequestChallengeConfig{
                                ChallengeType="http-01"
                            }
                         }),
                    PerformAutoConfig = true,
                    PerformAutomatedCertBinding = true,
                    PerformChallengeFileCopy = true,
                    PerformExtensionlessConfigChecks = true,
                    WebsiteRootPath = "c:\\inetpub\\wwwroot"
                },
                ItemType = ManagedCertificateType.SSL_LetsEncrypt_LocalIIS
            };
            return testSite;
        }

        [TestMethod, Description("Ensure managed sites list loads")]
        public async Task TestLoadManagedCertificates()
        {
            var managedCertificateSettings = new ItemManager();
            managedCertificateSettings.StorageSubfolder = "Tests";
            ManagedCertificate testCert = BuildTestManagedCertificate();
            try
            {
                var managedCertificate = await managedCertificateSettings.UpdatedManagedCertificate(testCert);

                var managedCertificates = await managedCertificateSettings.GetManagedCertificates();
                Assert.IsTrue(managedCertificates.Count > 0);
            }
            finally
            {
                await managedCertificateSettings.DeleteManagedCertificate(testCert);
            }
        }

        [TestMethod, Description("Ensure mamaged site can be created, retrieved and deleted")]
        public async Task TestCreateDeleteManagedCertificate()
        {
            var itemManager = new ItemManager();
            itemManager.StorageSubfolder = "Tests";

            var testSite = new ManagedCertificate
            {
                Id = Guid.NewGuid().ToString(),
                Name = "TestSite..",
                GroupId = "test",

                RequestConfig = new CertRequestConfig
                {
                    PrimaryDomain = "testsite.com",
                    Challenges = new ObservableCollection<CertRequestChallengeConfig>(
                        new List<CertRequestChallengeConfig>
                        {
                            new CertRequestChallengeConfig{
                                ChallengeType="http-01"
                            }
                        }),
                    PerformAutoConfig = true,
                    PerformAutomatedCertBinding = true,
                    PerformChallengeFileCopy = true,
                    PerformExtensionlessConfigChecks = true,
                    WebsiteRootPath = "c:\\inetpub\\wwwroot"
                },
                ItemType = ManagedCertificateType.SSL_LetsEncrypt_LocalIIS
            };

            var managedCertificate = await itemManager.UpdatedManagedCertificate(testSite);

            Assert.IsNotNull(managedCertificate, "Create/store managed site");

            //check site now exists
            managedCertificate = await itemManager.GetManagedCertificate(testSite.Id);
            Assert.IsNotNull(managedCertificate, "Retrieve managed site");

            await itemManager.DeleteManagedCertificate(managedCertificate);
            managedCertificate = await itemManager.GetManagedCertificate(testSite.Id);

            // now check site has been delete
            Assert.IsNull(managedCertificate, "Managed site deleted");
        }
    }
}
