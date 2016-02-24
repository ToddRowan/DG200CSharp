using System;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace kimandtodd.DG200CSharp
{
    public interface IDataOutput
    {
        void Output(byte[] dataBytes, int dataLength = 0);
        void Finish();
    }
}