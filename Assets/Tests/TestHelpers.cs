using UnityEngine;

public static class RehabTestHelpers
{
    public static bool TryParseAngles(string line, out float armAngle, out float handAngle, out float wristAngle)
    {
        armAngle = 0f;
        handAngle = 0f;
        wristAngle = 0f;

        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        if (!line.StartsWith("T:"))
        {
            return false;
        }

        string[] parts = line.Substring(2).Split(',');

        if (parts.Length != 3)
        {
            return false;
        }

        bool parsedArm = float.TryParse(parts[0], out armAngle);
        bool parsedHand = float.TryParse(parts[1], out handAngle);
        bool parsedWrist = float.TryParse(parts[2], out wristAngle);

        return parsedArm && parsedHand && parsedWrist;
    }

    public static Vector2 MapToWorkspace(
        float xInput,
        float yInput,
        float inputMin,
        float inputMax,
        Vector2 workspaceMin,
        Vector2 workspaceMax)
    {
        float xNormalised = Mathf.InverseLerp(inputMin, inputMax, xInput);
        float yNormalised = Mathf.InverseLerp(inputMin, inputMax, yInput);

        float x = Mathf.Lerp(workspaceMin.x, workspaceMax.x, xNormalised);
        float y = Mathf.Lerp(workspaceMin.y, workspaceMax.y, yNormalised);

        return new Vector2(x, y);
    }

    public static bool IsTaskComplete(float alignment, float threshold)
    {
        return alignment >= threshold;
    }
}