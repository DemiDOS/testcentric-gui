﻿// ***********************************************************************
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
using NUnit.Framework;
using NUnit.UiException.Controls;
using System.Threading;
using System.Windows.Forms;

namespace NUnit.UiException.Tests.Controls
{
    [TestFixture]
    public class TestStackTraceDisplay
    {
        private StackTraceDisplay _traceDisplay;

        [SetUp]
        public void SetUp()
        {
            _traceDisplay = new StackTraceDisplay();
        }

        [Test]
        public void DefaultState()
        {
            Assert.NotNull(_traceDisplay.PluginItem);
            Assert.That(_traceDisplay.PluginItem.Text, Is.EqualTo("Display actual stack trace"));
            Assert.NotNull(_traceDisplay.OptionItems);
            Assert.That(_traceDisplay.OptionItems.Length, Is.EqualTo(1));
            Assert.That(_traceDisplay.OptionItems[0].Text, Is.EqualTo("Copy stack trace to clipboard"));

            Assert.That(_traceDisplay.Content, Is.Not.Null);
            Assert.That(_traceDisplay.Content, Is.TypeOf(typeof(TextBox)));
            TextBox text = _traceDisplay.Content as TextBox;            
            Assert.That(text.Text, Is.EqualTo(""));
            Assert.That(text.ReadOnly, Is.True);
            Assert.That(text.Multiline, Is.True);

            return;
        }

        [Test]
        public void OnStackTraceChanged()
        {
            string trace_1 = 
                    "à System.ThrowHelper.ThrowArgumentException(ExceptionResource resource)\r\n" +
                    "à System.Collections.Generic.Dictionary`2.Insert(TKey key, TValue value, Boolean add)\r\n" +
                    "à System.Collections.Generic.Dictionary`2.Add(TKey key, TValue value)\r\n" +
                    "à NUnit.UiException.Tests.MockHelper.Two_Mocks_In_Dictionary() dans C:\\folder\\file1.cs:ligne 87\r\n";

            string trace_2 = "";
            TextBox content = _traceDisplay.Content as TextBox;

            _traceDisplay.OnStackTraceChanged(trace_1);
            Assert.That(content.Text, Is.EqualTo(trace_1));

            _traceDisplay.OnStackTraceChanged(trace_2);
            Assert.That(content.Text, Is.EqualTo(trace_2));

            // passing null should not cause error

            _traceDisplay.OnStackTraceChanged(null);
            Assert.That(content.Text, Is.EqualTo(""));

            return;
        }

        [Test, Apartment(ApartmentState.STA)]
        public void CopyToClipBoard()
        {
            Clipboard.Clear();

            _traceDisplay.OnStackTraceChanged("hi, there!");
            _traceDisplay.CopyToClipBoard();

            Assert.That(Clipboard.ContainsText(), Is.True);
            Assert.That(Clipboard.GetText(), Is.EqualTo("hi, there!"));

            // calling twice doesn't add twice same content

            _traceDisplay.CopyToClipBoard();
            _traceDisplay.CopyToClipBoard();
            Assert.That(Clipboard.GetText(), Is.EqualTo("hi, there!"));

            // test to fail: calling copy to clipboard
            // with an empty stack trace is valid

            _traceDisplay.OnStackTraceChanged("");
            _traceDisplay.CopyToClipBoard();
            Assert.That(Clipboard.GetText(), Is.EqualTo(""));

            return;
        }

        [Test]
        public void FeedingDisplayWithGarbageDoesNotMakeItCrash()
        {
            _traceDisplay.OnStackTraceChanged(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n" +
                "Nam at nisi ut neque sollicitudin ultrices. Sed rhoncus\r\n" +
                "rhoncus arcu. Morbi eu elit ut augue congue luctus. Nullam\r\n" +
                "eu eros. Nunc blandit varius orci. Mauris condimentum diam\r\n" +
                "ac ligula. Nullam ut metus. Maecenas sagittis nibh in nisl.\r\n" +
                "Phasellus rhoncus diam a nulla. Integer vestibulum.\r\n");

            TextBox text = _traceDisplay.Content as TextBox;
            Assert.That(text.Text, Is.Not.Null);

            return;
        }

    }
}