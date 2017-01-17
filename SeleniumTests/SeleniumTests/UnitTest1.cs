using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace SeleniumTests
{
    [TestClass]
    public class SeleniumTests
    {
        private static string returnChromeDriverPath()
        {
            string path = Directory.GetCurrentDirectory();
            return path = path.Remove(path.Length - 9) + "ChromeDriver";
        }

        string url = "http://localhost:9946";
        string login = "admin";
        string password = "admin";
        string chromeDriverPath = returnChromeDriverPath();
        Helpers helper = new Helpers();

        [TestInitialize]
        public void testSetup()
        {
            IWebDriver driver = helper.getDriver();
            driver.Navigate().GoToUrl(url);
        }

        [TestMethod]
        public void testOpeningServerPage()
        {
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//input[@id='Username']")));
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//input[@id='Password']")));
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//input[@value='Login' and @type='submit']")));
        }

        [TestMethod]
        public void testLoginIntoApplication()
        {
            helper.loginIntoApplication(login, password);
            IWebElement element = helper.getDriver().FindElement(By.Id("nav-mobile"));
            Assert.AreEqual(true, helper.isElementPresent(By.Id("logo-container")));
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//input[@type='submit' and @value='Add']")));
            for(int i =0; i<4; i++)
            {
                Assert.AreEqual(true, helper.isElementPresent(By.XPath("//ul[@id='nav-mobile']//li["+ (i+1) +"]//a")));
                string textelement = helper.getElementText(By.XPath("//ul[@id='nav-mobile']//li[" + (i + 1) + "]//a"));
                if(i == 0)
                {
                    Assert.AreEqual("Mapa", textelement);
                } else if (i == 1)
                {
                    Assert.AreEqual("Admin Panel", textelement);
                } else if (i == 2)
                {
                    Assert.AreEqual("Symulator", textelement);
                } else if (i == 3)
                {
                    if(textelement.Contains("admin"))
                    {
                        textelement = "admin";
                    } else
                    {
                        textelement = "blabablblb";
                    }
                    Assert.AreEqual("admin", textelement);
                }
            }

        }

        [TestMethod]
        public void testLogOut()
        {
            helper.loginIntoApplication(login, password);
            helper.logOutFromTheApplication();
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//input[@id='Username']")));
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//input[@id='Password']")));
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//input[@value='Login' and @type='submit']")));
        }

        [TestMethod]
        public void testAddCar()
        {
            string VIN = helper.randomStringInteger(10);
            string brand = helper.RandomString(8);
            string model = helper.RandomString(8);
            helper.loginIntoApplication(login, password);
            helper.addCar(VIN, brand, model);         
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//td[contains(text(), '"+ VIN +"')]")));
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//td[contains(text(), '" + brand + "')]")));
            Assert.AreEqual(true, helper.isElementPresent(By.XPath("//td[contains(text(), '" + model + "')]")));
        }

        [TestMethod]
        public void testDeleteCar()
        {
            string carId = "";
            string VIN = "";
            string brand = "";
            string model = "";
            helper.loginIntoApplication(login, password);
            if (!helper.isElementPresent(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//form")))
            {
                carId = helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[1]"));
                VIN = helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[2]"));
                brand = helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[3]"));
                model = helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[4]"));
            } else
            {                
                VIN = helper.randomStringInteger(10);
                brand = helper.RandomString(8);
                model = helper.RandomString(8);
                helper.addCar(VIN, brand, model);
                carId = helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[1]"));
            }
            helper.deleteCar(carId);
            if (!helper.isElementPresent(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//form")))
            {
                Assert.AreNotEqual(carId, helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[1]")));
                Assert.AreNotEqual(VIN, helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[2]")));
                Assert.AreNotEqual(brand, helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[3]")));
                Assert.AreNotEqual(model, helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[4]")));
            }
        }

        [TestMethod]
        public void testDeleteCarDetail()
        {
            Random rnd = new Random();
            string carId = "";
            var carSpeed = rnd.Next(0, 130).ToString();
            var carRpm = (rnd.Next(6, 60) * 100).ToString();
            var carEngineLoad = rnd.Next(0, 100).ToString();
            var Voltage = "12." + helper.randomStringIntegerWithoutNull(2);
            var Lon = "13." + helper.randomStringIntegerWithoutNull(4);
            var Lat = "78." + helper.randomStringIntegerWithoutNull(4);
            helper.loginIntoApplication(login, password);
            if (!helper.isElementPresent(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//form")))
            {
                carId = helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[1]"));
            }
            else
            {
                var VIN = helper.randomStringInteger(10);
                var brand = helper.RandomString(8);
                var model = helper.RandomString(8);
                helper.addCar(VIN, brand, model);
                carId = helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[1]"));
            }
            helper.deleteAllCarDetailsFromTable();
            Thread.Sleep(1000);
            helper.createDetailToGivenCarId(carId, carSpeed, carRpm, carEngineLoad, Voltage, Lon, Lat, url);
            helper.getDriver().Navigate().Refresh();
            Thread.Sleep(1000);
            Voltage = Voltage.Replace('.', ',');
            Lon = Lon.Replace('.', ',');
            Lat = Lat.Replace('.', ',');
            helper.deleteAllCarDetailsFromTable();
            Assert.AreEqual(false, helper.isElementPresent(By.XPath("//table[@id='DetailsTable']//td[contains(text(),'" + carSpeed + "')]")));
            Assert.AreEqual(false, helper.isElementPresent(By.XPath("//table[@id='DetailsTable']//td[contains(text(),'" + carRpm + "')]")));
            Assert.AreEqual(false, helper.isElementPresent(By.XPath("//table[@id='DetailsTable']//td[contains(text(),'" + Voltage + "')]")));
            Assert.AreEqual(false, helper.isElementPresent(By.XPath("//table[@id='DetailsTable']//td[contains(text(),'" + Lon + "')]")));
            Assert.AreEqual(false, helper.isElementPresent(By.XPath("//table[@id='DetailsTable']//td[contains(text(),'" + Lat + "')]")));
        }

        [TestMethod]
        public void testDeleteCarError()
        {
            Random rnd = new Random();
            string carId = "";
            string errorCode = helper.RandomString(4);
            var errorString = helper.RandomString(5);
            helper.loginIntoApplication(login, password);
            if (!helper.isElementPresent(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//form")))
            {
                carId = helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[1]"));
            }
            else
            {
                var VIN = helper.randomStringInteger(10);
                var brand = helper.RandomString(8);
                var model = helper.RandomString(8);
                helper.addCar(VIN, brand, model);
                carId = helper.getElementText(By.XPath("//table[@id='CarsTable']//tbody//tr[1]//td[1]"));
            }
            helper.deleteAllErrorsFromTable();
            Thread.Sleep(1000);
            helper.createErrorToCarId(carId, errorCode, errorString, url);
            Assert.AreEqual(false, helper.isElementPresent(By.XPath("//table[@id='ErrorsTable']//td[contains(text(),'" + carId + "')]")));
            Assert.AreEqual(false, helper.isElementPresent(By.XPath("//table[@id='ErrorsTable']//td[contains(text(),'" + errorCode + "')]")));
            Assert.AreEqual(false, helper.isElementPresent(By.XPath("//table[@id='ErrorsTable']//td[contains(text(),'" + errorString + "')]")));
        }

        [TestMethod]
        public void testSymulator()
        {
            helper.loginIntoApplication(login, password);
            helper.openSymulator();
            var currentDate = DateTime.Now;
            Assert.AreEqual("Symulator", helper.getElementText(By.XPath("//div[@class='card-title']")));
            helper.turnOnSymulator();
            Thread.Sleep(5000);
            helper.turnOffSymulator();
            var firstDateFromTable = helper.getElementText(By.XPath("//*[@id='SymulatorDetailsTable']/tbody/tr[1]/td[8]"));
            DateTime date = DateTime.ParseExact(firstDateFromTable, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            Assert.AreEqual(true, helper.compareDates(currentDate, date));
            Assert.AreNotEqual("", helper.getElementText(By.XPath("//*[@id='SymulatorDetailsTable']/tbody/tr[1]/td[1]")));
            Assert.AreNotEqual("", helper.getElementText(By.XPath("//*[@id='SymulatorDetailsTable']/tbody/tr[1]/td[2]")));
            Assert.AreNotEqual("", helper.getElementText(By.XPath("//*[@id='SymulatorDetailsTable']/tbody/tr[1]/td[3]")));
            Assert.AreNotEqual("", helper.getElementText(By.XPath("//*[@id='SymulatorDetailsTable']/tbody/tr[1]/td[4]")));
            Assert.AreNotEqual("", helper.getElementText(By.XPath("//*[@id='SymulatorDetailsTable']/tbody/tr[1]/td[5]")));
            Assert.AreNotEqual("", helper.getElementText(By.XPath("//*[@id='SymulatorDetailsTable']/tbody/tr[1]/td[6]")));
            Assert.AreNotEqual("", helper.getElementText(By.XPath("//*[@id='SymulatorDetailsTable']/tbody/tr[1]/td[7]")));
        }

        [TestCleanup]
        public void TearDown()
        {
            helper.getDriver().Quit();
            helper.getDriver().Dispose();
        }
    }

    public class Helpers
    {     
        public IWebDriver driver;
        private static Random random = new Random();

        public Helpers ()
        {
            string path = Directory.GetCurrentDirectory();
            path = path.Remove(path.Length - 9) + "ChromeDriver";
            this.driver = new ChromeDriver(path);
            this.driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            this.driver.Manage().Window.Maximize();
        }

        //Selenium driver
        public IWebDriver getDriver()
        {
            return driver;
        }

        //Selenium login/logout methods
        public void loginIntoApplication(string login, string password)
        {
            getDriver().FindElement(By.Id("Username")).SendKeys(login);
            getDriver().FindElement(By.Id("Password")).SendKeys(password);
            getDriver().FindElement(By.XPath("//input[@value='Login' and @type='submit']")).Click();
        }

        public void logOutFromTheApplication()
        {
            getDriver().FindElement(By.XPath("//a[contains(text(), 'admin')]")).Click();
            getDriver().FindElement(By.XPath("//a[contains(text(), 'Wyloguj')]")).Click();
        }

        //Admin panel methods
        public void addCar(string VIN, string brand, string model)
        {
            findElementAndSendText(By.Id("VIN"), VIN);
            findElementAndSendText(By.Id("Brand"), brand);
            findElementAndSendText(By.Id("Model"), model);
            findElementAndClick(By.XPath("//input[@type='submit' and @value='Add']"));
        }

        public void deleteCar(string carId)
        {
            findElementAndClick(By.Id("DeleteCar_" + carId));
        }

        public void deleteAllCarDetailsFromTable()
        {
            while (isElementPresent(By.XPath("//*[@id='DetailsTable']//tbody//tr[1]//td[9]//button")))
            {
                findElementAndClick(By.XPath("//*[@id='DetailsTable']//tbody//tr[1]//td[9]//button"));
            }
        }

        public void deleteAllErrorsFromTable()
        {
            while (isElementPresent(By.XPath("//*[@id='ErrorsTable']//tbody//tr[1]//td[6]//button")))
            {
                findElementAndClick(By.XPath("//*[@id='ErrorsTable']//tbody//tr[1]//td[6]//button"));
            }
        }

        //Symulator helpers
        public void openSymulator()
        {
            findElementAndClick(By.LinkText("Symulator"));
        }

        public void turnOnSymulator()
        {
            findElementAndClick(By.LinkText("TURN ON"));
        }

        public void turnOffSymulator()
        {
            findElementAndClick(By.LinkText("TURN OFF"));
        }

        //Selenium helpers
        public bool isElementPresent(By location)
        {
            try
            {
                IWebElement element = getDriver().FindElement(location);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public string getElementText(By location)
        {
            try
            {
                IWebElement element = getDriver().FindElement(location);
                return element.Text;
            }
            catch (NoSuchElementException)
            {
                return "element is not found";
            }
        }

        public void findElementAndSendText(By location, string text)
        {
            try
            {
                IWebElement element = getDriver().FindElement(location);
                element.SendKeys(text);
            }
            catch (NoSuchElementException)
            {
                Trace.TraceError("No such element could be found");
            }
        }

        public void findElementAndClick(By location)
        {
            try
            {
                IWebElement element = getDriver().FindElement(location);
                element.Click();
            }
            catch (NoSuchElementException)
            {
                Trace.TraceError("No such element could be found");
            }
        }

        //API Methods
        public void createDetailToGivenCarId(string carId, string carSpeed, string carRpm, string carEngineLoad, string Voltage, string Lon, string Lat, string serverUrl)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(serverUrl + "/api/details/post");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"CarId\":\"" + carId + "\"," +
                              "\"Speed\":\"" + carSpeed + "\"," +
                              "\"Rpm\":\"" + carRpm + "\"," +
                              "\"EngineLoad\":\"" + carEngineLoad + "\"," +
                              "\"Voltage\":\"" + Voltage + "\"," +
                              "\"Lon\":\"" + Lon + "\"," +
                              "\"Lat\":\"" + Lat + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }

        public void createErrorToCarId(string carId, string errorCode, string errorString, string serverUrl)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(serverUrl + "/api/errors/post");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"CarId\":\"" + carId + "\"," +
                              "\"ErrorCode\":\"" + errorCode + "\"," +
                              "\"ErrorString\":\"" + errorString + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }

        //Just helpers
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string randomStringInteger(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string randomStringIntegerWithoutNull(int length)
        {
            const string chars = "123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool compareDates(DateTime currentDate, DateTime dateToCmp)
        {
            int result = DateTime.Compare(currentDate, dateToCmp);
            if(result < 0)
            {
                return true;
            } else if (result ==0)
            {
                return false;
            } else
            {
                return false;
            }
        }
    }
}
