using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace App.Controllers
{
    public class md5
    {
        public static string CreateMD5(string s)
        {
            if (s!="") { 
                using (MD5 md5 = MD5.Create())
                {
                    var encoding = Encoding.ASCII;
                    var data = encoding.GetBytes(s);

                    Span<byte> hashBytes = stackalloc byte[16];
                    md5.TryComputeHash(data, hashBytes, out int written);
                    if (written != hashBytes.Length)
                        throw new OverflowException();


                    Span<char> stringBuffer = stackalloc char[32];
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        hashBytes[i].TryFormat(stringBuffer.Slice(2 * i), out _, "x2");
                    }
                    return new string(stringBuffer);
                }
            }
            return "";
        }

    }
}
