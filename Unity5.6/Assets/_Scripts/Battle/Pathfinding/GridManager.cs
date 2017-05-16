using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using TrueSync;
public class GridManager
{
    private Dictionary<eGridType, Grid> allGrids = new Dictionary<eGridType, Grid>();
    public Grid GetGridByType(eGridType gType)
    {
        Grid outV = null;
        allGrids.TryGetValue(gType, out outV);
        return outV;
    }
}