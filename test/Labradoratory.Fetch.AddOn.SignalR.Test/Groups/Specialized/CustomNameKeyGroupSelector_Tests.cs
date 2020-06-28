using System;
using System.Linq;
using System.Threading;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized;
using Labradoratory.Fetch.Processors.DataPackages;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Groups.Specialized
{
    public class CustomNameKeyGroupSelector_Tests
    {
        [Fact]
        public async void GetGroupAsync_Success()
        {
            var expectedKey = "MyKey9876";
            var expectedName = "MyGroupName";
            var expectedGroup = SignalrGroup.Create(expectedName, expectedKey);

            var subject = new CustomNameKeyGroupSelector<TestEntity>(expectedName);
            var package = new EntityAddedPackage<TestEntity>(new TestEntity(expectedKey));

            var groups = await subject.GetGroupAsync(package, CancellationToken.None);
            Assert.Single(groups);
            Assert.Equal(expectedGroup, groups.First());
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
                return Entity.ToKeys(_key);
            }
        }
    }
}
