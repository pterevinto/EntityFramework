// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class SqliteCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
    {
        private static readonly IMethodCallTranslator[] _sqliteTranslators =
        {
            new SqliteMathAbsTranslator(),
            new SqliteStringIsNullOrWhiteSpaceTranslator(),
            new SqliteStringToLowerTranslator(),
            new SqliteStringToUpperTranslator(),
            new SqliteStringTrimEndTranslator(),
            new SqliteStringTrimStartTranslator(),
            new SqliteStringTrimTranslator()
        };

        public SqliteCompositeMethodCallTranslator(
            [NotNull] ILogger<SqliteCompositeMethodCallTranslator> logger)
            : base(logger)
        {
            AddTranslators(_sqliteTranslators);
        }
    }
}
