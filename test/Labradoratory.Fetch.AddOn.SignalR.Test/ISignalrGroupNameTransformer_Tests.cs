using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test
{
    public class ISignalrGroupNameTransformer_Tests
    {
        [Fact]
        public void TransformIfPossible_TransformsValueWhenNotNull()
        {
            var originalValue = "MyValue";
            var modification = "Changed";
            var expectedValue = $"{originalValue}{modification}";

            var subjectMock = new Mock<ISignalrGroupNameTransformer>(MockBehavior.Strict);
            subjectMock
                .Setup(t => t.Transform(It.IsAny<string>()))
                .Returns<string>(v => v + modification);

            var result = subjectMock.Object.Transform(originalValue);
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void TransformIfPossible_OriginalValueWhenNull()
        {
            var originalValue = "MyValue";
            var expectedValue = originalValue;

            var result = ((ISignalrGroupNameTransformer)null).TransformIfPossible(originalValue);
            Assert.Equal(expectedValue, originalValue);
        }
    }
}
