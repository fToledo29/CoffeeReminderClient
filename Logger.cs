using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMatesClient
{
    class Logger
    {
        private string dinamicName = "";

        /// <summary>
        /// Writes a text into a log, it will create a new log after each hour.
        /// </summary>
        /// <param name="text">The text that will be set into the log file.</param>
        public void WirteLog(string text)
        {
            try
            {
             
                // Create a new dinamic name
                BuildDinamicName();
                Program prog = new Program();
                // Set the necesary values.
                string currentMatePathFile = prog.currentLocation + @"\" + dinamicName;
                string dateTime = DateTime.Now.ToString("dd/mm/yyyy H:mm:ss");
                text = "[" + dateTime + "]" + " - " + text;

                // validate if there is a log file
                if (File.Exists(currentMatePathFile))
                {
                    AppendLog(text);
                }
                else
                {
                    CreateNewLog(text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        /// <summary>
        /// If there is no log with the given name, a new one will be created
        /// </summary>
        /// <param name="text">The text that will be set into the log</param>
        private void CreateNewLog(string text)
        {
            try
            {
                BuildDinamicName();
                Program prog = new Program();

                string currentMatePathFile = prog.currentLocation + @"\" + dinamicName;

                System.IO.File.WriteAllText(currentMatePathFile, text);
                Console.WriteLine(currentMatePathFile);                
                Process.Start("notepad.exe", currentMatePathFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        /// <summary>
        /// If there is a log with the given name, the text will be appened here
        /// </summary>
        /// <param name="text">The text that will be set into the log</param>
        private void AppendLog(string text)
        {
            try
            {
                // Create a new dinamic name
                BuildDinamicName();
                Program prog = new Program();

                string currentMatePathFile = prog.currentLocation + @"\" + dinamicName;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentMatePathFile, true))
                {
                    file.WriteLine(text);
                }
                Console.WriteLine(currentMatePathFile);
                Process.Start("notepad.exe", currentMatePathFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Creates a new random name, based on the hour, day, month, and year, so each hour the name will be diferent
        /// </summary>
        private void BuildDinamicName()
        {            
            try
            {
                DateTime currentDate = DateTime.Now;
                string hour = currentDate.Hour.ToString();
                string day = currentDate.Day.ToString();
                string month = currentDate.Month.ToString();
                string year = currentDate.Year.ToString();

                dinamicName = "CoffeMatesLog-" + hour + day + month + year + ".txt";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Console.ReadKey();
            }
        }
    }
}