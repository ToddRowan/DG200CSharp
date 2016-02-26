using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp.commands
{
    public class CommandBuffer : MemoryStream
    {

        public CommandBuffer() : this(0)
        {

        }

        public CommandBuffer(Int32 initialSize) : base(initialSize)
        {
            
        }

    }
}
