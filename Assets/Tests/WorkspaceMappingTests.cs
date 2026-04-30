using NUnit.Framework;
using UnityEngine;

public class WorkspaceMappingTests
{
    [Test]
    public void MidInput_MapsToWorkspaceCentre()
    {
        Vector2 workspaceMin = new Vector2(-5f, -3f);
        Vector2 workspaceMax = new Vector2(5f, 3f);

        Vector2 position = RehabTestHelpers.MapToWorkspace(
            512f,
            512f,
            0f,
            1024f,
            workspaceMin,
            workspaceMax);

        Assert.AreEqual(0f, position.x, 0.01f);
        Assert.AreEqual(0f, position.y, 0.01f);
    }

    [Test]
    public void MinimumInput_MapsToWorkspaceMinimum()
    {
        Vector2 workspaceMin = new Vector2(-5f, -3f);
        Vector2 workspaceMax = new Vector2(5f, 3f);

        Vector2 position = RehabTestHelpers.MapToWorkspace(
            0f,
            0f,
            0f,
            1024f,
            workspaceMin,
            workspaceMax);

        Assert.AreEqual(-5f, position.x, 0.01f);
        Assert.AreEqual(-3f, position.y, 0.01f);
    }

    [Test]
    public void MaximumInput_MapsToWorkspaceMaximum()
    {
        Vector2 workspaceMin = new Vector2(-5f, -3f);
        Vector2 workspaceMax = new Vector2(5f, 3f);

        Vector2 position = RehabTestHelpers.MapToWorkspace(
            1024f,
            1024f,
            0f,
            1024f,
            workspaceMin,
            workspaceMax);

        Assert.AreEqual(5f, position.x, 0.01f);
        Assert.AreEqual(3f, position.y, 0.01f);
    }
}