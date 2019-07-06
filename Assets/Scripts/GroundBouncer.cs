using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBouncer : IBounceChecker
{
    public bool CanBounce(CellType cellType)
    {
        if (cellType == CellType.Ground)
            return true;
        return false;
    }
}
