// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    [DebuggerDisplay("{DeclaringEntityType.Name,nq}.{Name,nq}")]
    public class Navigation : PropertyBase, IMutableNavigation
    {
        // Warning: Never access these fields directly as access needs to be thread-safe
        private IClrCollectionAccessor _collectionAccessor;

        public Navigation([NotNull] PropertyInfo navigationProperty, [NotNull] ForeignKey foreignKey)
            : base(Check.NotNull(navigationProperty, nameof(navigationProperty)).Name, navigationProperty)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            ForeignKey = foreignKey;
        }

        public Navigation([NotNull] string navigationName, [NotNull] ForeignKey foreignKey)
            : base(navigationName, null)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            ForeignKey = foreignKey;
        }

        public virtual ForeignKey ForeignKey { get; }

        public override EntityType DeclaringEntityType
            => this.IsDependentToPrincipal()
                ? ForeignKey.DeclaringEntityType
                : ForeignKey.PrincipalEntityType;

        public override string ToString() => DeclaringEntityType + "." + Name;

        public static PropertyInfo GetClrProperty(
            [NotNull] string navigationName,
            [NotNull] EntityType sourceType,
            [NotNull] EntityType targetType,
            bool shouldThrow)
        {
            var sourceClrType = sourceType.ClrType;
            var navigationProperty = sourceClrType?.GetPropertiesInHierarchy(navigationName).FirstOrDefault();
            if (!IsCompatible(navigationName, navigationProperty, sourceType, targetType, null, shouldThrow))
            {
                return null;
            }

            return navigationProperty;
        }

        public static bool IsCompatible(
            [NotNull] string navigationName,
            [CanBeNull] PropertyInfo navigationProperty,
            [NotNull] EntityType sourceType,
            [NotNull] EntityType targetType,
            bool? shouldBeCollection,
            bool shouldThrow)
        {
            if (navigationProperty == null)
            {
                if (shouldThrow)
                {
                    throw new InvalidOperationException(CoreStrings.NoClrNavigation(navigationName, sourceType.DisplayName()));
                }
                return false;
            }

            var targetClrType = targetType.ClrType;
            if (targetClrType == null)
            {
                if (shouldThrow)
                {
                    throw new InvalidOperationException(
                        CoreStrings.NavigationToShadowEntity(navigationName, sourceType.DisplayName(), targetType.DisplayName()));
                }
                return false;
            }

            return IsCompatible(navigationProperty, sourceType.ClrType, targetClrType, shouldBeCollection, shouldThrow);
        }

        public static bool IsCompatible(
            [NotNull] PropertyInfo navigationProperty,
            [NotNull] Type sourceClrType,
            [NotNull] Type targetClrType,
            bool? shouldBeCollection,
            bool shouldThrow)
        {
            if (!navigationProperty.DeclaringType.GetTypeInfo().IsAssignableFrom(sourceClrType.GetTypeInfo()))
            {
                if (shouldThrow)
                {
                    throw new InvalidOperationException(CoreStrings.NoClrNavigation(
                        navigationProperty.Name, sourceClrType.DisplayName(fullName: false)));
                }
                return false;
            }

            var navigationTargetClrType = navigationProperty.PropertyType.TryGetSequenceType();
            if (shouldBeCollection == false
                || navigationTargetClrType == null
                || !navigationTargetClrType.GetTypeInfo().IsAssignableFrom(targetClrType.GetTypeInfo()))
            {
                if (shouldBeCollection == true)
                {
                    if (shouldThrow)
                    {
                        throw new InvalidOperationException(CoreStrings.NavigationCollectionWrongClrType(
                            navigationProperty.Name,
                            sourceClrType.DisplayName(fullName: false),
                            navigationProperty.PropertyType.DisplayName(fullName: false),
                            targetClrType.DisplayName(fullName: false)));
                    }
                    return false;
                }

                if (!navigationProperty.PropertyType.GetTypeInfo().IsAssignableFrom(targetClrType.GetTypeInfo()))
                {
                    if (shouldThrow)
                    {
                        throw new InvalidOperationException(CoreStrings.NavigationSingleWrongClrType(
                            navigationProperty.Name,
                            sourceClrType.DisplayName(fullName: false),
                            navigationProperty.PropertyType.DisplayName(fullName: false),
                            targetClrType.DisplayName(fullName: false)));
                    }
                    return false;
                }
            }

            return true;
        }

        public virtual Navigation FindInverse()
            => (Navigation)((INavigation)this).FindInverse();

        public virtual EntityType GetTargetType()
            => (EntityType)((INavigation)this).GetTargetType();

        public virtual IClrCollectionAccessor CollectionAccessor
            => NonCapturingLazyInitializer.EnsureInitialized(ref _collectionAccessor, this, n => new ClrCollectionAccessorFactory().Create(n));

        IForeignKey INavigation.ForeignKey => ForeignKey;
        IMutableForeignKey IMutableNavigation.ForeignKey => ForeignKey;
        IEntityType IPropertyBase.DeclaringEntityType => DeclaringEntityType;
        IMutableEntityType IMutableNavigation.DeclaringEntityType => DeclaringEntityType;
    }
}
