using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Extensions;

namespace KeithLink.Svc.Test.Extensions
{
    [TestClass]
    public class InvoiceExtensionsTest
    {
        [TestMethod]
        public void CustomerToCustomerWithInvoicesShouldTransferAllInformation()
        {
            // Arrange
            Customer mockCustomer = new Customer()
            {
                CustomerNumber = "11111",
                CustomerName = "Test Customer",
                DisplayName = "Test Customer",
                NationalOrRegionalAccountNumber = "22222",
                ContractId = "33333",
                CustomerId = new Guid("44444444444444444444444444444444"),
                AccountId = new Guid("55555555555555555555555555555555"),
                Phone = "666-666-6666",
                Email = "someone@gmail.com",
                NationalId = "77777",
                NationalNumber = "88888",
                NationalSubNumber = "99999",
                NationalIdDesc = "10",
                NationalNumberSubDesc = "11",
                RegionalId = "12",
                RegionalIdDesc = "13",
                RegionalNumber = "14",
                RegionalNumberDesc = "15"
            };
            CustomerWithInvoices expected = new CustomerWithInvoices()
            {
                CustomerNumber = mockCustomer.CustomerNumber,
                CustomerName = mockCustomer.CustomerName,
                DisplayName = mockCustomer.DisplayName,
                NationalOrRegionalAccountNumber = mockCustomer.NationalOrRegionalAccountNumber,
                ContractId = mockCustomer.ContractId,
                CustomerId = mockCustomer.CustomerId,
                AccountId = mockCustomer.AccountId,
                Phone = mockCustomer.Phone,
                Email = mockCustomer.Email,
                NationalId = mockCustomer.NationalId,
                NationalNumber = mockCustomer.NationalNumber,
                NationalSubNumber = mockCustomer.NationalSubNumber,
                NationalIdDesc = mockCustomer.NationalIdDesc,
                NationalNumberSubDesc=mockCustomer.NationalNumberSubDesc,
                RegionalId=mockCustomer.RegionalId,
                RegionalIdDesc=mockCustomer.RegionalIdDesc,
                RegionalNumber=mockCustomer.RegionalNumber,
                RegionalNumberDesc=mockCustomer.RegionalNumberDesc
            };
            // Action
            CustomerWithInvoices actual = mockCustomer.ToCustomerWithInvoices();
            // Assert
            Assert.AreEqual<string>(expected.CustomerNumber, actual.CustomerNumber);
            Assert.AreEqual<string>(expected.CustomerName, actual.CustomerName);
            Assert.AreEqual<string>(expected.DisplayName, actual.DisplayName);
            Assert.AreEqual<string>(expected.NationalOrRegionalAccountNumber, actual.NationalOrRegionalAccountNumber);
            Assert.AreEqual<string>(expected.ContractId, actual.ContractId);
            Assert.AreEqual<Guid>(expected.CustomerId, actual.CustomerId);
            Assert.AreEqual<Guid?>(expected.AccountId, actual.AccountId);
            Assert.AreEqual<string>(expected.Phone, actual.Phone);
            Assert.AreEqual<string>(expected.Email, actual.Email);
            Assert.AreEqual<string>(expected.NationalId, actual.NationalId);
            Assert.AreEqual<string>(expected.NationalNumber, actual.NationalNumber);
            Assert.AreEqual<string>(expected.NationalSubNumber, actual.NationalSubNumber);
            Assert.AreEqual<string>(expected.NationalIdDesc, actual.NationalIdDesc);
            Assert.AreEqual<string>(expected.NationalNumberSubDesc, actual.NationalNumberSubDesc);
            Assert.AreEqual<string>(expected.RegionalId, actual.RegionalId);
            Assert.AreEqual<string>(expected.RegionalIdDesc, actual.RegionalIdDesc);
            Assert.AreEqual<string>(expected.RegionalNumber, actual.RegionalNumber);
            Assert.AreEqual<string>(expected.RegionalNumberDesc, actual.RegionalNumberDesc);
        }
    }
}
