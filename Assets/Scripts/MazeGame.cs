using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class MazeGame : MonoBehaviour
{
    [Header("UI")]
    public RawImage mazeImage;
    public GameObject mazeCanvas;

    [Header("Result")]
    public GameObject studentCard;

    [Header("Screen Feedback")]
    public GameObject screenFlicker;
    
    [Header("Hint UI")]
    public TextMeshProUGUI baseHintText;
    // public TextMeshProUGUI ruleHintText;

    // ===== Maze settings =====
    const int TILE = 16;
    const int COLS = 20;
    const int ROWS = 20;

    Texture2D tex;

    // ===== Level system =====
    int level = 1;
    const int MAX_LEVEL = 3;

    int[,] mapLevel1;
    int[,] mapLevel2;
    int[,] mapLevel3;
    int[,] currentMap;
    bool showRuleHint = false;

    // ===== Player state =====
    Vector2Int player;
    int rule = 1;
    bool solved = false;

    // ===== Analytics =====
    float startTime;
    int moveCount = 0;
    string puzzleID = "maze";

    void Start()
    {
        tex = new Texture2D(COLS * TILE, ROWS * TILE);
        tex.filterMode = FilterMode.Point;
        mazeImage.texture = tex;
        // ruleHintText.gameObject.SetActive(false);

        GenerateLevels();
        LoadLevel(1);
    }

    void OnEnable()
    {
        startTime = Time.time;
        moveCount = 0;

        AnalyticsManager.LogEvent("puzzle_start",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID }
            });
    }

    void Update()
    {
        if (solved) return;

        // Exit
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            LogExit();
            ExitMaze();
            return;
        }

        // Rule switching
       bool ruleChanged = false;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            rule = 1;
            ruleChanged = true;
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame && level >= 2)
        {
            rule = 2;
            ruleChanged = true;
        }

        // if (Keyboard.current.digit3Key.wasPressedThisFrame && level >= 3)
        // {
        //     rule = 3;
        //     ruleChanged = true;
        // }

        int dx = 0, dy = 0;

        if (Keyboard.current.wKey.wasPressedThisFrame) dy = -1;
        if (Keyboard.current.sKey.wasPressedThisFrame) dy = 1;
        if (Keyboard.current.aKey.wasPressedThisFrame) dx = -1;
        if (Keyboard.current.dKey.wasPressedThisFrame) dx = 1;

//         if (Keyboard.current.tKey.wasPressedThisFrame)
// {
//             showRuleHint = !showRuleHint;
//             ruleHintText.gameObject.SetActive(showRuleHint);
//         }

        // level 3 reverse control
        if (level == 3 && rule == 1)
        {
            dx *= -1;
            dy *= -1;
        }

        if (dx != 0 || dy != 0)
        {
            moveCount++;
            TryMove(dx, dy);
            Draw();
        }
        else if (ruleChanged)
        {
            UpdateHint();  
            Draw();       
        }
        CheckGoal();
    }
    void CheckGoal()
    {
        int tile = currentMap[player.y, player.x];

        if (tile == 3 && rule == 1)
        {
            if (level < MAX_LEVEL)
            {
                LoadLevel(level + 1);
            }
            else
            {
                PuzzleSolved();
            }
        }
    }

    void LoadLevel(int lv)
    {
        level = lv;
        rule = 1;
        player = new Vector2Int(1, 1);

        if (level == 1) currentMap = mapLevel1;
        if (level == 2) currentMap = mapLevel2;
        if (level == 3) currentMap = mapLevel3;
        
        UpdateHint();
        Draw();
    }

    void TryMove(int dx, int dy)
    {
        int nx = player.x + dx;
        int ny = player.y + dy;

        if (nx < 0 || ny < 0 || nx >= COLS || ny >= ROWS)
            return;

        int tile = currentMap[ny, nx];

        if (tile == 1) return;
        if (tile == 2 && rule != 2) return;
        if (tile == 0 && rule == 2) return;

        player = new Vector2Int(nx, ny);

        // Exit only works in rule 1
        CheckGoal();
    }

    void PuzzleSolved()
    {
        if (solved) return;
        solved = true;

        float duration = Time.time - startTime;

        AnalyticsManager.LogEvent("puzzle_complete",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID },
                { "moves", moveCount },
                { "time_spent", duration },
                { "level", level }
            });

        MazeProgress.mazeSolved = true;

        mazeCanvas.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (studentCard != null)
            studentCard.SetActive(true);

        if (screenFlicker != null)
            screenFlicker.SetActive(false);
    }
    void UpdateHint()
    {
        // ===== Base Hint（随关卡变化）=====
        string genRule =
            "WASD to move\n" +
            "1 / 2 to shift state\n" +
            "Q to leave\n\n";
        if (level == 1)
        {
            baseHintText.text =
                genRule +
                "Level 1\n" +
                "The maze behaves as expected.\n" +
                "For now.";
        }
        else if (level == 2)
        {
            baseHintText.text =
            genRule +
                "Level 2\n" +
                "Not every path belongs to this world.\n" +
                "Sometimes, you must shift your view.";
        }
        else if (level == 3)
        {
            baseHintText.text =
            genRule +
                "Level 3\n" +
                "What feels right may be wrong.\n" +
                "And what is hidden may guide you.";
        }

        // ===== Rule Hint（随规则变化）=====
        // if (rule == 1)
        // {
        //     ruleHintText.text =
        //         "Rule 1 — Normal movement\n" +
        //         "Exit is visible.";
        // }
        // else if (rule == 2)
        // {
        //     ruleHintText.text =
        //         "Rule 2 — Purple mode\n" +
        //         "Can walk on purple tiles.";
        // }
        // else
        // {
        //     ruleHintText.text =
        //         "Rule 3 — Reversed control\n" +
        //         "Movement is inverted.";
        // }
    }
    void LogExit()
    {
        float duration = Time.time - startTime;

        AnalyticsManager.LogEvent("puzzle_exit",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID },
                { "moves", moveCount },
                { "time_spent", duration },
                { "level", level }
            });
    }

    void ExitMaze()
    {
        mazeCanvas.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Draw()
    {
        for (int y = 0; y < ROWS; y++)
            for (int x = 0; x < COLS; x++)
                DrawTile(x, ROWS - 1 - y, GetTileColor(currentMap[y, x]));

        DrawRect(player.x, ROWS - 1 - player.y, ColorForPlayer());
        tex.Apply();
    }

    Color GetTileColor(int tile)
    {
        if (tile == 1) return new Color(0.35f, 0.35f, 0.35f);

        if (tile == 2 && rule == 2)
            return new Color(0.6f, 0f, 0.8f);

        // Exit visible ONLY in rule 1
        if (tile == 3 && rule == 1)
            return Color.green;

        return Color.black;
    }

    Color ColorForPlayer()
    {
        if (rule == 1) return Color.white;
        if (rule == 2) return Color.cyan;
        return Color.yellow;
    }

    void DrawTile(int x, int y, Color c)
    {
        for (int py = 0; py < TILE; py++)
            for (int px = 0; px < TILE; px++)
                tex.SetPixel(x * TILE + px, y * TILE + py, c);
    }

    void DrawRect(int x, int y, Color c)
    {
        for (int py = 3; py < TILE - 3; py++)
            for (int px = 3; px < TILE - 3; px++)
                tex.SetPixel(x * TILE + px, y * TILE + py, c);
    }

    void GenerateLevels()
    {
        // ===== Level 1 (simple) =====
        mapLevel1 = new int[20, 20];

        for (int y = 0; y < ROWS; y++)
            for (int x = 0; x < COLS; x++)
                mapLevel1[y, x] = 1;

        for (int x = 1; x < COLS - 1; x++)
            mapLevel1[1, x] = 0;

        for (int y = 1; y < ROWS - 1; y++)
            mapLevel1[y, COLS - 2] = 0;

        mapLevel1[ROWS - 2, COLS - 2] = 3;

        // ===== Level 2 =====
        mapLevel2 = new int[20, 20];

        for (int y = 0; y < ROWS; y++)
            for (int x = 0; x < COLS; x++)
                mapLevel2[y, x] = 1;

        for (int x = 1; x < COLS - 1; x++)
            mapLevel2[1, x] = 0;

        for (int y = 1; y < ROWS - 1; y++)
            mapLevel2[y, COLS - 3] = 2;

        for (int x = 2; x < COLS - 2; x++)
            mapLevel2[ROWS - 3, x] = 0;

        mapLevel2[ROWS - 2, COLS - 2] = 3;

        // ===== Level 3 (original map) =====
        mapLevel3 = new int[20, 20]
        {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,1,2,2,2,1,0,0,0,2,2,2,0,0,0,0,1},
            {1,0,1,0,1,2,1,2,1,0,1,0,1,1,2,1,1,0,1,1},
            {1,0,1,0,0,2,1,2,0,0,1,0,0,0,2,0,1,0,0,1},
            {1,0,1,1,1,1,1,2,1,1,1,1,1,0,2,1,1,1,0,1},
            {1,0,0,0,0,0,0,2,0,0,0,0,1,0,2,2,2,0,0,1},
            {1,1,1,1,1,1,0,1,1,1,1,0,1,1,1,1,2,1,0,1},
            {1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,2,0,0,1},
            {1,0,1,1,1,1,1,0,1,0,1,1,1,1,0,1,2,1,1,1},
            {1,0,0,0,0,0,1,0,0,0,2,2,2,1,0,0,2,2,2,1},
            {1,1,1,1,1,0,1,1,1,1,1,1,2,1,1,1,1,1,2,1},
            {1,0,0,0,1,0,2,2,2,0,0,1,2,0,0,0,0,1,2,1},
            {1,0,1,0,1,1,1,1,2,1,0,1,1,1,1,1,0,1,2,1},
            {1,0,1,0,0,0,0,0,2,1,0,0,0,0,1,0,0,1,2,1},
            {1,0,0,0,1,1,1,1,2,1,1,1,1,0,1,1,1,1,2,1},
            {1,1,1,0,1,2,2,2,2,0,0,0,1,0,0,0,0,1,2,1},
            {1,0,0,0,1,2,1,1,1,1,1,0,1,1,1,1,0,1,2,1},
            {1,0,1,1,1,2,1,0,0,0,1,0,0,0,0,1,0,1,2,1},
            {1,0,0,0,0,2,2,2,1,0,2,2,2,2,0,0,0,0,3,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        };
    }
}