using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
    public int x = 0;
    public int y = 0;
    public int dx = 0;
    public int dy = 0;

    public int skipFramesAtMove;
    private int framesCounter;

    protected bool SkipFrame
    {
        get
        {
            if (++framesCounter >= skipFramesAtMove)
            {
                framesCounter = 0;
                return false;
            }
            else
                return true;
        }
    }
}
