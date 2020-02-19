// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETSTANDARD2_0 || NET40
using System;
using System.Diagnostics;
using System.IO;
using TestCentric.Engine.Helpers;
using TestCentric.Engine.Services;

namespace TestCentric.Engine.Transports
{
    public interface ITestAgencyTransport : ITestAgency, IDisposable
    {
        TestAgency Agency { get; }
        string ConnectionPoint { get; }
        bool IsRuntimeSupported(RuntimeFramework runtime);
        void Start();
        void Stop();
    }
}
#endif
