using NUnit.Framework;

public class TaskCompletionTests
{
    [Test]
    public void AlignmentBelowThreshold_DoesNotCompleteTask()
    {
        bool complete = RehabTestHelpers.IsTaskComplete(0.85f, 0.90f);

        Assert.IsFalse(complete);
    }

    [Test]
    public void AlignmentAtThreshold_CompletesTask()
    {
        bool complete = RehabTestHelpers.IsTaskComplete(0.90f, 0.90f);

        Assert.IsTrue(complete);
    }

    [Test]
    public void AlignmentAboveThreshold_CompletesTask()
    {
        bool complete = RehabTestHelpers.IsTaskComplete(0.95f, 0.90f);

        Assert.IsTrue(complete);
    }
}