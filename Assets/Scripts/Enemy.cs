using UnityEngine;

public class Enemy: Cell
{
    private IBounceChecker moveChecker;

    public Enemy(IBounceChecker moveChecker)
    {
        this.moveChecker = moveChecker;
    }

    public void Move(CellType[][] grid, int height, int width)
    {
        if (SkipFrame)
            return;
        x += dx;
        if (x < 0 || x >= width)
            FlipX();
        else
            if (moveChecker.CanBounce(grid[y][x]))
            {
                FlipX();
            }

        y += dy;
        if (y < 0 || y >= height)
            FlipY();
        else
            if (moveChecker.CanBounce(grid[y][x]))
            {
                FlipY();
            }

    }

    private void FlipX()
    {
        dx = -dx;
        x += dx;
    }

    private void FlipY()
    {
        dy = -dy;
        y += dy;
    }
}
