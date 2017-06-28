using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;


namespace CoffeeMatesClient
{
    
    class Program
    {
        //public string currentLocation = AppDomain.CurrentDomain.BaseDirectory + @"MatesRollDocs\"; //System.Reflection.Assembly.GetExecutingAssembly().Location; //uncoment!
        public string currentLocation = @"C:\Users\jtoledo\documents\visual studio 2013\Projects\CoffeeMatesClient\CoffeeMatesClient\MatesRollDocs\";
        static Helper HelpMeTo = new Helper();

        static void Main(string[] args)
        {
            Program context = new Program();
            HttpClient cons = SetProperties();
            Task<List<Mate>> jsonObj = APIAsyncCall(cons);
            List<Mate> obj = jsonObj.Result;
            int LastMateID = Convert.ToInt32(context.GetLastMate()[0][0]);
            UpdateCurrentMate(obj, LastMateID);
        }

        /// <summary>
        /// Saves the current mate into an XML file
        /// </summary>
        /// <param name="newMate">Name of the new mate</param>
        /// <param name="Id">Id of the new mate</param>
        public void SaveMate(string newMate, string Id)
        {
            try 
            {

                string currentMatePathFile = currentLocation + @"\CurrentMate.xml";
                string currentMate = ResourceTest.CurrentMate;

                XmlDocument matesDocument = new XmlDocument();
                matesDocument.LoadXml(ResourceTest.CurrentMate);
                XmlNode idNode = matesDocument.DocumentElement.SelectSingleNode("/CoffeeMate/Id");
                XmlNode nameMateNode = matesDocument.DocumentElement.SelectSingleNode("/CoffeeMate/Mate");
                idNode.InnerText = Id;
                nameMateNode.InnerText = newMate;
                matesDocument.Save(currentMatePathFile);
                Console.WriteLine(currentMatePathFile);
                Process.Start("notepad.exe", currentMatePathFile);
            }
            catch (Exception ex)
            {
                HelpMeTo.LogAnException(ex.Message.ToString());
            }
        }
                
        /// <summary>
        /// Gets the last mate set into an XML
        /// </summary>
        /// <returns>List<string[]></returns>
        private List<string[]> GetLastMate()
        {
            List<string[]> lastMate = new List<string[]>();
            try
            {
                
                string currentMate = ResourceTest.CurrentMate;
                
                XmlDocument matesDocument = new XmlDocument();
                matesDocument.LoadXml(ResourceTest.CurrentMate);
                XmlNode idNode = matesDocument.DocumentElement.SelectSingleNode("/CoffeeMate/Id");
                XmlNode nameMateNode = matesDocument.DocumentElement.SelectSingleNode("/CoffeeMate/Mate");
                string idMate = HelpMeTo.CleanString(idNode.InnerText);
                string nameMate = HelpMeTo.CleanString(nameMateNode.InnerText);
                
                lastMate.Add(new string[2] { idMate, nameMate });
            }
            catch (Exception ex)
            {
               HelpMeTo.LogAnException(ex.Message.ToString());
            }
            return lastMate;
        }
         
        /// <summary>
        /// Process the given mate information
        /// </summary>
        /// <param name="newMate">New mate info</param>
        /// <param name="LastMateName">The name of the last mate</param>
        private static void ProcessNewMateInfo(IEnumerable<Mate> newMate)
        {
            try
            {
                Program context = new Program();
                string newtMate = newMate.Select(x => x.Name).FirstOrDefault().ToString();
                string newMateId = newMate.Select(x => x.Id).FirstOrDefault().ToString();
                // Saving the new mate information
                context.SaveMate(newtMate, newMateId);
            }
            catch (Exception ex)
            {
                HelpMeTo.LogAnException(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Sets the new mate.
        /// </summary>
        /// <param name="Mates">List of mates</param>
        /// <param name="LastMateID">Id of the last mate</param>
        public static void UpdateCurrentMate(List<Mate> Mates, int LastMateID)
        {
            try
            {
                Program context = new Program();
                if (LastMateID >= Convert.ToInt32(Mates.Last().Id))
                {
                    IEnumerable<Mate> newMate = Mates.Where(x => x.Id == "0");
                    ProcessNewMateInfo(newMate);
                }
                else
                {
                    IEnumerable<Mate> newMate = Mates.Where(x => x.Id == Convert.ToString(LastMateID + 1));
                    ProcessNewMateInfo(newMate);
                }
                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                HelpMeTo.LogAnException(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Executes the call to the API which is going to give us all the mates.
        /// </summary>
        /// <param name="cons">Http client consumer</param>
        /// <returns>Task<List<Mate>></returns>
       static async Task<List<Mate>> APIAsyncCall(HttpClient cons)
        {
            List<Mate> jsonObj = null;
            try
            {
                using (cons)
                {
                    HttpResponseMessage res = await cons.GetAsync("CoffeeAPI/mates");
                    res.EnsureSuccessStatusCode();
                    if (res.IsSuccessStatusCode)
                    {
                        string response = await res.Content.ReadAsStringAsync();
                        jsonObj = await Task.Run(() => JsonConvert.DeserializeObject<List<Mate>>(response));    
                        
                        Console.WriteLine("Response: " + jsonObj);
                    }
                }
            }
            catch (Exception ex)
            {
                HelpMeTo.LogAnException(ex.Message.ToString());
            }
            return jsonObj;
        }

        /// <summary>
        /// Sets the HTTP client properties.
        /// </summary>
        /// <returns>HttpClient</returns>
        static HttpClient SetProperties()
        {
            HttpClient cons = new HttpClient();
            try
            {
                Program context = new Program();
                string URLApi = ResourceTest.URLApi;
                cons.BaseAddress = new Uri(URLApi);
                cons.DefaultRequestHeaders.Accept.Clear();
                cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));                
            }
            catch (Exception ex)
            {
                HelpMeTo.LogAnException(ex.Message.ToString());
            }
            return cons;
        }
    }
}
