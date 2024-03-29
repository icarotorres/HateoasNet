﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace HateoasNet.Abstractions
{
    /// <summary>
    ///   Represents the entry point for configuring HATEOAS integration.
    /// </summary>
    public interface IHateoasContext
    {
        /// <summary>
        ///   Gets all applicable <see cref="IHateoasLinkBuilder" /> from a <see cref="IHateoasSource{T}" /> configuration
        ///   for the <see cref="Type"/> of <paramref name="source" /> to evaluate the applicability.
        /// </summary>
        /// <param name="source">An object used to evaluate the applicability.</param>
        /// <returns><see cref="IEnumerable{IHateoasLink}" /> with all applicable <see cref="IHateoasLinkBuilder" />.</returns>
        internal IEnumerable<IHateoasLinkBuilder> GetApplicableLinkBuilders(object source);

        internal IHateoasSource<T> GetOrInsert<T>() where T : class;

        internal IHateoasSource GetOrInsert(Type targetType);

        /// <summary>
        ///   Adds or continues an <see cref="IHateoasSource{T}" /> configuration of <typeparamref name="T" /> 
        ///   using an <see cref="Action{T}" /> of <see cref="IHateoasSource{T}" /> action.
        /// </summary>
        /// <param name="source">Action enabling configuration over the instance of IHateoasSource{T}.</param>
        /// <typeparam name="T">Target class for source configuration.</typeparam>
        /// <returns>Current <see cref="IHateoasContext" /> instance.</returns>
        IHateoasContext Configure<T>(Action<IHateoasSource<T>> source) where T : class;

        /// <summary>
        ///   Adds or continues an <see cref="IHateoasSource{T}" /> configuration of <typeparamref name="T" /> using
        ///   a <see cref="IHateoasSourceBuilder{T}" /> instance.
        /// </summary>
        /// <param name="configuration">
        ///   A <see cref="IHateoasSourceBuilder{T}" /> instance implementing the configuration of
        ///   <typeparamref name="T" /> in separated class.
        /// </param>
        /// <typeparam name="T">Target class for source configuration.</typeparam>
        /// <returns>Current <see cref="IHateoasContext" /> instance.</returns>
        IHateoasContext ApplyConfiguration<T>(IHateoasSourceBuilder<T> configuration) where T : class;

        /// <summary>
        ///   Applies all configurations of HATEOAS resources found in classes implementing
        ///   <see cref="IHateoasSourceBuilder{T}" />
        ///   on given <paramref name="assembly" /> through their <see cref="IHateoasSourceBuilder{T}.Build" /> method.
        /// </summary>
        /// <param name="assembly">Target assembly containing classes implementing <see cref="IHateoasSourceBuilder{T}" />.</param>
        /// <returns>Current <see cref="IHateoasContext" /> instance.</returns>
        IHateoasContext ConfigureFromAssembly(Assembly assembly);
    }
}
