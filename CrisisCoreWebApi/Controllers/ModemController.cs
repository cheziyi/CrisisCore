using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace CrisisCoreWebApi.Controllers
{
    public class ModemController : ApiController
    {
        private static string COM_PORT = "COM4";
        private static int BAUD_RATE = 9600;

        [HttpGet]
        public bool SendSms(string phoneNo, string message)
        {
            SerialPort serialPort = new SerialPort(COM_PORT, BAUD_RATE);

            serialPort.Open();

            Thread.Sleep(200);
            serialPort.Write("AT+CMGF=1\r");

            Thread.Sleep(200);
            serialPort.Write("AT+CMGS=\"" + phoneNo + "\"\r\n");

            Thread.Sleep(200);
            serialPort.Write(message + "\x1A");

            serialPort.Close();

            return true;
        }
    }
}
