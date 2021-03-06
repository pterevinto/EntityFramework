// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    public class AsyncGroupJoinInclude : IDisposable
    {
        private readonly IReadOnlyList<INavigation> _navigationPath;
        private readonly IReadOnlyList<Func<QueryContext, IAsyncRelatedEntitiesLoader>> _relatedEntitiesLoaderFactories;
        private readonly bool _querySourceRequiresTracking;

        private RelationalQueryContext _queryContext;
        private IAsyncRelatedEntitiesLoader[] _relatedEntitiesLoaders;
        private AsyncGroupJoinInclude _previous;

        public AsyncGroupJoinInclude(
            [NotNull] IReadOnlyList<INavigation> navigationPath,
            [NotNull] IReadOnlyList<Func<QueryContext, IAsyncRelatedEntitiesLoader>> relatedEntitiesLoaderFactories,
            bool querySourceRequiresTracking)
        {
            _navigationPath = navigationPath;
            _relatedEntitiesLoaderFactories = relatedEntitiesLoaderFactories;
            _querySourceRequiresTracking = querySourceRequiresTracking;
        }

        public virtual void SetPrevious([NotNull] AsyncGroupJoinInclude previous)
        {
            if (_previous != null)
            {
                _previous.SetPrevious(previous);
            }
            else
            {
                _previous = previous;
            }
        }

        public virtual void Initialize([NotNull] RelationalQueryContext queryContext)
        {
            _queryContext = queryContext;
            _queryContext.BeginIncludeScope();

            _relatedEntitiesLoaders
                = _relatedEntitiesLoaderFactories.Select(f => f(queryContext))
                    .ToArray();

            _previous?.Initialize(queryContext);
        }

        public virtual async Task IncludeAsync([CanBeNull] object entity, CancellationToken cancellationToken)
        {
            if (_previous != null)
            {
                await _previous.IncludeAsync(entity, cancellationToken);
            }

            await _queryContext.QueryBuffer
                .IncludeAsync(
                    _queryContext,
                    entity,
                    _navigationPath,
                    _relatedEntitiesLoaders,
                    _querySourceRequiresTracking,
                    cancellationToken);
        }

        public virtual void Dispose()
        {
            if (_queryContext != null)
            {
                _previous?.Dispose();

                foreach (var relatedEntitiesLoader in _relatedEntitiesLoaders)
                {
                    relatedEntitiesLoader.Dispose();
                }

                _queryContext.EndIncludeScope();
            }
        }
    }
}
