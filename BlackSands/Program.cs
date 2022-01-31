using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Leaf.xNet;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxImage = System.Windows.Forms.MessageBoxIcon;

namespace AuthGG
{
    class Program
    {
        static void Main(string[] args)
        {
            //Update this with your program information found in dashboard
            //APPNAME = Name of your application
            //AIDHERE = AID found in your settings page > Upper right corner > Settings
            //APPSECRET = Secret in applications table
            //1.0 = indicates your application version located in your application settings
            //YOUTUBE TUTORIAL | https://www.youtube.com/watch?v=VjPz21Va9wU

            //This connects your file to the Auth.GG API, and sends back your application settings and such
            OnProgramStart.Initialize("BlackSands", "139881", "4Pn9IqRGQaPrw2uqKPalvnXrmefwiEC87rt", "1.0");
            if (ApplicationSettings.Freemode)
            {
                //Usually when your application doesn't need a login and has freemode enabled you put the code here you want to do
                MessageBox.Show("Freemode is active, bypassing login!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (!ApplicationSettings.Status)
            {
                //If application is disabled in your web-panel settings this action will occur
                MessageBox.Show("Application is disabled!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill(); // closes the application
            }
            PrintLogo();
            Console.WriteLine("[1] Register");
            Console.WriteLine("[2] Login");
            Console.WriteLine("[3] All in one");
            Console.WriteLine("[4] Extend Subscription");
            Console.Write("[>] ");
            string option = Console.ReadLine();
            if (option == "1")
            {
                if (!ApplicationSettings.Register)
                {
                    //Register is disabled in application settings, will show a messagebox that it is not enabled
                    MessageBox.Show("Register is not enabled, please try again later!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    Process.GetCurrentProcess().Kill(); //closes the application
                }
                else
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine();
                    Console.WriteLine("Username:");
                    string username = Console.ReadLine();
                    Console.WriteLine("Password:");
                    string password = Console.ReadLine();
                    Console.WriteLine("Email:");
                    string email = Console.ReadLine();
                    Console.WriteLine("License:");
                    string license = Console.ReadLine();
                    if (API.Register(username, password, email, license))
                    {
                        MessageBox.Show("You have successfully registered!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                        // Do code of what you want after successful register here!
                    }
                }
            }
            else if (option == "2")
            {
                if (!ApplicationSettings.Login)
                {
                    //Register is disabled in application settings, will show a messagebox that it is not enabled
                    MessageBox.Show("Login is not enabled, please try again later!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    Process.GetCurrentProcess().Kill(); //closes the application
                }
                else
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine();
                    Console.WriteLine("Username:");
                    string username = Console.ReadLine();
                    Console.WriteLine("Password:");
                    string password = Console.ReadLine();
                    if (API.Login(username, password))
                    {
                        MessageBox.Show("You have successfully logged in!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                        Console.Clear();
                        PrintLogo();
                        // Success login stuff goes here
                        Console.ForegroundColor = ConsoleColor.White;
                        API.Log(username, "Logged in!"); //Logs this action to your web-panel, you can do this anywhere and for anything!
                        App();
                    }
                }
            }
            else if (option == "4")
            {
                Console.Clear();
                PrintLogo();
                Console.WriteLine();
                Console.WriteLine("Username:");
                string username = Console.ReadLine();
                Console.WriteLine("Password:");
                string password = Console.ReadLine();
                Console.WriteLine("Token:");
                string token = Console.ReadLine();
                if (API.ExtendSubscription(username, password, token))
                {
                    MessageBox.Show("You have successfully extended your subscription!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                    // Do code of what you want after successful extend here!
                }
            }
            else if (option == "3")
            {
                Console.Clear();
                PrintLogo();
                Console.WriteLine();
                Console.WriteLine("AIO Key:");
                string KEY = Console.ReadLine();
                if (API.AIO(KEY))
                {
                    //Code you want to do here on successful login
                    MessageBox.Show("Welcome back to my application!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                    Process.GetCurrentProcess().Kill(); // closes the application
                }
                else
                {
                    //Code you want to do here on failed login
                    MessageBox.Show("Your key does not exist!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    Process.GetCurrentProcess().Kill(); // closes the application
                }
            }
            Console.Read();
        }
        public static void App()
        {

            Console.Clear();
            PrintLogo();
            Console.Write("[?] Phone number : ");
            string phone = Console.ReadLine();
            Console.Write("[?] Password : ");
            string password = Console.ReadLine();
            password = MD5Encryption("hFEkFMMf2YpPD6T2R9h76wW7e27w55Yi"+password);
            string data = SandsLogin(phone, password);
            if (data == "false")
            {

            }
            else
            {

            }


        }
        public static string SandsLogin(string phone ,string password)
        {
            using (HttpRequest req = new HttpRequest())
            {
                req.IgnoreProtocolErrors = true;
                var param = new RequestParams();
                param["username"] = phone;
                param["password"] = password;
                param["prefix"] = "+20";
                param["os"] = "other";
                var res = req.Post("https://whitesands.top/api/buyer/login",param);
                Console.WriteLine(res);
                JObject data = JObject.Parse(res.ToString());
                string h = data["code"].ToString();
                if (h == "-1")
                {

                }
                else
                {
                    return data["data"].ToString();
                }
                
            }

             return "false";
        }
        public static string getUser(string access_token)
        {
            using (HttpRequest req = new HttpRequest())
            {
                HttpRequest req  = addAuthorization(req,access_token);
                req.Get("https://whitesands.top/api/buyer");

            }

            return "false";
        }
        public static HttpRequest addAuthorization(HttpRequest req,string token)
        {
            req.AddHeader("authorization", "Bearer " + token);
            return req;

        }
        public static string MD5Encryption(string encryptionText)
        {

            // We have created an instance of the MD5CryptoServiceProvider class.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //We converted the data as a parameter to a byte array.
            byte[] array = Encoding.UTF8.GetBytes(encryptionText);
            //We have calculated the hash of the array.
            array = md5.ComputeHash(array);
            //We created a StringBuilder object to store hashed data.
            StringBuilder sb = new StringBuilder();
            //We have converted each byte from string into string type.

            foreach (byte ba in array)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            //We returned the hexadecimal string.
            return sb.ToString();
        }

        public static void PrintLogo()
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine();
                Console.WriteLine("                        (                        ");
                Console.WriteLine("   (  (             )   )\\ )             (       ");
                Console.WriteLine(" ( )\\ )\\   )     ( /(  (()/(    )        )\\ )    ");
                Console.WriteLine(" )((_|(_| /(  (  )\\())  /(_))( /(  (    (()/((   ");
                Console.WriteLine("((_)_ _ )(_)) )\\((_)\\  (_))  )(_)) )\\ )  ((_))\\  ");
                Console.WriteLine(" | _ ) ((_)_ ((_) |(_) / __|((_)_ _(_/(  _| ((_) ");
                Console.WriteLine(" | _ \\ / _` / _|| / /  \\__ \\/ _` | ' \\)) _` (_-< ");
                Console.WriteLine(" |___/_\\__,_\\__||_\\_\\  |___/\\__,_|_||_|\\__,_/__/ ");
                Console.WriteLine("");

                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
