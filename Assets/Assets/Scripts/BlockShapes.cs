using UnityEngine;
using System.Collections.Generic;

public class BlockShapes : MonoBehaviour
{
    // List of 2D arrays representing block shapes (1 = filled, 0 = empty)
    public List<int[,]> shapes = new List<int[,]>
    {
        // 1x1
        new int[,] { { 1 } },
        // 1x2
        new int[,] { { 1, 1 } },
        // 2x2
        new int[,] { { 1, 1 }, { 1, 1 } },
        // 3x3
        new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } },
        // L-Shape
        new int[,] { { 1, 0 }, { 1, 0 }, { 1, 1 } },
        // T-Shape
        new int[,] { { 1, 1, 1 }, { 0, 1, 0 } },
        // 1x3 Line
        new int[,] { { 1, 1, 1 } },
        // 1x4 Line
        new int[,] { { 1, 1, 1, 1 } },
        // S-Shape
        new int[,] { { 0, 1, 1 }, { 1, 1, 0 } }
    };
}