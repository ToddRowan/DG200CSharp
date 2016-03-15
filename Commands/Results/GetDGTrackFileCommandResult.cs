using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.commandresults.resultitems;


namespace kimandtodd.DG200CSharp.commandresults
{
    public class GetDGTrackFileCommandResult : BaseCommandResult
    {
        private UInt16 _trackFormat;
        private List<IDGTrackPoint> _tracks;
        private DGTrackPointFactory _factory;

        /// <summary>
        /// Constructor
        /// </summary>
        public GetDGTrackFileCommandResult() : base()
        {
            this._tracks = new List<IDGTrackPoint>();
        }

        /// <summary>
        /// Pass the buffer to our factory that will 
        /// </summary>
        protected override void processBuffer()
        {
            // Pass the buffer to the factory. 
            this._factory = new DGTrackPointFactory(this.getCurrentBuffer());

            // Ask the factory what kind of tracks we'll get.
            this._trackFormat = this._factory.getFormatType();

            // Get the trackpoint objects and store them. 
            foreach (IDGTrackPoint tp in this._factory.getTrackPoints())
            {
                this._tracks.Add(tp);
            }
        }

        /// <summary>
        /// Ask the result how many trackpoints there are.
        /// </summary>
        /// <returns>The complete track count.</returns>
        public int getTrackCount()
        {
            return this._tracks.Count;
        }
    }
}
