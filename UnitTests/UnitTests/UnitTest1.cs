using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace UnitTests
{


    [TestClass]
    public class UnitTest1
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private static String serverUrl = "http://localhost:9946";
        private string randomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        private string parseJsonToString(String json)
        {   
            json = json.Replace("[", "");
            json = json.Replace("]", "");
            json = json.Replace(",", "");
            json = json.Replace("{", "");    
            return json;
        }

        public String[] parseCarListToStringArray(String carList)
        {
            var arrayCalList = carList.Split('}');
            return arrayCalList;
        }

        public String getAllCarsFromDatabase()
        {
            string json = string.Empty;
            string url = serverUrl + "/api/cars/getall";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            var responseString = parseJsonToString(json);
            return responseString;
        }

        [TestMethod]
        public void addCar()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(serverUrl + "/api/cars/post");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var carModel = randomString(7);
            var carBrand = randomString(7);
            var carVin = random.Next(1000000, 2000000);
            String carVinString = carVin.ToString();

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"Model\":\""+ carModel + "\"," +
                              "\"Brand\":\"" + carBrand + "\"," +
                              "\"VIN\":\"" + carVin.ToString() + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Assert.AreEqual("OK", httpResponse.StatusCode.ToString());
            
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            var allCars = getAllCarsFromDatabase();
            Assert.IsTrue(allCars.Contains(carModel));
            Assert.IsTrue(allCars.Contains(carBrand));
            Assert.IsTrue(allCars.Contains(carVinString));
        }

        [TestMethod]
        public void getAllCars()
        {
            string json = string.Empty;
            string url = serverUrl + "/api/cars/getall";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            Assert.AreEqual("OK", response.StatusCode.ToString());
            Assert.IsTrue(json.Contains("CarId"));
            Assert.IsTrue(json.Contains("VIN"));
            Assert.IsTrue(json.Contains("Brand"));
            Assert.IsTrue(json.Contains("Model"));
            Assert.IsTrue(json.Contains("CreateDate"));
        } 
        
        [TestMethod]
        public void deleteCarFromDatabase()
        {
            string jsom = string.Empty;
            string url = serverUrl + "/Home/DeleteCar/";

            var carListString = getAllCarsFromDatabase();
            carListString = parseJsonToString(carListString);
            string[] carArrayListString = parseCarListToStringArray(carListString);
            var firstCarToDelete = carArrayListString[0];
            int pFrom = firstCarToDelete.IndexOf("CarId\":") + "CarId\":".Length;
            int pTo = firstCarToDelete.LastIndexOf("\"VIN");
            String result = firstCarToDelete.Substring(pFrom, pTo - pFrom);
            url += result;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();

            carListString = getAllCarsFromDatabase();
            Assert.IsFalse(carListString.Contains("CarId\":" + result));
        }       
    }
}
