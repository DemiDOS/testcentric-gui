﻿// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric GUI contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

using System;
using System.Windows.Forms;

namespace TestCentric.Gui
{
    public interface IMessageDisplay
    {
        DialogResult Display(string message);
        DialogResult Display(string message, MessageBoxButtons buttons);

        DialogResult Error(string message);
        DialogResult Error(string message, MessageBoxButtons buttons);
        DialogResult Error(string message, Exception exception);
        DialogResult Error(string message, Exception exception, MessageBoxButtons buttons);

        DialogResult FatalError(string message, Exception exception);

        DialogResult Info(string message);
        DialogResult Info(string message, MessageBoxButtons buttons);

        DialogResult Ask(string message);
        DialogResult Ask(string message, MessageBoxButtons buttons);
    }
}
