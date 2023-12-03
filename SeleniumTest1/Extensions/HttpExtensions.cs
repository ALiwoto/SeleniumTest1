using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest1.Extensions
{
    public static class HttpExtensions
    {
        public static bool AddHeader(this HttpRequestMessage? request, string name, string? value) =>
            request?.Headers?.TryAddWithoutValidation(name, value) ?? false;
    }
}
