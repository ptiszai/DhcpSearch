using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DhcpSearch
{
    public struct MacIpPair
    {
        public string MacAddress;
        public string IpAddress;
    }
    public class DhcpSearch
    {
        //--------------------------------------------
        public string MakelowerCasetoUpper(string input_a)
        {
            string _output = "";
            foreach (char c in input_a)
            {
                _output += Char.IsUpper(c) ? Char.ToLower(c) : Char.ToUpper(c);
            }           
            return _output;
        }

        //--------------------------------------------
        public string getIpByMac(string mac_a)
        {
            var macIpPairs = GetAllMacAddressesAndIppairs();
            string _mac = MakelowerCasetoUpper(mac_a);            
            int index = macIpPairs.FindIndex(x => x.MacAddress == _mac);
            if ((index >= 0) && (index < macIpPairs.Count))
            {
                return macIpPairs[index].IpAddress.ToUpper();
            }
            else
            {
                return "";
            }
        }

        //--------------------------------------------
        public string getMacByIp(string ip_a)
        {
            var macIpPairs = GetAllMacAddressesAndIppairs();
            int index = macIpPairs.FindIndex(x => x.IpAddress == ip_a);
            if ((index >= 0) && (index < macIpPairs.Count))
            {
                return macIpPairs[index].MacAddress.ToUpper();
            }
            else
            {
                return "";
            }
        }

        //--------------------------------------------
        public List<MacIpPair> GetAllMacAddressesAndIppairs()
        {
            List<MacIpPair> mip = new List<MacIpPair>();
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string cmdOutput = pProcess.StandardOutput.ReadToEnd();
            string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

            foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
            {
                mip.Add(new MacIpPair()
                {
                    MacAddress = m.Groups["mac"].Value,
                    IpAddress = m.Groups["ip"].Value
                    //Console.WriteLine($"MacAddress: {MacAddress}");
                });
            }
            return mip;
        }

        //--------------------------------------------
        public void DeleteIpFromArpCashe(string ip_a)
        {
            if (ip_a == "")
            {
                return;
            }            
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = $"-d {ip_a}";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();            
            return;
        }
    }
}
