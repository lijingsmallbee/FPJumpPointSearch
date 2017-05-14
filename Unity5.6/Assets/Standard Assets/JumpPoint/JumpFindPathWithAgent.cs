using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using TrueSync;
public class JumpFindPathWithGrid:FindPath
{  
    public void Initialize(Grid grid)
    {
        _grid = grid;
        openSet = new Heap<Node>(_grid.GridSize);
        openSetContainer = new HashSet<Node>();
        closedSet = new HashSet<Node>();
        jumpNodes = new List<Node>();
    }
}