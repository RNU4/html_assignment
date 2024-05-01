// See https://aka.ms/new-console-template for more information
namespace SimpleHttpServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;


    public static class Program
    {
        public static void Main(string[] args)
        {
            IHttpServer server = new HttpServer(80);
            server.Start();
        }
    }


    public interface IHttpServer
    {
        void Start();
    }


    public class HttpServer : IHttpServer
    {
        private readonly TcpListener listener;


        public HttpServer(int port)
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
        }

static string file_to_string(string path)
    {
        if (!File.Exists(path))
        {
            return "404";
        }
        return File.ReadAllText(path);
    }
    struct html_responese{
        public string html = "";
        public string type = "";
        public string url = "";
        public Dictionary<string, string> parameter_map;

        public html_responese() {
        html = "";
        type = "";
        url = "";
        parameter_map = new Dictionary<string, string>();
    }
    }
    

    public static string gen_password(int length, bool uppercase, bool numbers, bool symbols){   
        Random random = new Random();

        string password = "";
        string chars = "abcdefghijklmnopqrstuvwxyz";
        if (uppercase)
            chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (numbers)
            chars += "0123456789";
        if (symbols)
            chars += "!@#$%^&*()_-+=<>?";

        for (int i = 0; i < length; i++)
        {
            password += chars[random.Next(chars.Length)];
        }

        return password;
    }


        static html_responese html_response_parser(string[] responese){
        // GET /endpoint?param1=value1&param2=value2 HTTP/1.1

        html_responese parsed = new html_responese{};
        string[] split =  responese[0].Split(' ');
        if (split.Length > 0){
            parsed.type = split[0];
        }else {
            //error
        }
        if (split.Length > 1){
        split =  split[1].Split(' ','?');
        if (split.Length > 0){
            parsed.url = split[0];
        }else {
            //error
        }}


        if (split.Length > 1){
        split =  split[1].Split('&');
        if (split.Length > 1){
            for (int i = 0; i < split.Length; i++){
                string[] param = split[i].Split('=');
                if (param.Length > 1){
                    parsed.parameter_map[param[0]] = param[1];
                }
        }}}
        return parsed;
        }
        public void Start()
        {
            int i = 0;
            listener.Start();
            while (true)
            {
                Console.WriteLine("Waiting for client {0}", i++);
                var client = listener.AcceptTcpClient();
                

                var buff = new byte[10240];
                var stream = client.GetStream();
                

                var length = stream.Read(buff, 0, buff.Length);
                var buffstr = Encoding.UTF8.GetString(buff, 0, length);
                Console.WriteLine(buffstr);
                string[] split_buff = buffstr.Split('\n').ToArray();
                html_responese parsed = html_response_parser(split_buff);
                Console.WriteLine("type " + parsed.type);
                Console.WriteLine("url " + parsed.url);
                foreach (var entry in parsed.parameter_map)
                {
                    Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
                }


                var http_body = "404";
                if (parsed.type == "GET" && parsed.url == "/"){
                    http_body = file_to_string("./page.html");
                }else if (parsed.type == "GET" && parsed.url == "/generator"){
                    http_body = file_to_string("./generator.html");

                }else if (parsed.type == "GET" && parsed.url == "/result"){
                    var len = 0;
                    int.TryParse(parsed.parameter_map["length"], out len);

                    var password = gen_password(len,
                                    parsed.parameter_map["uppercase"] == "on",
                                    parsed.parameter_map["numbers"] == "on",
                                    parsed.parameter_map["symbols"] == "on");
                    http_body = $"<p>{password}</p>";
                } else {
                    Console.WriteLine("Request ignored");
                }
                        

                var httpResonse = "HTTP/1.0 200 OK" + "\r\n"
                                + "Content-Length: " + http_body.Length + "\r\n"
                                + "Content-Type: " + "text/html" + "\r\n"
                                + "\r\n"
                                + http_body
                                + "\r\n" + "\r\n";
                
                stream.Write(Encoding.UTF8.GetBytes(httpResonse));
                stream.Flush();
                stream.Close();
                client.Close();
                Thread.Sleep(100);
            }
        }
    }
}