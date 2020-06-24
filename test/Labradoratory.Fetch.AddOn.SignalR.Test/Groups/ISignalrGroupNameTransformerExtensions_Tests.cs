using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Groups
{
    public class ISignalrGroupNameTransformerExtensions_Tests
    {
        [Fact]
        public async Task TransformIfPossibleAsync_ReturnsOrginalIfNull()
        {
            var orignalName = "originalgroupname";
            ISignalrGroupNameTransformer nullTransformer = null;
            var result = await nullTransformer.TransformIfPossibleAsync(orignalName, CancellationToken.None);
            Assert.Equal(orignalName, result);
        }

        [Fact]
        public async Task TransformIfPossibleAsync_TransformIfNotNull()
        {
            var orignalName = "originalgroupname";
            var expectedResult = "I've been transformed!";

            var mockTransformer = new Mock<ISignalrGroupNameTransformer>(MockBehavior.Strict);
            mockTransformer.Setup(t => t.TransformAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

            var result = await mockTransformer.Object.TransformIfPossibleAsync(orignalName, CancellationToken.None);
            Assert.Equal(expectedResult, result);
        }
    }
}
