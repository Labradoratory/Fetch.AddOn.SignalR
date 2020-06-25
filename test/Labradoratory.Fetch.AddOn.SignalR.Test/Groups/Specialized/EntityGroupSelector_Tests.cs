﻿using System;
using System.Linq;
using System.Threading;
using Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized;
using Labradoratory.Fetch.Processors.DataPackages;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Groups.Specialized
{
    public class EntityGroupSelector_Tests
    {
        [Fact]
        public async void GetGroupAsync_Success()
        {
            var expectedKey = "MyKey9876";

            var subject = new EntityGroupSelector<TestEntity>();
            var package = new EntityAddedPackage<TestEntity>(new TestEntity(expectedKey));

            var groups = await subject.GetGroupAsync(package, CancellationToken.None);
            Assert.Single(groups);
            Assert.Equal(typeof(TestEntity).Name.ToLower(), groups.First());
        }

        [Fact]
        public async void GetGroupAsync_WithPrefix_Success()
        {
            var expectedKey = "MyKey9876";
            var expectedPrefix = "prefix";

            var expectedPath = $"{expectedPrefix}/{typeof(TestEntity).Name.ToLower()}";

            var subject = new EntityGroupSelector<TestEntity>(package => expectedPrefix);
            var package = new EntityAddedPackage<TestEntity>(new TestEntity(expectedKey));

            var groups = await subject.GetGroupAsync(package, CancellationToken.None);
            Assert.Single(groups);
            Assert.Equal(expectedPath, groups.First());
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
