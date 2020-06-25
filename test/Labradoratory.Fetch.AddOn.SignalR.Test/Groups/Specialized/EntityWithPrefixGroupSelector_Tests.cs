using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized;
using Labradoratory.Fetch.Processors.DataPackages;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Groups.Specialized
{
    public class EntityGroupWithPrefixSelector_Tests
    {
        [Fact]
        public async void GetGroupAsync_Success()
        {
            var expectedKey = "MyKey9876";
            var expectedPrefix1 = "prefix1";
            var expectedPrefix2 = "prefix2";

            var expectedPath1 = $"{expectedPrefix1}/{typeof(TestEntity).Name.ToLower()}";
            var expectedPath2 = $"{expectedPrefix2}/{typeof(TestEntity).Name.ToLower()}";

            var subject = new EntityWithPrefixGroupSelector<TestEntity>(package => expectedPrefix1, package => expectedPrefix2);
            var package = new EntityAddedPackage<TestEntity>(new TestEntity(expectedKey));

            var groups = (await subject.GetGroupAsync(package, CancellationToken.None)).ToList();
            Assert.Equal(2, groups.Count);
            Assert.Equal(expectedPath1, groups[0]);
            Assert.Equal(expectedPath2, groups[1]);
        }

        public class TestEntity : Entity
        {
            private readonly string _key;

            public TestEntity(string key)
            {
                _key = key;
            }

            public override object[] DecodeKeys(string encodedKeys)
            {
                throw new NotImplementedException();
            }

            public override string EncodeKeys()
            {
                return _key;
            }

            public override object[] GetKeys()
            {
                return ToKeys(_key);
            }
        }
    }
}
