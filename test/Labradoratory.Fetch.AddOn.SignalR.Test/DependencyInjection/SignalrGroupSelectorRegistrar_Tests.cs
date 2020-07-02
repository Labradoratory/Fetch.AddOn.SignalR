using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.DependencyInjection;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.DependencyInjection
{
    public class SignalrGroupSelectorRegistrar_Tests
    {
        [Fact]
        public void UseEntityGroup_Success()
        {
            var serviceCollectionMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            serviceCollectionMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();

            var subject = new SignalrGroupSelectorRegistrar<TestEntity>(serviceCollectionMock.Object);
            subject.UseEntityGroup();

            serviceCollectionMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v =>
                    v.ServiceType == typeof(ISignalrGroupSelector<TestEntity>)
                    && v.ImplementationInstance is EntityGroupSelector<TestEntity>)),
                Times.Once);
        }

        [Fact]
        public void UseEntityGroupWithPrefix_Success()
        {
            var expectedPrefix = new Func<BaseEntityDataPackage<TestEntity>, object[]>(package => new[] { "blah" });

            var serviceCollectionMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            serviceCollectionMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();

            var subject = new SignalrGroupSelectorRegistrar<TestEntity>(serviceCollectionMock.Object);
            subject.UseEntityGroupWithPrefix(expectedPrefix);

            serviceCollectionMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v =>
                    v.ServiceType == typeof(ISignalrGroupSelector<TestEntity>)
                    && v.ImplementationInstance is EntityWithPrefixGroupSelector<TestEntity>
                    && (v.ImplementationInstance as EntityWithPrefixGroupSelector<TestEntity>).AddPrefix == expectedPrefix)),
                Times.Once);
        }

        [Fact]
        public void UseEntityGroupWithKeys_Success()
        {
            var serviceCollectionMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            serviceCollectionMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();

            var subject = new SignalrGroupSelectorRegistrar<TestEntity>(serviceCollectionMock.Object);
            subject.UseEntityGroupWithKeys();

            serviceCollectionMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v =>
                    v.ServiceType == typeof(ISignalrGroupSelector<TestEntity>)
                    && v.ImplementationInstance is EntityKeyGroupSelector<TestEntity>)),
                Times.Once);
        }

        [Fact]
        public void UseNamedGroup_Success()
        {
            var expectedName = "TheExpectedName";

            var serviceCollectionMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            serviceCollectionMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();

            var subject = new SignalrGroupSelectorRegistrar<TestEntity>(serviceCollectionMock.Object);
            subject.UseNamedGroup(expectedName);

            serviceCollectionMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v =>
                    v.ServiceType == typeof(ISignalrGroupSelector<TestEntity>)
                    && v.ImplementationInstance is CustomNameGroupSelector<TestEntity>
                    && Equals((v.ImplementationInstance as CustomNameGroupSelector<TestEntity>).NameParts[0], expectedName))),
                Times.Once);
        }

        [Fact]
        public void UseNamedGroupWithPrefix_Success()
        {
            var expectedName = "TheExpectedName";
            var expectedPrefix = new Func<BaseEntityDataPackage<TestEntity>, object[]>(package => new[] { "blah" });

            var serviceCollectionMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            serviceCollectionMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();

            var subject = new SignalrGroupSelectorRegistrar<TestEntity>(serviceCollectionMock.Object);
            subject.UseNamedGroupWithPrefix(expectedPrefix, expectedName);

            serviceCollectionMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v =>
                    v.ServiceType == typeof(ISignalrGroupSelector<TestEntity>)
                    && v.ImplementationInstance is CustomNameWithPrefixGroupSelector<TestEntity>
                    && Equals((v.ImplementationInstance as CustomNameWithPrefixGroupSelector<TestEntity>).NameParts[0], expectedName)
                    && (v.ImplementationInstance as CustomNameWithPrefixGroupSelector<TestEntity>).AddPrefix == expectedPrefix)),
                Times.Once);
        }

        [Fact]
        public void UseNamedGroupWithKeys_Success()
        {
            var expectedName = "TheExpectedName";

            var serviceCollectionMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            serviceCollectionMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();

            var subject = new SignalrGroupSelectorRegistrar<TestEntity>(serviceCollectionMock.Object);
            subject.UseNamedGroupWithKeys(expectedName);

            serviceCollectionMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v =>
                    v.ServiceType == typeof(ISignalrGroupSelector<TestEntity>)
                    && v.ImplementationInstance is CustomNameKeyGroupSelector<TestEntity>
                    && Equals((v.ImplementationInstance as CustomNameKeyGroupSelector<TestEntity>).NameParts[0], expectedName))),
                Times.Once);
        }

        [Fact]
        public void UseCustomSelector_Success()
        {
            var serviceCollectionMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            serviceCollectionMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();

            var subject = new SignalrGroupSelectorRegistrar<TestEntity>(serviceCollectionMock.Object);
            subject.UseCustomSelector<TestSelector>();

            serviceCollectionMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v =>
                    v.ServiceType == typeof(ISignalrGroupSelector<TestEntity>)
                    && v.ImplementationType == typeof(TestSelector))),
                Times.Once);
        }

        public class TestSelector : ISignalrGroupSelector<TestEntity>
        {
            public Task<IEnumerable<SignalrGroup>> GetGroupAsync(BaseEntityDataPackage<TestEntity> dataPackage, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }

        public class TestEntity : Entity
        {
            public override object[] DecodeKeys(string encodedKeys)
            {
                throw new NotImplementedException();
            }

            public override string EncodeKeys()
            {
                throw new NotImplementedException();
            }

            public override object[] GetKeys()
            {
                throw new NotImplementedException();
            }
        }
    }
}
