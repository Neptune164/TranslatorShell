using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace TranslatorShell.Services
{
    internal class TranslatorClient
    {
        private static readonly HttpClient Http = new();

        // send a request to get a transpaltion result
        public async Task<string> TranslateAsync(string input) 
        {
            var encodedInput = Uri.EscapeDataString(input);
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=zh-CN&dt=t&q={encodedInput}"; ;
            try
            {
                var response = await Http.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);
                // parse the JSON data
                var sb = new StringBuilder();
                foreach (var sentence in doc.RootElement[0].EnumerateArray())
                {
                    var seg = sentence[0].GetString();
                    if (!string.IsNullOrEmpty(seg))
                        sb.Append(seg);
                }

                return $"[translate]:{sb}";
            }
            catch (Exception ex)
            {
                return $"[error]:{ex.Message}";
            }
        }
    }
}
