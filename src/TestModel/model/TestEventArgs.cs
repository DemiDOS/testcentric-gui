// ***********************************************************************
// Copyright (c) 2018 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace TestCentric.Gui.Model
{
    /// <summary>
    /// Argument used for all test events
    /// </summary>
    public class TestEventArgs : EventArgs
    {
    }

    public class TestNodeEventArgs : TestEventArgs
    {
        public TestNodeEventArgs(TestNode test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            Test = test;
        }

        public TestNode Test { get; }
    }

    public class RunStartingEventArgs : TestEventArgs
    {
        public RunStartingEventArgs(int testCount)
        {
            TestCount = testCount;
        }

        public int TestCount { get; set; }
    }

    public class TestResultEventArgs : TestEventArgs
    {
        public TestResultEventArgs(ResultNode result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            Result = result;
        }

        public ResultNode Result { get; set; }
    }

    public class TestItemEventArgs : EventArgs
    {
        public TestItemEventArgs(ITestItem testItem)
        {
            TestItem = testItem;
        }

        public ITestItem TestItem { get; private set; }
    }

    public class TestOutputEventArgs : EventArgs
    {
        public TestOutputEventArgs(string testName, string stream, string text)
        {
            TestName = testName;
            Stream = stream;
            Text = text;
        }

        public string TestName { get; }
        public string Stream { get; }
        public string Text { get; }
    }

    public class UnhandledExceptionEventArgs : EventArgs
    {
        public UnhandledExceptionEventArgs(string message, string stackTrace)
        {
            Message = message;
            StackTrace = stackTrace;
        }

        public string Message;
        public string StackTrace;
    }

    public class TestFilesLoadingEventArgs : EventArgs
    {
        public TestFilesLoadingEventArgs(IList<string> testFilesLoading)
        {
            TestFilesLoading = testFilesLoading;
        }

        public IList<string> TestFilesLoading { get; }
    }
}
