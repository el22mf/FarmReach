using NUnit.Framework;

public class MessageParsingTests
{
    [Test]
    public void ValidMessage_ReturnsExpectedAngles()
    {
        bool success = RehabTestHelpers.TryParseAngles("T:10,20,30", out float arm, out float hand, out float wrist);

        Assert.IsTrue(success);
        Assert.AreEqual(10f, arm);
        Assert.AreEqual(20f, hand);
        Assert.AreEqual(30f, wrist);
    }

    [Test]
    public void InvalidMessage_ReturnsFalse()
    {
        bool success = RehabTestHelpers.TryParseAngles("INVALID", out float arm, out float hand, out float wrist);

        Assert.IsFalse(success);
    }
}