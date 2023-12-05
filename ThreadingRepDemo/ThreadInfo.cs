using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadingRepDemo
{
    internal class ThreadInfo
    {
        public Thread CurrentThread { get; set; }
        public CancellationTokenSource Cts { get; set; }
    }
}
