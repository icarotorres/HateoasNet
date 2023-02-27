// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using FluentAssertions;
using HateoasNet.Extensions;
using Xunit;
using Xunit.Sdk;

namespace HateoasNet.Tests.Extensions
{
    public class DictionaryExtensionShould
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ToRouteDictionary_WithEnumerable_ReturnEmptyDictionary(int size)
        {
            var data = Enumerable.Range(0, size).ToArray();
            var dictionary = data.ToRouteDictionary();
            dictionary.Should().NotBeNull().And.BeEmpty();
        }

        [Theory]
        [StructValues]
        public void ToRouteDictionary_WithNonDateStructOrPrimitive_ReturnExpectedDictionary(object data)
        {
            var dictionary = data.ToRouteDictionary();
            dictionary.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void ToRouteDictionary_WithNonEnumerableRefType_ReturnExpectedDictionary()
        {
            var data = new HateoasLink("abc", "http://sample.com", HttpMethod.Get.Method);
            var expected = new Dictionary<string, object>
            {
                [nameof(HateoasLink.Rel)] = data.Rel,
                [nameof(HateoasLink.Href)] = data.Href,
                [nameof(HateoasLink.Method)] = data.Method
            };
            var dictionary = data.ToRouteDictionary();
            dictionary.Should().NotBeNull()
            .And.BeEquivalentTo(expected);
        }
    }

    public class StructValuesAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { int.MaxValue };
            yield return new object[] { double.MaxValue };
            yield return new object[] { Guid.NewGuid().ToString() };
            yield return new object[] { Guid.NewGuid() };
        }
    }
}
