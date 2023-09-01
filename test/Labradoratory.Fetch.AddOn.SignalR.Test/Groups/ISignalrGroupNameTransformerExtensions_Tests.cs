using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Groups
{
    public class ISignalrGroupNameTransformerExtensions_Tests
    {
        [Fact]
        public async Task TransformIfPossibleAsync_ReturnsOrginalIfNull()
        {
            var orignalName = SignalrGroup.Create("originalgroupname");
            ISignalrGroupTransformer nullTransformer = null;
            var result = await nullTransformer.TransformIfPossibleAsync(orignalName, CancellationToken.None);
            Assert.Equal(orignalName, result);
        }

        [Fact]
        public async Task TransformIfPossibleAsync_TransformIfNotNull()
        {
            var orignalName = SignalrGroup.Create("originalgroupname");
            var expectedResult = SignalrGroup.Create("I've been transformed!");

            var mockTransformer = new Mock<ISignalrGroupTransformer>(MockBehavior.Strict);
            mockTransformer.Setup(t => t.TransformAsync(It.IsAny<SignalrGroup>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

            var result = await mockTransformer.Object.TransformIfPossibleAsync(orignalName, CancellationToken.None);
            Assert.Equal(expectedResult, result);
        }
    }
}
