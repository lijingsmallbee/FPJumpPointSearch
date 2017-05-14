using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using TrueSync;
public class GridManager
{
    private Dictionary<GridType, Grid> allGrids = new Dictionary<GridType, Grid>();
    public Grid GetGridByType(GridType gType)
    {
        Grid outV = null;
        allGrids.TryGetValue(gType, out outV);
        return outV;
    }
}