using System;
using System.Linq;
using System.Threading;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
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
            var expectedPrefix1 = "prefix1";
            var entityGroup = SignalrGroup.Create(typeof(TestEntity).Name);
            var expectedPath1 = entityGroup.Prepend(expectedPrefix1);

            var subject = new EntityWithPrefixGroupSelector<TestEntity>(package => new[] { expectedPrefix1 });
            var package = new EntityAddedPackage<TestEntity>(new TestEntity("123456"));

            var groups = (await subject.GetGroupAsync(package, CancellationToken.None)).ToList();
            Assert.Single(groups);
            Assert.Equal(expectedPath1, groups[0]);
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
