using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class GameManager : MonoBehaviour
{
    [Range(0f,0.2f)]
    public float refreshRate;
    [Range(0.5f, 0.95f)]
    public float percentToNextLevel;
    public int health;
    public int width;
    public int height;
    public int playerSkipFramesPerMove;
    public int enemySkipFramesPerMove;

    [SerializeField]
    private InfoDisplayer displayer;
    private Player player;
    private CellType[][] grid;
    private int enemyCount;
    private int waterEnemiesCount;
    private int groundEnemiesCount;
    private List<Enemy> enemies;
    private List<Enemy> waterEnemies;
    private List<List<Point>> enemiesFields;
    private Renderer _renderer;
    private int borderSize = 3;
    private int round;
    private WaitForSeconds waits;
    private WaitForSeconds secod = new WaitForSeconds(1);
    private bool pause;
    private double timeInSeconds;
    private TimeSpan timeSpan;
    private string timeString;


    private bool canPlay
    {
        get { return player.health > 0; }
    }

    private void Start()
    {
        GetComponent<InputDetector>().onInput += InputDirection;
        waits = new WaitForSeconds(refreshRate);
        enemiesFields = new List<List<Point>>();
        _renderer = this.GetComponent<Renderer>();
        round = 0;
        player = new Player();
        player.health = health;
        displayer.ShowHp(player.health);
        GameDataInitialization();
        StartCoroutine(GameCoroutine());
        StartCoroutine(Timer());
    }

    private IEnumerator GameCoroutine()
    {
        while (true)
        {
            if (!pause)
                GameLoop();
            yield return waits;
        }
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            if (!pause)
            {
                ++timeInSeconds;
                timeSpan = TimeSpan.FromSeconds(timeInSeconds);
                timeString = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                            timeSpan.Hours,
                            timeSpan.Minutes,
                            timeSpan.Seconds,
                            timeSpan.Milliseconds);
                displayer.ShowTime(timeString);
            }
            yield return secod;
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        CheckInput();
    }
#endif

#if UNITY_EDITOR
    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { player.SetDirection(Direction.Left); };
        if (Input.GetKeyDown(KeyCode.RightArrow)) { player.SetDirection(Direction.Right); };
        if (Input.GetKeyDown(KeyCode.UpArrow)) { player.SetDirection(Direction.Up); };
        if (Input.GetKeyDown(KeyCode.DownArrow)) { player.SetDirection(Direction.Down); };
    }
#endif

    public void SetPause(bool pause)
    {
        this.pause = pause;
    }

    private void InputDirection(Direction direction)
    {
        player.SetDirection(direction);
    }

    private void GameDataInitialization()
    {
        ++round;
        player.ResetPosition();
        player.skipFramesAtMove = playerSkipFramesPerMove;
        SetEnemiesCount();
        CreateEnemies(waterEnemiesCount, groundEnemiesCount);
        CreateGrid();
        SetTexture();
        displayer.ShowRound(round);
    }

    private void SetEnemiesCount()
    {
        switch (round)
        {
            case 1:
                waterEnemiesCount = 1;
                groundEnemiesCount = 1;
                break;
            case 2:
                waterEnemiesCount = 2;
                groundEnemiesCount = 1;
                break;
            case 3:
                waterEnemiesCount = 4;
                groundEnemiesCount = 2;
                break;
            default:
                waterEnemiesCount = 5;
                groundEnemiesCount = 3;
                break;
        }
    }

    private void CreateGrid()
    {
        grid = new CellType[height][];
        for (int i = 0; i < height; i++)
        {
            grid[i] = new CellType[width];
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i < borderSize || j < borderSize || i >= height - borderSize || j >= width - borderSize)
                    grid[i][j] = CellType.Ground;
                else
                    grid[i][j] = CellType.Water;
            }
        }
    }

    private void CreateEnemies(int waterEnemiesCount, int groundEnemiesCount)
    {
        enemyCount = waterEnemiesCount + groundEnemiesCount;
        enemies = new List<Enemy>(enemyCount);
        waterEnemies = new List<Enemy>(waterEnemiesCount);                  // need them to fill grid 
        GroundBouncer groundBouncer = new GroundBouncer();                  //water enemies moves at water and bounce of ground and boarder
        WaterBouncer waterBouncer = new WaterBouncer();                     // ground enemies moves at ground and bounce of water
        for (int i = 0; i < enemyCount; i++)
        {
            if (i < waterEnemiesCount)
            {
                enemies.Add(new Enemy(groundBouncer));
                waterEnemies.Add(enemies[i]);
            }
            else
                enemies.Add(new Enemy(waterBouncer));
        }

        for (int i = 0; i < enemyCount; i++)
        {
            if (i < waterEnemiesCount)
            {
                enemies[i].x = UnityEngine.Random.Range(borderSize, width - borderSize);
                enemies[i].y = UnityEngine.Random.Range(borderSize, height - borderSize);
            }
            else
            {
                enemies[i].x = UnityEngine.Random.Range(width - borderSize + 1, width);
                enemies[i].y = UnityEngine.Random.Range(0, height);
            }
            enemies[i].dx = (UnityEngine.Random.Range(0, 2) == 0) ? -1 : 1;
            enemies[i].dy = (UnityEngine.Random.Range(0, 2) == 0) ? -1 : 1;
            enemies[i].skipFramesAtMove = enemySkipFramesPerMove;
        }
    }

    public void SetNewFieldAndKillEnemies()
    {
        enemiesFields = new List<List<Point>>();
        Dictionary<List<Point>, Enemy> dict = new Dictionary<List<Point>, Enemy>();
        for (int i = 0; i < waterEnemies.Count; i++)
        {
            enemiesFields.Add(new List<Point>());
            enemiesFields[i] = EnemyField(enemiesFields[i], waterEnemies[i].y, waterEnemies[i].x);
            dict.Add(enemiesFields[i], waterEnemies[i]);
        }

        if(!enemiesFields.All(field => field.Count == enemiesFields[0].Count))
        {
            int minSquare = enemiesFields.Min(field => field.Count);
            var smallestFields = enemiesFields.Where(field => field.Count == minSquare).ToList();
            for (int i = 0; i < smallestFields.Count; i++)
            {
                waterEnemies.Remove(dict[smallestFields[i]]);
                enemies.Remove(dict[smallestFields[i]]);
                for (int j = 0; j < smallestFields[i].Count; j++)
                {
                    grid[smallestFields[i][j].y][smallestFields[i][j].x] = CellType.Ground;
                }
                enemiesFields.Remove(smallestFields[i]);
            }
        }
        for (int i = 0; i < enemiesFields.Count; i++)
        {
            for (int j = 0; j < enemiesFields[i].Count; j++)
            {
                grid[enemiesFields[i][j].y][enemiesFields[i][j].x] = CellType.Enemy;
            }
        }
        RefreshBoard();
    }

    public List<Point> EnemyField(List<Point> points, int y, int x)
    {
        if (!points.Any(point => point.x == x && point.y == y))
        {
            if (grid[y][x] == CellType.Water)
            {
                points.Add(new Point(x, y));
            }
            if (grid[y + 1][x] == CellType.Water) EnemyField(points, y + 1, x);
            if (grid[y - 1][x] == CellType.Water) EnemyField(points, y - 1, x);
            if (grid[y][x - 1] == CellType.Water) EnemyField(points, y, x - 1);
            if (grid[y][x + 1] == CellType.Water) EnemyField(points, y, x + 1);
        }
        return points;
    }

    private void RefreshBoard()
    {
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                if (grid[i][j] == CellType.Enemy) grid[i][j] = CellType.Water;
                else grid[i][j] = CellType.Ground;
    }

    private void CheckBoards()
    {
        if (player.x < 0)
            player.x = 0;
        if (player.x > width - 1)
            player.x = width - 1;
        if (player.y < 0)
            player.y = 0;
        if (player.y > height - 1)
            player.y = height - 1;
    }

    private void DeclinePlayerWalls()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[i][j] == CellType.PlayerWall)
                    grid[i][j] = CellType.Water;
            }
        }
    }

    private void DamagePlayer()
    {
        player.GetDamage();
        player.ResetPosition();
        DeclinePlayerWalls();
        displayer.ShowHp(player.health);
        if (player.health <= 0)
            displayer.ShowDeathScreen();
    }

    private bool CanGoToNextLevel()
    {
        int size = (width - borderSize) * (height - borderSize);
        int ground = 0;

        for (int i = borderSize; i < height - borderSize; i++)
        {
            for (int j = borderSize; j < width - borderSize; j++)
            {
                if (grid[i][j] == CellType.Ground)
                    ++ground;
            }
        }
        float currentPercent = ((float)ground / (float)size);
        if (currentPercent >= percentToNextLevel)
            return true;
        return false;
    }

    private void GameLoop()
    {
        player.Move();
        CheckBoards();

        if (grid[player.y][player.x] == CellType.PlayerWall)
        {
            DamagePlayer();
            return;
        }

        if (grid[player.y][player.x] == 0)
            grid[player.y][player.x] = CellType.PlayerWall;


        for (int i = 0; i < enemies.Count; i++) enemies[i].Move(grid,height,width);

        if (grid[player.y][player.x] == CellType.Ground)
        {
            player.dx = player.dy = 0;

            SetNewFieldAndKillEnemies();
            if(CanGoToNextLevel())
            {
                GameDataInitialization();
                return;
            }
        }

        for (int i = 0; i < enemies.Count; i++)
            if (grid[enemies[i].y][enemies[i].x] == CellType.PlayerWall || (enemies[i].x == player.x && enemies[i].y == player.y))
            {
                DamagePlayer();
                return;
            }

        SetTexture();
    }

    public void SetTexture()
    {
        Texture2D t = new Texture2D(width, height);
        t.filterMode = FilterMode.Point;
        t.wrapMode = TextureWrapMode.Clamp;
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                t.SetPixel(j, i, GetColor(grid[i][j]));

        for (int i = 0; i < enemies.Count; i++)
        {
            t.SetPixel(enemies[i].x, enemies[i].y, GetColor(CellType.Enemy));
        }
        t.SetPixel(player.x, player.y, GetColor(CellType.PlayerWall));
        t.Apply();
        _renderer.sharedMaterial.SetTexture("_MainTex", t);
    }

    private Color GetColor(CellType val)
    {
        switch (val)
        {
            case CellType.Enemy:
                return Color.red;
            case CellType.Water:
                return Color.blue;
            case CellType.Ground:
                return Color.green;
            case CellType.PlayerWall:
                return Color.yellow;
        }
        return Color.black;
    }
}
