using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class Helper
    {
        public static string decodeEscapeChar(string _str)
        {
            if (_str.Contains(@"\E\"))
            {
                _str = _str.Replace(@"\E\", @"\");
            }

            if (_str.Contains(@"\F\"))
            {
                _str = _str.Replace(@"\F\", @"|");
            }

            if (_str.Contains(@"\R\"))
            {
                _str = _str.Replace(@"\R\", @"~");
            }

            if (_str.Contains(@"\S\"))
            {
                _str = _str.Replace(@"\S\", @"^");
            }

            if (_str.Contains(@"\T\"))
            {
                _str = _str.Replace(@"\T\", @"&");
            }

            return _str;
        }

        public static string encodeEscapeChar(string _str)
        {
            if (_str.Contains(@"\"))
            {
                _str = _str.Replace(@"\", @"\E\");
            }

            if (_str.Contains(@"|"))
            {
                _str = _str.Replace(@"|", @"\F\");
            }

            if (_str.Contains(@"~"))
            {
                _str = _str.Replace(@"~", @"\R\");
            }

            if (_str.Contains(@"^"))
            {
                _str = _str.Replace(@"^", @"\S\");
            }

            if (_str.Contains(@"&"))
            {
                _str = _str.Replace(@"&", @"\T\");
            }

            return _str;
        }

        public static string generateACK(string _messageType, string _messageControlId, string _ackCode, string _textTessage)
        {
            string message = string.Format(@"MSH|^~\&|MazikCare|MazikCare|LDM|Bupa Main Clinics,52|{2}||ACK^{0}^ACK|{1}|P|2.3|||NE", _messageType, _messageControlId, DateTime.Now.ToString("yyyyMMddHHmm"));
            
            message += string.Format(@"{3}MSA|{0}|{1}|{2}", _ackCode, _messageControlId, _textTessage, Environment.NewLine);
                      
            return message;
        }
    }
}
