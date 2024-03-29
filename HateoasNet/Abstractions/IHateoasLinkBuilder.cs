﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace HateoasNet.Abstractions
{
    /// <summary>
    ///   Represents a configuration of <see cref="HateoasLink"/> displayed to response output.
    /// </summary>
    public interface IHateoasLinkBuilder
    {
        /// <summary>
        ///   An endpoint Route Name assigned on an action method attribute.
        /// </summary>
        /// <value>
        ///   The <see langword="string" /> value for <see cref="HateoasLink.Rel" /> property of generated <see cref="HateoasLink"/>.
        /// </value>
        string RouteName { get; }

        /// <summary>
        ///   The optional custom presented name to override <see cref="RouteName"/> in <see cref="HateoasLink.Rel"/>.
        /// </summary>
        /// <value>
        ///   The <see langword="string" /> value for <see cref="HateoasLink.Rel" /> property of generated <see cref="HateoasLink"/>.
        /// </value>
        string PresentedName { get; }

        /// <summary>
        ///   Gets a <see cref="IDictionary{TKey, TValue}" /> of <see langword="string" />,
        ///   <see langword="object" /> representing the route values.
        /// </summary>
        /// <param name="source">
        ///   An object to be used as parameter of <see cref="IHateoasLinkBuilder{T}.RouteDictionaryFunction" />
        ///   to generate de dictionary.
        /// </param>
        /// <returns>
        ///   Generated route values dictionary <see cref="IDictionary{TKey, TValue}" /> of <see langword="string" />,
        ///   <see langword="object" />.
        /// </returns>
        internal IDictionary<string, object> GetRouteDictionary(object source);

        /// <summary>
        ///   Checks if this <see cref="IHateoasLinkBuilder" /> instance is satisfied by <paramref name="source" /> .
        /// </summary>
        /// <param name="source">
        ///   An object to be used as parameter of <see cref="IHateoasLinkBuilder{T}.Predicate" />
        ///   to evaluate predicate.
        /// </param>
        /// <returns>
        ///   <seealso langword="bool" /> if this <see cref="IHateoasLinkBuilder" /> for <paramref name="source" />
        ///   passes <see cref="IHateoasLinkBuilder{T}.Predicate" /> for applicability.
        /// </returns>
        bool IsSatisfiedBy(object source);
    }

    /// <summary>
    ///   Represents a <typeparamref name="T" /> configuration of <see cref="HateoasLink"/> displayed to response output.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHateoasLinkBuilder<T> : IHateoasLinkBuilder
    {
        /// <summary>
        ///   A framework function to generate <see cref="IDictionary{TKey, TValue}" /> of <see langword="string" />,
        ///   <see langword="object" /> representing the route values.
        /// </summary>
        internal Func<T, IDictionary<string, object>> RouteDictionaryFunction { get; }

        /// <summary>
        ///   An user defined predicate function to filter applicability of <see cref="IHateoasLinkBuilder{T}" />.
        /// </summary>
        internal Func<T, bool> Predicate { get; }

        /// <summary>
        ///   Receives an user defined function returning anonymous <see langword="object" /> for ease usage, converting it to
        ///   <see cref="RouteDictionaryFunction" /> internally.
        /// </summary>
        /// <param name="routeDataFunction">Function returning anonymous <see langword="object" /> for ease usage.</param>
        /// <returns>Current <see cref="IHateoasLinkBuilder{T}" /> instance.</returns>
        IHateoasLinkBuilder<T> HasRouteData(Func<T, object> routeDataFunction);

        /// <summary>
        ///   Receives an user defined predicate function to filter applicability of <see cref="IHateoasLinkBuilder{T}" />
        /// </summary>
        /// <param name="predicate">Predicate function to filter applicability of <see cref="IHateoasLinkBuilder{T}" />.</param>
        /// <returns>Current <see cref="IHateoasLinkBuilder{T}" /> instance.</returns>
        IHateoasLinkBuilder<T> When(Func<T, bool> predicate);

        /// <summary>
        ///     Overrides <see cref="HateoasLink.Rel"/> with <paramref name="presentedName"/> instead of default 
        ///     <see cref="IHateoasLinkBuilder.RouteName"/> value.
        /// </summary>
        /// <param name="presentedName">
        ///     Optional value to override <see cref="IHateoasLinkBuilder.RouteName"/> originally used to 
        ///     be presented on generated <see cref="HateoasLink.Rel"/>.
        /// </param>
        /// <returns>A <see cref="IHateoasLinkBuilder{T}" /> configuration generated with <paramref name="presentedName" />.</returns>
        IHateoasLinkBuilder<T> PresentedAs(string presentedName);
    }
}
