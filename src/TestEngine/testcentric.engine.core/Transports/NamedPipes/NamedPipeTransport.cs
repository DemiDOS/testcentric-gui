// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETSTANDARD2_0 || NET40
using System;
using System.IO.Pipes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Threading.Tasks;

namespace TestCentric.Engine.Transports.NamedPipes
{
    public class NamedPipeTransport : IDisposable
    {
        private BinaryFormatter _formatter = new BinaryFormatter();

        public NamedPipeTransport(PipeStream pipeStream)
        {
            PipeStream = pipeStream;
            IsConnected = pipeStream.IsConnected;
        }

        public PipeStream PipeStream { get; }

        public bool IsConnected { get; private set; }

        public void WriteBytes(byte[] bytes)
        {
            PipeStream.Write(bytes, 0, bytes.Length);
        }

        public void WriteObject(object graph)
        {
            PipeStream.WriteObject(graph);
        }

        public void WriteResult(object result)
        {
            PipeStream.WriteObject(result);
        }

        public byte[] ReadBytes(int count)
        {
            var buf = new byte[count];
            PipeStream.Read(buf, 0, count);
            return buf;
        }

        public T ReadObject<T>()
        {
            return (T)ReadObject();
        }

        public NamedPipeCommand ReadCommand()
        {
            return ReadObject<NamedPipeCommand>();
        }

        public object ReadObject()
        {
            return PipeStream.ReadObject();
        }

        public void Dispose()
        {
            PipeStream.Dispose();
        }
    }

    [Serializable]
    public abstract class NamedPipeCommand
    {
        public readonly string Name;

        public NamedPipeCommand(string name)
        {
            Name = name;
        }

        public virtual void WriteTo(PipeStream pipe)
        {
            pipe.WriteObject(this);
        }
    }
}
#endif
