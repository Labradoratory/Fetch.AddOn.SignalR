using System;
using System.Collections.Generic;
using Labradoratory.Fetch.AddOn.SignalR.Extensions;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.AddOn.SignalR.Processors;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Extensions
{
    public class IServiceCollectionExtentions_Tests
    {
        [Fact]
        public void AddFetchSignalrProcessor_All_Succeed()
        {
            var subjectMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            subjectMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(IHubContext<TestHub>))))
                .Returns(Mock.Of<IHubContext<TestHub>>());
            serviceProviderMock
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(IEnumerable<ISignalrGroupSelector<TestEntity>>))))
                .Returns(new List<ISignalrGroupSelector<TestEntity>> { Mock.Of<ISignalrGroupSelector<TestEntity>>() });

            var subject = subjectMock.Object;
            var result = subject.AddFetchSignalrProcessor<TestEntity, TestHub>();

            Assert.Same(subject, result);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v =>
                    v.ServiceType == typeof(IProcessor<EntityAddedPackage<TestEntity>>)
                    && v.ImplementationType == typeof(SignalrOnAdded<TestEntity, TestHub>))),
                Times.Once);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityDeletedPackage<TestEntity>>)
                    && v.ImplementationType == typeof(SignalrOnDeleted<TestEntity, TestHub>))),
                Times.Once);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityUpdatedPackage<TestEntity>>)
                    && v.ImplementationType == typeof(SignalrOnUpdated<TestEntity, TestHub>))),
                Times.Once);
        }

        [Fact]
        public void AddFetchSignalrProcessor_Add_Succeed()
        {
            var subjectMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            subjectMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(IHubContext<TestHub>))))
                .Returns(Mock.Of<IHubContext<TestHub>>());
            serviceProviderMock
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(IEnumerable<ISignalrGroupSelector<TestEntity>>))))
                .Returns(new List<ISignalrGroupSelector<TestEntity>> { Mock.Of<ISignalrGroupSelector<TestEntity>>() });

            var subject = subjectMock.Object;
            var result = subject.AddFetchSignalrProcessor<TestEntity, TestHub>(SignalrProcessActions.Add);

            Assert.Same(subject, result);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v =>
                    v.ServiceType == typeof(IProcessor<EntityAddedPackage<TestEntity>>)
                    && v.ImplementationType == typeof(SignalrOnAdded<TestEntity, TestHub>))),
                Times.Once);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityDeletedPackage<TestEntity>>))),
                Times.Never);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityUpdatedPackage<TestEntity>>))),
                Times.Never);
        }

        [Fact]
        public void AddFetchSignalrProcessor_Delete_Succeed()
        {
            var subjectMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            subjectMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(IHubContext<TestHub>))))
                .Returns(Mock.Of<IHubContext<TestHub>>());
            serviceProviderMock
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(IEnumerable<ISignalrGroupSelector<TestEntity>>))))
                .Returns(new List<ISignalrGroupSelector<TestEntity>> { Mock.Of<ISignalrGroupSelector<TestEntity>>() });

            var subject = subjectMock.Object;
            var result = subject.AddFetchSignalrProcessor<TestEntity, TestHub>(SignalrProcessActions.Delete);

            Assert.Same(subject, result);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityAddedPackage<TestEntity>>))),
                Times.Never);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityDeletedPackage<TestEntity>>)
                    && v.ImplementationType == typeof(SignalrOnDeleted<TestEntity, TestHub>))),
                Times.Once);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityUpdatedPackage<TestEntity>>))),
                Times.Never);
        }

        [Fact]
        public void AddFetchSignalrProcessor_Update_Succeed()
        {
            var subjectMock = new Mock<IServiceCollection>(MockBehavior.Strict);
            subjectMock.Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(IHubContext<TestHub>))))
                .Returns(Mock.Of<IHubContext<TestHub>>());
            serviceProviderMock
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(IEnumerable<ISignalrGroupSelector<TestEntity>>))))
                .Returns(new List<ISignalrGroupSelector<TestEntity>> { Mock.Of<ISignalrGroupSelector<TestEntity>>() });

            var subject = subjectMock.Object;
            var result = subject.AddFetchSignalrProcessor<TestEntity, TestHub>(SignalrProcessActions.Update);

            Assert.Same(subject, result);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityAddedPackage<TestEntity>>))),
                Times.Never);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityDeletedPackage<TestEntity>>))),
                Times.Never);

            subjectMock.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<EntityUpdatedPackage<TestEntity>>)
                    && v.ImplementationType == typeof(SignalrOnUpdated<TestEntity, TestHub>))),
                Times.Once);
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

        public interface ITestHub
        { }

        public class TestHub : EntityHub<ITestHub>
        {
            public TestHub() : base(null) 
            { }
        }
    }
}
