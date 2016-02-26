using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp.commands.exceptions

{
    public class CommandException : Exception
    {
        public CommandException(String msg) : base(msg)
        {

        }
    }
}
