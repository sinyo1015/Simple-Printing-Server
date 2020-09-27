// Filename:  HttpServer.cs        
// Author:    Benjamin N. Summerton <define-private-public>        
// License:   Unlicense (http://unlicense.org/)

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace PrinterServer
{
    class JsonConfig
    {
        public string NamaToko { get; set; }
        public string AlamatToko { get; set; }
        public string DefaultPrinter { get; set; }
    }


    class Response
    {
        public bool Status { get; set; }
        public string Data { get; set; }
    }


    class Program
    {
        private static HttpListener listener;
        private static string url = "http://localhost:7001/";
        
        private static byte[] configJson;

        private static JsonConfig jsonConfig;

        const int Hide = 0;
        const int Show = 1;

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);


        public static async Task HandleIncomingConnections()
        {

            

            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                if(req.HttpMethod == "POST")
                {
                    Stream body = req.InputStream;
                    StreamReader streamReader = new StreamReader(body, req.ContentEncoding);

                    string data2 = streamReader.ReadToEnd();

                    var baru = JObject.Parse(data2);

                    var mentah = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(data2);

                    string namaPembeli = baru.GetValue("NamaPembeli").ToString();
                    string tanggalTransaksi = baru.GetValue("TanggalTransaksi").ToString();
                    string kodeBelanja = baru.GetValue("KodeBelanja").ToString();
                    string totalBelanjaan = baru.GetValue("TotalBelanjaan").ToString();
                    string bayarBelanjaan = baru.GetValue("BayarBelanjaan").ToString();
                    string kembalianBelanjaan = baru.GetValue("KembalianBelanjaan").ToString();

                    List<BarangBelanjaan> barangBelanjaans = new List<BarangBelanjaan>();



                    foreach (JObject jObject in baru.GetValue("Belanjaan").ToObject<JArray>())
                    {
                        barangBelanjaans.Add(jObject.ToObject<BarangBelanjaan>());
                    }
                    

                    RawPrinterHelper rawPrinterHelper = new RawPrinterHelper()
                                                            .CetakHeader(jsonConfig.NamaToko, jsonConfig.AlamatToko)
                                                            .CetakTanggal(tanggalTransaksi)
                                                            .CetakNama(namaPembeli)
                                                            .CetakKodeBelanja(kodeBelanja)
                                                            .CetakBelanjaan(barangBelanjaans)
                                                            .CetakUang(totalBelanjaan, bayarBelanjaan, kembalianBelanjaan);
                    rawPrinterHelper.PrintDirect(jsonConfig.DefaultPrinter);

                    body.Close();


                    Response response = new Response() { Status = true, Data = "Success" };
                    


                    // Write the response info
                    string disableSubmit = !runServer ? "disabled" : "";
                    byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                    resp.ContentType = "application/json";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;

                    // Write out to the response stream (asynchronously), then close it
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

            }
        }


        public static void Main(string[] args)
        {
            IntPtr hWndConsole = GetConsoleWindow();
            ShowWindow(hWndConsole, Hide);


            //Read Configs
            try
            {
                configJson = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "config.json"));
                string readJson = Encoding.UTF8.GetString(configJson, 0, configJson.Length);
                jsonConfig = JsonConvert.DeserializeObject<JsonConfig>(readJson);
            }
            catch (FileNotFoundException e)
            {
                MessageBox((IntPtr)0, "File config.json tidak ditemukan, program keluar", "config.json missing", 0);
                Environment.Exit(-1);
            }
            catch(JsonException e)
            {
                MessageBox((IntPtr)0, "Format config.json korup atau format salah, program keluar", "malformed config.json", 0);
                Environment.Exit(-1);
            }


            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}