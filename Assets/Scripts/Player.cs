using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Cell
{
    public int health;

    public void GetDamage()
    {
        --health;
    }

    public void ResetPosition()
    {
        x = 0;
        y = 0;
        dx = 0;
        dy = 0;
    }

    public void Move()
    {
        if (SkipFrame)
            return;
        x += dx;
        y += dy;
    }

    public void SetDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                dx = 0;
                dy = 1;
                break;
            case Direction.Right:
                dx = 1;
                dy = 0;
                break;
            case Direction.Down:
                dx = 0;
                dy = -1;
                break;
            case Direction.Left:
                dx = -1;
                dy = 0;
                break;
        }
    }
}