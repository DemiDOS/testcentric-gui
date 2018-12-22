﻿// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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

using NUnit.Framework;
using NSubstitute;

namespace TestCentric.Gui.Presenters
{
    using Views;
    using Model;
    using Controls;

    public class StatusBarPresenterTests
    {
        private IStatusBarView _view;
        private ITestModel _model;
        private StatusBarPresenter _presenter;

        [SetUp]
        public void CreatePresenter()
        {
            _view = Substitute.For<IStatusBarView>();
            _model = Substitute.For<ITestModel>();

            _presenter = new StatusBarPresenter(_view, _model);
        }

        [TearDown]
        public void RemovePresenter()
        {
            //if (_presenter != null)
            //    _presenter.Dispose();

            _presenter = null;
        }

        //[Test]
        //public void WhenProjectIsUnloaded_ProgressBar_IsInitialized()
        //{
        //    _settings.IsProjectLoaded.Returns(false);
        //    _settings.IsTestLoaded.Returns(false);
        //    _settings.IsTestRunning.Returns(false);
        //    _settings.ProjectUnloaded += Raise.Event<TestEventHandler>(new TestEventArgs(TestAction.ProjectUnloaded, "Dummy"));

        //    _view.Received().Initialize(100);
        //}

        [Test]
        public void WhenTestsAreUnloaded_StatusBar_IsInitialized()
        {
            _model.HasTests.Returns(false);
            _model.IsTestRunning.Returns(false);
            _model.Events.TestUnloaded += Raise.Event<TestEventHandler>(new TestEventArgs());

            _view.Received().OnTestUnloaded();
        }

        [Test]
        public void WhenTestRunBegins_StatusBar_IsInitialized()
        {
            _model.HasTests.Returns(true);
            _model.IsTestRunning.Returns(true);
            _model.Events.RunStarting += Raise.Event<RunStartingEventHandler>(new RunStartingEventArgs(1234));

            _view.Received().OnRunStarting(1234);
        }

        [Test]
        public void WhenTestCaseCompletes_CountIsIncremented()
        {
            var result = new ResultNode(XmlHelper.CreateXmlNode("<test-case id='1'/>"));

            _model.Events.TestFinished += Raise.Event<TestResultEventHandler>(new TestResultEventArgs(result));

            _view.Received().OnTestPassed();
        }

        [Test]
        public void WhenTestsFinish_TestResultsAreCalculated()
        {
            var result = new ResultNode(XmlHelper.CreateXmlNode("<test-run/>"));

            _model.Events.RunFinished += Raise.Event<TestResultEventHandler>(new TestResultEventArgs(result));

            _view.Received().OnRunFinished(0);
            _view.Received().OnTestRunSummaryCompiled(Arg.Any<string>());
        }

        //[Test]
        //public void WhenTestSuiteCompletes_ProgressIsNotIncremented()
        //{
        //    int priorValue = _view.Progress;
        //    var suiteResult = new ResultNode(XmlHelper.CreateXmlNode("<test-suite id='1'/>"));

        //    _settings.SuiteFinished += Raise.Event<TestResultEventHandler>(new TestResultEventArgs(suiteResult));

        //    Assert.That(_view.Progress, Is.EqualTo(priorValue));
        //}

        //[Test]
        //public void WhenTestSuiteCompletes_ResultIsPostedToView()
        //{
        //    int priorValue = _view.Progress;
        //    var suiteResult = new ResultNode(XmlHelper.CreateTopLevelElement("test-suite"));

        //    _settings.TestFinished += Raise.Event<TestEventHandler>(new TestEventArgs(TestAction.TestFinished, suiteResult));

        //    Assert.That(_view.Progress, Is.EqualTo(priorValue));
        //}

        static object[] statusTestCases = new object[] {
            new object[] { ProgressBarStatus.Success, ResultState.Failure, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Warning, ResultState.Failure, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Failure, ResultState.Failure, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Success, ResultState.Error, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Warning, ResultState.Error, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Failure, ResultState.Error, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Success, ResultState.NotRunnable, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Warning, ResultState.NotRunnable, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Failure, ResultState.NotRunnable, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Success, ResultState.Ignored, ProgressBarStatus.Warning },
            new object[] { ProgressBarStatus.Warning, ResultState.Ignored, ProgressBarStatus.Warning },
            new object[] { ProgressBarStatus.Failure, ResultState.Ignored, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Success, ResultState.Inconclusive, ProgressBarStatus.Success },
            new object[] { ProgressBarStatus.Warning, ResultState.Inconclusive, ProgressBarStatus.Warning },
            new object[] { ProgressBarStatus.Failure, ResultState.Inconclusive, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Success, ResultState.Skipped, ProgressBarStatus.Success },
            new object[] { ProgressBarStatus.Warning, ResultState.Skipped, ProgressBarStatus.Warning },
            new object[] { ProgressBarStatus.Failure, ResultState.Skipped, ProgressBarStatus.Failure },
            new object[] { ProgressBarStatus.Success, ResultState.Success, ProgressBarStatus.Success },
            new object[] { ProgressBarStatus.Warning, ResultState.Success, ProgressBarStatus.Warning },
            new object[] { ProgressBarStatus.Failure, ResultState.Success, ProgressBarStatus.Failure }
        };

        //[TestCaseSource("statusTestCases")]
        //public void BarShowsProperStatus(TestProgressBarStatus priorStatus, ResultState resultState, TestProgressBarStatus expectedStatus)
        //{
        //    _view.Status = priorStatus;

        //    var doc = new XmlDocument();
        //    if (resultState.Label == string.Empty)
        //        doc.LoadXml(string.Format("<test-case id='1' suiteResult='{0}'/>", resultState.Status));
        //    else
        //        doc.LoadXml(string.Format("<test-case id='1' suiteResult='{0}' label='{1}'/>", resultState.Status, resultState.Label));
        //    var suiteResult = new ResultNode(doc.FirstChild);

        //    _settings.IsProjectLoaded.Returns(true);
        //    _settings.IsTestLoaded.Returns(true);
        //    _settings.LoadedTests.Returns(suiteResult);
        //    _settings.TestLoaded += Raise.Event<TestEventHandler>(new TestEventArgs(TestAction.TestLoaded, "Dummy", suiteResult));
        //    _settings.TestFinished += Raise.Event<TestResultEventHandler>(new TestResultEventArgs(TestAction.TestFinished, suiteResult));

        //    Assert.That(_view.Status, Is.EqualTo(expectedStatus));
        //}
    }
}
