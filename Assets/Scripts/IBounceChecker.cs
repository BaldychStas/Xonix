using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBounceChecker
{
    bool CanBounce(CellType cellType);
}
