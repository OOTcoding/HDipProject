using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace PM.MVCTests
{
    [TestFixture]
    public class SeleniumTests
    {
        [Test]
        public void IsLoginSuccessful()
        {
            using (var driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://projectmanagementplatform.azurewebsites.net/Identity/Account/Login");
                driver.FindElement(By.Id("Input_Email")).SendKeys("steve@gmail.com");
                driver.FindElement(By.Id("Input_Password")).SendKeys("Pass123!");
                driver.FindElement(By.CssSelector("input.btn.btn-primary")).Click();

                var actual = driver.Url;
                var expected = "https://projectmanagementplatform.azurewebsites.net/Home/Summary";

                Assert.AreEqual(expected,actual);
            }
        }

        [Test]
        public void CanCreateNewProject()
        {
            using (var driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://projectmanagementplatform.azurewebsites.net/Identity/Account/Login");
                driver.FindElement(By.Id("Input_Email")).SendKeys("steve@gmail.com");
                driver.FindElement(By.Id("Input_Password")).SendKeys("Pass123!");
                driver.FindElement(By.CssSelector("input.btn.btn-primary")).Click();

                driver.Navigate().GoToUrl("https://projectmanagementplatform.azurewebsites.net/Projects");

                driver.FindElement(By.CssSelector("a.btn.btn-primary.mb-5")).Click();

                driver.FindElement(By.Id("Name")).SendKeys("Test project");
                driver.FindElement(By.Id("FromDuration")).SendKeys("05/11/2020");
                driver.FindElement(By.Id("ToDuration")).SendKeys("31/01/2021");

                driver.FindElement(By.CssSelector("div>input.btn.btn-primary")).Click();

                var actual = driver.Url;
                var expected = "https://projectmanagementplatform.azurewebsites.net/Projects";

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
