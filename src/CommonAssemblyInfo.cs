// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric GUI contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

using System.Reflection;

//
// Common Information for the TestCentric Runner 
//

[assembly: AssemblyProduct("TestCentric Runner for NUnit")]

[assembly: AssemblyCompany("TestCentric Software")]
[assembly: AssemblyCopyright("Copyright (C) 2018-2020 Charlie Poole and TestCentric Runner contributors")]
[assembly: AssemblyTrademark("TestCentric is a trademark of TestCentric Software")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyVersion("1.4.1.0")]
[assembly: AssemblyFileVersion("1.4.1")]
[assembly: AssemblyInformationalVersion("1.4.1-ci00003-issue-579")]
