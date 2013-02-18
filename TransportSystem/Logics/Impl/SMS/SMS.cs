using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using TransportSystem.Domain;
using TransportSystem.Logics.Interfaces.SMS;
using TransportSystem.Logics.Interfaces.Trips;

namespace TransportSystem.Logics.Impl.SMS
{
    public class SMS : ISMS
    {
        public void Dispose()
        {
        }

        public void SendMessage(string number, string message)
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest
                .Create(string.Format("http://smspilot.ru/api.php?send={0}&to={1}&from={2}&apikey={3}", message, number, "Beride", "80972DUW993V86N9GRKQA9TG765OI66H7B375F49S3BQD18P46J1B1U8QSE0OASZ"));

            var myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
        }
    }
}
