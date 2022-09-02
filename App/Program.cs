using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();    
        }
        //host builder
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // create appsettings.json if not exist
                    const string File_name = "appsettings.json";
                    if (!File.Exists(File_name) || new FileInfo(File_name).Length == 0) {
                        using (StreamWriter writer = new StreamWriter(File_name))
                        {
                            writer.Write("{\r\n  \"ConnectionStrings\": {\r\n    \"DefaultConnection\": \"Server=(localdb)\\\\mssqllocaldb;Database=CargoTruckDatabase");
                            writer.Write(GetRandomString());
                            writer.Write("; Trusted_Connection = True; MultipleActiveResultSets = true\",\r\n  },\r\n  \"Logging\": {\r\n    \"LogLevel\": {\r\n      \"Default\": \"Information\",\r\n      \"Microsoft\": \"Warning\",\r\n      \"Microsoft.Hosting.Lifetime\": \"Information\"\r\n    }\r\n  },\r\n  \"AllowedHosts\": \"*\"\r\n}");
                        } 
                    }
                    webBuilder.UseStartup<Startup>();
                });

        //random string 
        private static string GetRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
    }
}
