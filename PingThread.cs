using System;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;

namespace DhcpSearch
{
    public class PingThread
    {
        #region public variables 
        public bool StopThread;
        #endregion

        #region private variables       
        private string ipAddress = "";
        private Ping pingSender;
        private int sleepTimeInMsec;
        private StatusEnum state;
        private int repeatedNumber;   
        private bool success = false;
        private bool failed = false;
        private DhcpSearch ipmac;
        #endregion

        #region public eventHandlers                
        #endregion

        #region public functions 
        //--------------------------------------------
        public PingThread(string ipAddress_a)
        {
            ipAddress = ipAddress_a.Trim();
            StopThread = false;
            sleepTimeInMsec = 1000;
            state = StatusEnum.PING_START;
            repeatedNumber = 0;
            ipmac = new DhcpSearch();
        }

        //--------------------------------------------
        public void SetIp(string ipAddress_a)
        {
            ipAddress = ipAddress_a;
            state = StatusEnum.PING_START;
        }

        //-------------------------------------------- 
        public void Procedure()
        {
            while (!StopThread)
            {
                switch (state)
                {
                    case StatusEnum.PING_START:
                        {
                            repeatedNumber = 0;
                            ping();
                            state = StatusEnum.PING_RUNNING;
                            break;
                        }
                    case StatusEnum.PING_RUNNING:
                        {
                      //      Console.WriteLine($"PING_RUNNING");
                            break;
                        }
                    case StatusEnum.PING_TIMEOUT_ERROR:
                        {
                            if (!failed)
                            {
                                Console.WriteLine($"Ping request timed out : {ipAddress} :");  
                            }
                            sleepTimeInMsec = 3000;                          
                            state = StatusEnum.PING_START;
                            success = false;
                            failed = true;
                            break;
                        }
                    case StatusEnum.PING_GENERAL_ERROR:
                        {
                            if (!failed)
                            {
                                Console.WriteLine($"Ping tunnel failer : {ipAddress} :");
                                success = false;
                            }
                            sleepTimeInMsec = 3000;                           
                            state = StatusEnum.PING_START;
                            failed = true;
                            success = false;
                            break;
                        }
                    case StatusEnum.PING_SUCCESS:
                        {                          
                            if (!success)
                            {
                                string _mac = ipmac.getMacByIp(ipAddress);
                                Console.WriteLine($"Ping success : {ipAddress} ;;; {_mac}");                                                                   
                            }                                             
                            sleepTimeInMsec = 3000;
                            state = StatusEnum.PING_START;
                            failed = false;
                            success = true;
                            break;
                        }
                    case StatusEnum.GENERAL_ERROR:
                        {
                            if (!failed)
                            {
                                Console.WriteLine($"Ping general failer : {ipAddress} :");
                            }
                            sleepTimeInMsec = 3000;
                            state = StatusEnum.PING_START;
                            failed = true;
                            success = false;
                            break;
                        }
                }                       
              
                Thread.Sleep(sleepTimeInMsec);
            }
        }
        #endregion

        #region private functions 

        //----------------------------------
        private void ping()
        {
            try
            {                
                pingSender = new Ping();
                AutoResetEvent waiter = new AutoResetEvent(false);
                PingOptions _options = new PingOptions(64, true);
                _options.DontFragment = true;
                pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);                
                int _timeout = 2000;                
                string _data = "0aaaaaaaaa1aaaaaaaaaXaaaaa5aaaab"; // 32 bytes           
                byte[] _buffer = Encoding.ASCII.GetBytes(_data);                
                pingSender.SendAsync(ipAddress, _timeout, _buffer, _options);
            }
            catch (Exception msg)
            {
                 Console.WriteLine($"ERROR:Ping start: {msg}");
            }
        }
     
        #endregion

        #region private events functions
        //---------------------------
        // Ping event
        //---------------------------
        private void PingCompletedCallback(object sender, PingCompletedEventArgs eArgs)
        {
            if (eArgs.Cancelled)
            {
                ping();
                return;
            }          

            if (eArgs.Error != null)
            {
                if (repeatedNumber >= 5)
                {
                    repeatedNumber = 0;
                    state = StatusEnum.PING_GENERAL_ERROR;
                }
                else
                {
                    repeatedNumber++;
                    ping();
                }
                return;
            }

            if (eArgs.Reply.Status == IPStatus.Success)
            {
                repeatedNumber = 0;
                state = StatusEnum.PING_SUCCESS;
                return;
            }

            if (eArgs.Reply.Status == IPStatus.TimedOut)
            {
                if (repeatedNumber >= 5)
                {
                    repeatedNumber = 0;
                    state = StatusEnum.PING_TIMEOUT_ERROR;
                }
                else
                {
                    repeatedNumber++;
                    ping();
                }
                return;
            }
            if (repeatedNumber >= 5)
            {
                repeatedNumber = 0;
                state = StatusEnum.GENERAL_ERROR;
            }
            else
            {
                repeatedNumber++;
                ping();
            }
        }
        #endregion
    }
}
