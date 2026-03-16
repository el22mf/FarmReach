using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResolutionItem
{
    public int width;
    public int height;

    public ResolutionItem(int newWidth, int newHeight)
    {
        width = newWidth;
        height = newHeight;
    }
}