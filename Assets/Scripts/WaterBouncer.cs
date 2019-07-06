using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBouncer : IBounceChecker
{
    public bool CanBounce(CellType cellType)
    {
        if (cellType == CellType.Water)
            return true;
        return false;
    }
}
