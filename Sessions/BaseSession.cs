using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.logging;
using kimandtodd.DG200CSharp.commands.exceptions;

namespace kimandtodd.DG200CSharp.sessions
{
    public class BaseSession
    {
        protected bool _commandHeaderDataEvaluated;
        protected bool _sizeDataEvaluated;

        protected int _expectedByteCount;

        protected List<CommandBuffer> _buffers;

        protected ICommandResult _currentResult;

        public BaseSession()
        {
            this.reset();
            
            this._buffers = new List<CommandBuffer>();
            this._buffers.Add(new CommandBuffer());
        }

        protected void reset()
        {
            this._commandHeaderDataEvaluated = false;
            this._sizeDataEvaluated = false;
            this._expectedByteCount = -1; // We don't know yet.
        }

        /// <summary>
        /// Evaluates the stream as it comes in. Initially used to determine if the array starts as expected,
        /// but also calculates the expected payload size for the entire stream. Either continues on merrily,
        /// or throws exceptions if the expectation is the data can't be used. 
        /// </summary>
        protected virtual void evaluateData()
        {
            // If we don't even have two bytes, we can't get started. 
            if (this.getCurrentBuffer().Length < 2)
            {
                return;
            }
            // Evaluate the payload as enough bytes come in. 
            this.evaluateCommandHeaderData();
            this.evaluatePayloadSizeData();
        }

        /// <summary>
        /// Read the command header and make sure that it is something we are interested in.
        /// </summary>
        protected void evaluateCommandHeaderData()
        {
            if (!this._commandHeaderDataEvaluated)
            {
                this._commandHeaderDataEvaluated = true;

                byte[] header = new byte[2];
                this.getCurrentBuffer().Position = 0;
                int count = this.getCurrentBuffer().Read(header, 0, 2);
                this.getCurrentBuffer().Position = this.getCurrentBuffer().Length;
                if (!this.isCommandHeader(header))
                {
                    throw new CommandException("The connection attempt failed. Either the device is not a DG200 or it is a DG200 that is not turned on.");
                }
            }
        }

        /// <summary>
        /// Read the payload size data and store the result internally.
        /// </summary>
        protected void evaluatePayloadSizeData()
        {
            // Once we get enough data in the buffer, evaluate the payload size. 
            if (!this._sizeDataEvaluated && this.getCurrentBuffer().Length > 3)
            {
                this._sizeDataEvaluated = true;
                byte[] sizeArr = new byte[2];
                this.getCurrentBuffer().Position = 2;
                this.getCurrentBuffer().Read(sizeArr, 0, 2);
                this.getCurrentBuffer().Position = this.getCurrentBuffer().Length;
                this._expectedByteCount = this.calculateExpectedBytes(sizeArr);
                DG200FileLogger.Log("BaseSession calculated payload size: " + this._expectedByteCount, 3);
                this.overrideExpectedByteCount();
            }
        }

        /// <summary>
        /// Figures out if the byte array matches the command header. 
        /// </summary>
        /// <param name="bytes">The array of bytes to evaluate.</param>
        /// <returns>True if the arrays match, false otherwise.</returns>
        private bool isCommandHeader(byte[] bytes)
        {
            for (int inx = 0; inx < BaseCommand.commandHeader.Length; inx++)
            {
                if (bytes[inx] != BaseCommand.commandHeader[inx])
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool continueReading()
        {
            // If we haven't even seen the payload size data, say yes.
            if (!this._sizeDataEvaluated)
            {
                DG200FileLogger.Log("BaseSession has not received enough bytes to evaluate size.", 3);
                return true;
            }
            else
            {
                DG200FileLogger.Log("BaseSession comparing bytes read (" + this.getCurrentBuffer().Length + ") to expected (" + this._expectedByteCount + ").", 3);
                bool _continue = this.getCurrentBuffer().Length < this._expectedByteCount;
                if (!_continue)
                {
                    DG200FileLogger.Log("BaseSession has read expected amount data and is starting the result processing.", 3);
                    // Initialize the result and save the value on requesting an additional session.
                    this.processResult();
                }

                return _continue;
            }
        }

        /// <summary>
        /// Converts the two-byte, big-endian, payload size value into an x64 little-endian integer. 
        /// </summary>
        /// <param name="payloadSize">The two byte array with the payload value in it.</param>
        /// <returns>The integer of the payload size. Includes eight bytes of padding included in every message.</returns>
        private int calculateExpectedBytes(byte[] payloadSize)
        {
            int total = 0;

            total = payloadSize[0] << 8;
            total += payloadSize[1] & 255;

            // Add eight bytes for the following (2 bytes each):  start and end sequences, the payload length, the checksum
            total += 8;

            return total;
        }

        /// <summary>
        /// Allows descendant classes to override the expected byte count, 
        /// as some commands lie about their payload sizes. 
        /// </summary>
        protected virtual void overrideExpectedByteCount()
        {
            // No-op here. 
        }

        /// <summary>
        /// Can be overidden. 
        /// </summary>
        protected virtual void processResult()
        {
            this._currentResult.addResultBuffer(this.getCurrentBuffer());
        }

        public virtual void Write(byte[] bytes, Int32 byteCount)
        {
            // Write to our local buffer.
            this.getCurrentBuffer().Write(bytes, 0, byteCount);
            // Figure out what we have. 
            this.evaluateData();
        }  
      
        /// <summary>
        /// Should we execute another call to the device. Almost always the answer is no.
        /// </summary>
        /// <returns>True if yes, false if no.</returns>
        public virtual bool callAgain()
        {
            return false;
        }

        protected virtual CommandBuffer getCurrentBuffer()
        {
            return this._buffers[this._buffers.Count - 1];
        }

        protected int getBufferCount()
        {
            return this._buffers.Count;
        }

        public virtual void setResult(ICommandResult newRes)
        {
            this._currentResult = newRes;
        }
    }
}
