using System;
using System.Collections.Generic;
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
                    && (v.ImplementationInstance as CustomNameGroupSelector<TestEntity>).Name == expectedName)),
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
                    && (v.ImplementationInstance as CustomNameKeyGroupSelector<TestEntity>).Name == expectedName)),
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
            public Task<IEnumerable<string>> GetGroupAsync(BaseEntityDataPackage<TestEntity> dataPackage, CancellationToken cancellationToken = default)
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
