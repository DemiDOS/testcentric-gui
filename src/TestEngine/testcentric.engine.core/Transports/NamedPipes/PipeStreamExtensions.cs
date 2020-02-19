// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETSTANDARD2_0 || NET40
using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace TestCentric.Engine.Transports.NamedPipes
{
    /// <summary>
    /// Extension Methods for use with PipeStreams.
    /// </summary>
    public static class PipeStreamExtensions
    {
        // NOTE: The ReadObject methods will not always succeed when
        // used by themselves. There may be no data available or
        // the opertion may time out. They must be used in a context
        // where exceptions are handled and the operation is retried
        // if necessary.

        public static T ReadObject<T>(this PipeStream pipe)
        {
            return (T)pipe.ReadObject();
        }

        public static object ReadObject(this PipeStream pipe)
        {
            var lenBuf = new byte[sizeof(int)];
            int nBytes = 0;
            do
            {
                nBytes = pipe.Read(lenBuf, 0, sizeof(int));
            } while (nBytes == 0);
            //if (nBytes == 0)
            //    return null;

            if (nBytes != sizeof(int))
                throw new Exception($"Expected {sizeof(int)} bytes but read {nBytes}");

            int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lenBuf, 0));

            var buf = new byte[length];
            pipe.Read(buf, 0, length);
            using (var stream = new MemoryStream(buf))
            {
                return new BinaryFormatter().Deserialize(stream);
            }
        }

        public static void WriteObject(this PipeStream pipe, object graph)
        {
            var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, graph);
            var bytes = stream.ToArray();
            int length = bytes.Length;
            int lenSize = sizeof(int);
            var lenBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));
            pipe.Write(lenBytes, 0, lenSize);
            pipe.Write(bytes, 0, length);
            pipe.Flush();
            pipe.WaitForPipeDrain();
        }
    }
}
#endif
