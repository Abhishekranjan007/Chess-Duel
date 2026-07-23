
using UnityEngine;

public class BoardFormation : MonoBehaviour
{
    public static BoardFormation Instance { get; private set; }

    [SerializeField] private SpriteRenderer boardRenderer;
    [Header("Prefabs")]
    [SerializeField] private GameObject[] prefabs;    

    [SerializeField] private Sprite whiteBoardSprite;
    [SerializeField] private Sprite blackBoardSprite;


    private const int BOARD_WIDTH = 8;
    private const int BOARD_HEIGHT = 8;
    public GameObject tilePrefab, tilePrefabdt;
    public GameObject boardParent;

    private float distanceX = 1f;
    private float distanceY = 1f;
    private GameObject[,] tiles, tilesdt;
    private Vector3 bounds, bounds2;
    private ChessPiece[,] chessPieces;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    //Board Grid Generation
    public void GenerateBoard()
    {
        tiles = new GameObject[BOARD_WIDTH, BOARD_HEIGHT];
        bounds = new Vector2((BOARD_WIDTH / 2) * distanceX, (BOARD_HEIGHT / 2) * distanceY) /*+ boardCenter2*/;

        float offsetLeft = (-BOARD_WIDTH / 2f) * distanceX + distanceX / 2f;
        float offsetBottom = (-BOARD_HEIGHT / 2f) * distanceY + distanceY / 2f;

        Vector3 nextPosition = new Vector3(offsetLeft, offsetBottom, 1f);

        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            for (int y = 0; y < BOARD_HEIGHT; y++)
            {
                tiles[x, y] = GenerateSingleTile(nextPosition, x, y);
                nextPosition.y += distanceY;
            }
            nextPosition.y = offsetBottom;
            nextPosition.x += distanceX;
        }
    }

    private GameObject GenerateSingleTile(Vector3 nextPosition, int x, int y)
    {
        GameObject tile =
                   Instantiate(tilePrefab, transform, false);

        tile.transform.localPosition =
            nextPosition;

        ChessTile tileData = tile.AddComponent<ChessTile>();

        tileData.x = x;
        tileData.y = y;

        tile.name = $"Tile {x},{y}";
        return tile;
    }


    public void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[BOARD_WIDTH, BOARD_HEIGHT];
        int whiteTeam = 0;
        int blackTeam = 1;
        //newEmptyGameObject2 = new GameObject("Pieces");
        //newEmptyGameObject2.transform.position = Vector3.zero;

        float offsetLeft = (-BOARD_WIDTH / 2f) * distanceX + distanceX / 2f;
        float offsetBottom = (-BOARD_HEIGHT / 2f) * distanceY + distanceY / 2f;
        //Debug.Log("Spawn " + offsetLeft+"::"+ distanceX);
        Vector3 nextPosition = new Vector3(offsetLeft, offsetBottom, 1f);

        //White team
        chessPieces[0, 0] = SpawnOnePiece(1, nextPosition, whiteTeam, ChessPieceType.Rook);
        chessPieces[1, 0] = SpawnOnePiece(2, nextPosition, whiteTeam, ChessPieceType.Knight);
        chessPieces[2, 0] = SpawnOnePiece(3, nextPosition, whiteTeam, ChessPieceType.Bishop);
        chessPieces[3, 0] = SpawnOnePiece(4, nextPosition, whiteTeam, ChessPieceType.Queen);
        chessPieces[4, 0] = SpawnOnePiece(5, nextPosition, whiteTeam, ChessPieceType.King);
        chessPieces[5, 0] = SpawnOnePiece(3, nextPosition, whiteTeam, ChessPieceType.Bishop);
        chessPieces[6, 0] = SpawnOnePiece(2, nextPosition, whiteTeam, ChessPieceType.Knight);
        chessPieces[7, 0] = SpawnOnePiece(1, nextPosition, whiteTeam, ChessPieceType.Rook);

        for (int i = 0; i < BOARD_WIDTH; i++)
        {
            chessPieces[i, 1] = SpawnOnePiece(0, nextPosition, whiteTeam, ChessPieceType.Pawn);
        }


        //Black team
        chessPieces[0, 7] = SpawnOnePiece(7, nextPosition, blackTeam, ChessPieceType.Rook);
        chessPieces[1, 7] = SpawnOnePiece(8, nextPosition, blackTeam, ChessPieceType.Knight);
        chessPieces[2, 7] = SpawnOnePiece(9, nextPosition, blackTeam, ChessPieceType.Bishop);
        chessPieces[3, 7] = SpawnOnePiece(10, nextPosition, blackTeam, ChessPieceType.Queen);
        chessPieces[4, 7] = SpawnOnePiece(11, nextPosition, blackTeam, ChessPieceType.King);
        chessPieces[5, 7] = SpawnOnePiece(9, nextPosition, blackTeam, ChessPieceType.Bishop);
        chessPieces[6, 7] = SpawnOnePiece(8, nextPosition, blackTeam, ChessPieceType.Knight);
        chessPieces[7, 7] = SpawnOnePiece(7, nextPosition, blackTeam, ChessPieceType.Rook);


        for (int i = 0; i < BOARD_WIDTH; i++)
        {
            chessPieces[i, 6] = SpawnOnePiece(6, nextPosition, blackTeam, ChessPieceType.Pawn);
        }



    }

    //Single Chesspiece is spawned in this function
    public ChessPiece SpawnOnePiece(int prefabpos, Vector3 nextPosition, int team, ChessPieceType type)
    {
        GameObject spawnedPiece = Instantiate(prefabs[prefabpos], transform);
        ChessPiece cp = spawnedPiece.GetComponent<ChessPiece>();

        //cp.SetScale(new Vector2(0.7f, 0.7f));


        //if (player2 == RoomPhoton.room.GetID())
        //{
        //    if (type.Equals(ChessPieceType.Knight))
        //    {
        //        Utility.TurnChesKnightBlack(cp);
        //    }
        //    else
        //    {
        //        Utility.TurnChesPiecesBlack(cp);
        //    }
        //}


        cp.type = type;
        cp.team = team;

        NetworkGameManager manager = FusionRoomManager.Instance?.GetNetworkGameManager();

        if (manager != null && manager.AmIBlack())
        {
            cp.transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        cp.GetComponent<SpriteRenderer>().sortingOrder = 12;
        return cp;
    }

    public void PlaceAllPieces()
    {
        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            for (int y = 0; y < BOARD_HEIGHT; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    PlaceOnePiece(x, y, true);
                }
            }
        }
    }

    //Actual placing of single piece on board
    public void PlaceOnePiece(int x, int y, bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);        
    }

    public Vector3 GetTileCenter(int x, int y)
    {
        float offsetLeft =
            (-BOARD_WIDTH / 2f) * distanceX + distanceX / 2f;

        float offsetBottom =
            (-BOARD_HEIGHT / 2f) * distanceY + distanceY / 2f;

        return new Vector3(
            offsetLeft + x * distanceX,
            offsetBottom + y * distanceY,
            0
        );
    }


    //Generates grid which is handles highlight(selection, last moves etc)
    public void GenerateGriddt()
    {
        tilesdt = new GameObject[BOARD_WIDTH, BOARD_HEIGHT];
        bounds2 = new Vector2((BOARD_WIDTH / 2) * distanceX, (BOARD_HEIGHT / 2) * distanceX) /*+ boardCenter2*/;

        float offsetLeft = (-BOARD_WIDTH / 2f) * distanceX + distanceX / 2f;
        float offsetBottom = (-BOARD_HEIGHT / 2f) * distanceY + distanceY / 2f;
        Vector3 nextPosition = new Vector3(offsetLeft, offsetBottom, 1f);

        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            for (int y = 0; y < BOARD_HEIGHT; y++)
            {
                tilesdt[x, y] = GenerateSingleGriddt(nextPosition, x, y);
                nextPosition.y += distanceY;
            }
            nextPosition.y = offsetBottom;
            nextPosition.x += distanceX;
        }

    }

    //Single grid(single Cell) generation for highlight(selection, last moves etc)
    private GameObject GenerateSingleGriddt(Vector3 nextPosition, int x, int y)
    {
        GameObject spawnedTile;


        spawnedTile = Instantiate(tilePrefabdt, transform, false);

        spawnedTile.transform.localPosition =
            nextPosition;


        spawnedTile.name = $"Tiledt {x} {y}";

        return spawnedTile;
    }

    public void SetupBoardSprite()
    {
        if (GameManager.Instance.SelectedMode != ChessGameMode.Multiplayer)
        {
            boardRenderer.sprite = whiteBoardSprite;
            return;
        }

        NetworkGameManager manager = FusionRoomManager.Instance.GetNetworkGameManager();

        if (manager == null)
            return;

        if (manager.AmIWhite())
            boardRenderer.sprite = whiteBoardSprite;
        else
            boardRenderer.sprite = blackBoardSprite;
    }

    public ChessPiece[,] ChessPieces
    {
        get
        {
            return chessPieces;
        }
        
    }

    public GameObject[,] GetTiles()
    {
        return tiles;
    }

    public int GetBoardWidth()
    {
        return BOARD_WIDTH;
    }

    public int GetBoardHeight()
    {
        return BOARD_HEIGHT;
    }

    public float GetDistanceX()
    {
        return distanceX;
    }

    public float GetDistanceY()
    {
        return distanceY;
    }

    public void TurnChessPieces()
    {
        for (int i = 0; i < BOARD_WIDTH; i++)
        {
            for (int j = 0; j < BOARD_HEIGHT; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    chessPieces[i, j].transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
        }
    }

    public void ShowBoard()
    {
        boardParent.SetActive(true);
    }

    public void HideBoard()
    {
        boardParent?.SetActive(false);
    }

    public void SetBoardVisible(bool visible)
    {
        // Hide/show the board sprite if you have one
        if (boardRenderer != null)
            boardRenderer.enabled = visible;

        // Hide/show every sprite under BoardFormation
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
        {
            sr.enabled = visible;
        }
    }
}
