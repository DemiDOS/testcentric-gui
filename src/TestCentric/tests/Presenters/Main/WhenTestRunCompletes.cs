// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric GUI contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

using NSubstitute;
using NUnit.Framework;

namespace TestCentric.Gui.Presenters.Main
{
    using Model;

    public class WhenTestRunCompletes : MainPresenterTestBase
    {
        [SetUp]
        public void SimulateTestRunFinish()
        {
            ClearAllReceivedCalls();

            _model.HasTests.Returns(true);
            _model.HasResults.Returns(true);
            _model.IsTestRunning.Returns(false);

            var resultNode = new ResultNode("<test-run/>");
            FireRunFinishedEvent(resultNode);
        }

#if NYI // Add after implementation of project or package saving
        [TestCase("NewProjectCommand", true)]
        [TestCase("OpenProjectCommand", true)]
        [TestCase("SaveCommand", true)]
        [TestCase("SaveAsCommand", true)
#endif

        [TestCase("RunButton", true)]
        [TestCase("StopButton", false)]
        [TestCase("OpenCommand", true)]
        [TestCase("CloseCommand", true)]
        [TestCase("AddTestFilesCommand", true)]
        [TestCase("ReloadTestsCommand", true)]
        [TestCase("RuntimeMenu", false)]
        [TestCase("RecentFilesMenu", true)]
        [TestCase("ExitCommand", true)]
        [TestCase("RunAllCommand", true)]
        [TestCase("RunSelectedCommand", true)]
        [TestCase("RunFailedCommand", true)]
        [TestCase("TestParametersCommand", true)]
        [TestCase("StopRunCommand", false)]
        [TestCase("SaveResultsCommand", true)]
        public void CheckCommandEnabled(string propName, bool enabled)
        {
            ViewElement(propName).Received().Enabled = enabled;
        }
    }
}
