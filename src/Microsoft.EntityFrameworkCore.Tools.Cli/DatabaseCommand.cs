﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Extensions.CommandLineUtils;

namespace Microsoft.EntityFrameworkCore.Tools.Cli
{
    public class DatabaseCommand
    {
        public static void Configure([NotNull] CommandLineApplication command, [NotNull] CommonCommandOptions commonOptions)
        {
            command.Description = "Commands to manage your database";

            command.HelpOption();
            command.VerboseOption();

            command.Command("update", c => DatabaseUpdateCommand.Configure(c, commonOptions));
            command.Command("drop", c => DatabaseDropCommand.Configure(c, commonOptions));

            command.OnExecute(() => command.ShowHelp());
        }
    }
}
