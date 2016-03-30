using System;

using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public class GetDGConfigurationCommandResult : BaseCommandResult
    {

        private DG200Configuration _config;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public GetDGConfigurationCommandResult() : base()
        {

        }

        protected override void processBuffer()
        {
            this._config = new DG200Configuration(this.getCurrentBuffer());
        }

        /// <summary>
        /// Returns a string containing the config in a human readable format.
        /// </summary>
        /// <returns>A string with each config setting.</returns>
        public string printConfig()
        {
            if (this._config != null)
            {
                return this._config.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Returns the just retrieved configuration.
        /// </summary>
        /// <returns>A DG200Configuration instance.</returns>
        public DG200Configuration getConfiguration()
        {
            return this._config;
        }
    }
}
