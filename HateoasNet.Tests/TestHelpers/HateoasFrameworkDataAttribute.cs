// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using Bogus;
using Bogus.Extensions.Brazil;
using Xunit.Sdk;

namespace HateoasNet.Tests.TestHelpers
{
    public class HateoasFrameworkDataAttribute : DataAttribute
    {
        private static readonly Faker s_faker = new();
        /// <inheritdoc />
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { HateoasTestData.Fake().Generate() };

            var values1 = new Dictionary<string, object>
            {
                ["Id"] = s_faker.Random.Guid(),
                ["cpf"] = s_faker.Person.Cpf(),
                ["date-of-birth"] = s_faker.Person.DateOfBirth,
            };
            yield return new object[] { HateoasTestData.Fake(values1).Generate() };

            var values2 = new Dictionary<string, object>
            {
                ["numberId"] = s_faker.Random.Int(),
                ["active"] = s_faker.Random.Bool(),
                ["name"] = s_faker.Person.UserName,
            };
            yield return new object[] { HateoasTestData.Fake(values2).Generate() };

            var values3 = new Dictionary<string, object>
            {
                ["productId"] = s_faker.Random.AlphaNumeric(24),
                ["price"] = s_faker.Random.Float(max: 300),
                ["product"] = s_faker.Commerce.ProductName(),
            };
            yield return new object[] { HateoasTestData.Fake(values3).Generate() };
        }
    }
}
