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
    /// <summary>
    /// ModemController web service class for interfacing with hardware AT modem and sending of SMS.
    /// </summary>
    public class ModemController : ApiController
    {
        /// <summary>
        /// Serial port that AT modem is connected to.
        /// </summary>
        private static string COM_PORT = "COM4";
        /// <summary>
        /// Baud rate of AT modem communications.
        /// </summary>
        private static int BAUD_RATE = 9600;

        /// <summary>
        /// Method to send an SMS message to a phone number.
        /// </summary>
        /// <param name="phoneNo">Pecipient's phone number</param>
        /// <param name="message">SMS message to send</param>
        /// <returns></returns>
        [HttpGet]
        public bool SendSms(string phoneNo, string message)
        {
            // Initialize and open serial port
            SerialPort serialPort = new SerialPort(COM_PORT, BAUD_RATE);
            serialPort.Open();

            // Send AT commands to modem to send SMS
            Thread.Sleep(200);
            serialPort.Write("AT+CMGF=1\r");
            Thread.Sleep(200);
            serialPort.Write("AT+CMGS=\"" + phoneNo + "\"\r\n");
            Thread.Sleep(200);
            serialPort.Write(message + "\x1A");

            // Close serial port and return success
            serialPort.Close();
            return true;
        }
    }
}
