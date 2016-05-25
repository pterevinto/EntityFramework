// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
    public class WarningsConfigurationBuilder
    {
        public virtual WarningsConfiguration Configuration { get; } = new WarningsConfiguration();

        public virtual WarningsConfigurationBuilder DefaultBehavior(WarningBehavior warningBehavior)
        {
            Configuration.DefaultBehavior = warningBehavior;

            return this;
        }

        public virtual WarningsConfigurationBuilder Throw(
            [NotNull] params CoreLoggingEventId[] coreLoggingEventIds)
        {
            Check.NotNull(coreLoggingEventIds, nameof(coreLoggingEventIds));

            Configuration.AddExplicit(coreLoggingEventIds.Cast<object>(), WarningBehavior.Throw);

            return this;
        }

        public virtual WarningsConfigurationBuilder Log(
            [NotNull] params CoreLoggingEventId[] coreLoggingEventIds)
        {
            Check.NotNull(coreLoggingEventIds, nameof(coreLoggingEventIds));

            Configuration.AddExplicit(coreLoggingEventIds.Cast<object>(), WarningBehavior.Log);

            return this;
        }

        public virtual WarningsConfigurationBuilder Ignore(
            [NotNull] params CoreLoggingEventId[] coreLoggingEventIds)
        {
            Check.NotNull(coreLoggingEventIds, nameof(coreLoggingEventIds));

            Configuration.AddExplicit(coreLoggingEventIds.Cast<object>(), WarningBehavior.Ignore);

            return this;
        }
    }
}
