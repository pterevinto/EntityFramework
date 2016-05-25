// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Sqlite.FunctionalTests
{
    public class QueryNoClientEvalSqliteFixture : NorthwindQuerySqliteFixture
    {
        protected override DbContextOptionsBuilder ConfigureOptions(DbContextOptionsBuilder dbContextOptionsBuilder)
            => dbContextOptionsBuilder.ConfigureWarnings(c => c.DefaultBehavior(WarningBehavior.Throw));
    }
}
