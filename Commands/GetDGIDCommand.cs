using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using kimandtodd.DG200CSharp.commandresults;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGIDCommand : BaseCommand
    {
        private static byte[] commandArray = new byte[] { 0xBF };

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public GetDGIDCommand() : base()
        {

        }

        /// <summary>
        /// Gets the command data for the serial connector. 
        /// </summary>
        /// <returns>A byte array with the command data.</returns>
        public override byte[] getCommandData()
        {
            return buildCommandArray(GetDGIDCommand.commandArray);
        }

        protected override void processResult()
        {
            this._currentResult = new GetDGIDCommandResult(this._buf);
        }
    }
}
