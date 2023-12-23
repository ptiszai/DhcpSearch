using System;
using System.Threading;

namespace DhcpSearch
{
    internal class Program
    {
       // static string mac = "FE-6D-60-37-2A-A9";
        //static string mac = "94-DE-80-74-F7-90";
        static string mac = "B8-66-85-FF-23-5A";
        static string ip = "";
        static Thread dhcpThr;
        static PingThread pingThreadObject;
        static Thread pingThread;
        static DhcpSearch ipdhcp;
        static bool running = true;        

        static void Main(string[] args)
        {
            Console.WriteLine("Exit ESC key.");
            ipdhcp = new DhcpSearch();
            dhcpThr = new Thread(dhcp_executor);
            dhcpThr.Start();
            pingThreadObject = new PingThread(ip);
            pingThread = new Thread(new ThreadStart(pingThreadObject.Procedure));
            //  pingThread.Start();
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;

            Exit();
            System.Environment.Exit(0);
        }

        static void Exit()
        {           
            try
            {
                if (pingThread != null)
                {
                    pingThreadObject.StopThread = true;
                    if (pingThread.IsAlive)
                    {
                        if (pingThread.Join(2000))
                            Console.WriteLine("pingThread:Thread has termminated.");
                        else
                            Console.WriteLine("pingThread:The timeout has elapsed and Thread1 will resume.");
                    }
                }
                if ((dhcpThr != null) && dhcpThr.IsAlive)
                {
                    running = false;
                    if (dhcpThr.Join(2000))
                        Console.WriteLine("dhcpThr:Thread has termminated.");
                    else
                        Console.WriteLine("dhcpThr:The timeout has elapsed and Thread1 will resume.");
                }
            }
            catch (ThreadAbortException exp)
            {
                Console.WriteLine("Exit error");
            }
        }

        static void dhcp_executor()
        {
            Console.WriteLine($"Start DHCP executor.");
            try
            {
                string _ip_address = "0";
                while (running)
                {
                    _ip_address = ipdhcp.getIpByMac(mac);
                    if (_ip_address != "")
                    {
                        if (!_ip_address.Equals(ip))
                        {
                            ip = _ip_address;
                            Console.WriteLine($"MacAddress: {mac}");
                            Console.WriteLine($"IpAddress: {ip}");
                            pingThreadObject.SetIp(ip);
                            if (!pingThread.IsAlive)
                            {
                                pingThread.Start();
                            }
                        }
                    }
                    Thread.Sleep(3000);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine($"ERROR {StatusEnum.DHCP_EXECUTOR} , {exp.Message}");               
                return;
            }
        }
    }    
}
