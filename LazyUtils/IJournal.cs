using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyUtils
{
    public interface IJournal
    {
        string ReadAllText(string path);

        void WriteAllText(string path, string contents);
    }
}
