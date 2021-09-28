using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameState
{
    wait,
    move
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}
public class Board : MonoBehaviour
{
    [SerializeField] float scale = 1;
    public AgentMatch3 agent;
    public World world;
    public GameState currentState = GameState.move;
    public int width, height, offSet;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject[] dots;
    public GameObject[,] allDots;
    private PlayerStats playerStats;
    [SerializeField] TileType[] boardLayout;
    private bool[,] blankSpaces;
    [SerializeField] GameObject destroyEffect;
    private BackgroundTile[,] allTiles;
    private FindMatches findMatches;
    [SerializeField] ScoreManager ScoreManager;
    [SerializeField] Button restart;
    [SerializeField] SoundManager SoundManager;
    [SerializeField] GameObject[] trash;
    private GameObject piece;
    private int trashNumber;
    public float cTime;
    [SerializeField] Text timer;


    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        blankSpaces = new bool[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        SetUp();
        //ScoreManager.setLevelText();
    }

    void Update()
    {
        /* Timer
        cTime -= 1 * Time.deltaTime;
        timer.text = cTime.ToString("0") + "s";
        if (cTime < 0f)
        {
            restart.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        */
    }

    private void Awake()
    {
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStats>();
        if (world != null)
        {
            if(world.levels[playerStats.level] != null)
            {
                width = world.levels[playerStats.level].width;
                height = world.levels[playerStats.level].height;
                dots = world.levels[playerStats.level].dots;
                cTime = world.levels[playerStats.level].sTime;
            }
        }
    }
    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }
    public void SetUp()
    {
        GenerateBlankSpaces();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    GameObject BackgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                    BackgroundTile.transform.parent = this.transform;
                    BackgroundTile.name = "( " + i + ", " + j + " )";
                    int dotToUse = Random.Range(0, dots.Length);

                    int maxLoops = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxLoops < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxLoops++;
                    }
                    maxLoops = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.transform.localScale = new Vector3(scale, scale, scale);
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                }
            }
        }
    }


    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }

        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            agent.SetAgentReward(1);
            switch (findMatches.currentMatches.Count)
            {
                case 3:
                    ScoreManager.IncreaseScore(30);
                    break;
                case 4:
                    ScoreManager.IncreaseScore(20);
                    break;
                case 5:
                    ScoreManager.IncreaseScore(30);
                    break;
                case 6:
                    ScoreManager.IncreaseScore(40);
                    break;
                case 7:
                    ScoreManager.IncreaseScore(50);
                    break;
            }
            switch (allDots[column, row].tag)
            {
                case "Blue":
                    trashNumber = 0;
                    break;
                case "Brown":
                    trashNumber = 1;
                    break;
                case "Green":
                    trashNumber = 2;
                    break;
                case "Orange":
                    trashNumber = 3;
                    break;
                case "Pink":
                    trashNumber = 4;
                    break;
                case "Red":
                    trashNumber = 5;
                    break;
            }
            findMatches.currentMatches.Remove(allDots[column, row]);
            /*
            SoundManager.PlayDestroyNoise();
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 1f);
            piece = Instantiate(trash[trashNumber], allDots[column, row].transform.position, Quaternion.identity);
            piece.transform.localScale = new Vector3(0.05339105f/2, 0.05339105f / 2, 0.0533915f / 2);
            */

            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }


    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo2());
    }

    private IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allDots[i, j] == null)
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (allDots[i, k] != null)
                        {
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());
    }
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    nullCount++;
                } else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                    piece.transform.SetParent(transform);
                    piece.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    if(allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard()){
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.2f);

        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }
        currentState = GameState.move;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Match3");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        allDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j] != null)
                {
                    if(i < width - 2)
                    {
                    if(allDots[i + 1, j] != null && allDots [i + 2, j] != null)
                    {
                        if(allDots[i + 1, j].tag == allDots[i, j].tag 
                            && allDots[i + 2, j].tag == allDots[i, j].tag)
                        {
                            return true;
                        }
                    }
                    }
                    if (j < height - 2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag
                                && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadlocked()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    if(i < width - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }  
        return true;
    }
    private IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(0.5f);
        List<GameObject> newBoard = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    int maxIterations = 0;

                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;
                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }
    }
}
