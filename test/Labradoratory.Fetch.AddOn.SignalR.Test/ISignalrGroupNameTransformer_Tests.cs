using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test
{
    public class ISignalrGroupNameTransformer_Tests
    {
        [Fact]
        public async Task TransformIfPossible_TransformsValueWhenNotNull()
        {
            var originalValue = "MyValue";
            var modification = "Changed";
            var expectedValue = $"{originalValue}{modification}";

            var subjectMock = new Mock<ISignalrGroupNameTransformer>(MockBehavior.Strict);
            subjectMock
                .Setup(t => t.TransformAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync<string, CancellationToken, ISignalrGroupNameTransformer, string>((v, token) => v + modification);

            var result = await subjectMock.Object.TransformAsync(originalValue);
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public async Task TransformIfPossible_OriginalValueWhenNull()
        {
            var originalValue = "MyValue";
            var expectedValue = originalValue;

            var result = await ((ISignalrGroupNameTransformer)null).TransformIfPossibleAsync(originalValue);
            Assert.Equal(expectedValue, result);
        }
    }
}
