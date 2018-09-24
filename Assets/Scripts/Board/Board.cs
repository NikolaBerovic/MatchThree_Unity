using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Spawner))]
public class Board : MonoBehaviour
{
    //2D ARRAYS - ON BOARD POSITIONS
    public GamePiece[,] AllPieces { get; private set; }
    public Tile[,] AllTiles { get; private set; }

    public float SwapTime { get { return _swapTime; } }
    public bool IsRefilling { get { return _isRefilling; } }

    //BOARD RELATED
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _borderSize;

    [SerializeField] int _minBonus = 4;

    //PREFABS
    [SerializeField] private GameObject _tileNormalPrefab;
    [SerializeField] private GameObject _tileObstaclePrefab;
    [SerializeField] private GameObject[] _gamePiecePrefabs;
    [SerializeField] private GameObject _neighbourBombPrefab;
    [SerializeField] private GameObject _columnBombPrefabs;
    [SerializeField] private GameObject _rowBombPrefabs;
    [SerializeField] private GameObject _colorBombPrefab;

    //LEVEL STARTING OBJECTS
    [SerializeField] private StartingObject[] startingTiles;
    [SerializeField] private StartingObject[] startingGamePieces;

    //INGAME ACTION RELATED
    [SerializeField] private int _fillOffsetPosition = 10;
    [SerializeField] private float _fillMoveTime = 0.5f;
    [SerializeField] private float _swapTime = 0.5f;

    private Spawner _spawner;

    //CONTROLLED BOMBS
    private GameObject _clickedTileBomb;
    private GameObject _targetTileBomb;
    private Tile _clickedTile;
    private Tile _targetTile;

    //CONTROL RELATED
    private bool _playerInputEnabled = true;
    private int _scoreMultiplier = 0;
    private bool _isRefilling = false;

    [System.Serializable]
    public class StartingObject
    {
        public GameObject prefab;
        public int x;
        public int y;
        public int z;
    }

    void Start()
    {
        _spawner = GetComponent<Spawner>();
        AllTiles = new Tile[_width, _height];
        AllPieces = new GamePiece[_width, _height];
    }

    ///<summary>Initializes board</summary>
    public void InitBoard()
    {
        SetupTiles();
        SetupGamePieces();
        SetupCamera();
        FillBoard(_fillOffsetPosition, _fillMoveTime);
    }

    ///<summary>Setups starting and default tiles</summary>
    void SetupTiles()
    {
        foreach (StartingObject sTile in startingTiles)
        {
            if (sTile != null)
            {
                _spawner.SpawnTile(sTile.prefab, sTile.x, sTile.y, sTile.z);
            }

        }

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (AllTiles[i, j] == null)
                {
                    _spawner.SpawnTile(_tileNormalPrefab, i, j);
                }
            }
        }
    }

    ///<summary>Setups starting and default game pieces</summary>
    void SetupGamePieces()
    {
        foreach (StartingObject sPiece in startingGamePieces)
        {
            if (sPiece != null)
            {
                _spawner.SpawnGamePiece(sPiece.prefab, sPiece.x, sPiece.y, _fillOffsetPosition, _fillMoveTime);
            }
        }
    }

    ///<summary>Setups camera acording to board width and height</summary>
    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(_width - 1) / 2f, (float)(_height - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)_height / 2f + (float)_borderSize;
        float horizontalSize = ((float)_width / 2f + (float)_borderSize) / aspectRatio;

        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;

    }

    ///<summary>Gets random game piece of game piece prefabs</summary>
    GameObject GetRandomGamePiece()
    {
        return Helpers.GetRandomGameObject(_gamePiecePrefabs);
    }

    ///<summary>Places game piece to new x and y positions</summary>
    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("Error! PlaceGamePiece()");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        if (Helpers.IsWithinBounds(AllPieces, x, y))
            { AllPieces[x, y] = gamePiece; }

        gamePiece.SetCoord(x, y);
    }

    ///<summary>Fills random game piece at specified position</summary>
    GamePiece FillRandomGamePieceAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (Helpers.IsWithinBounds(_width, _height, x, y))
        {
            GamePiece gp = _spawner.SpawnGamePiece(GetRandomGamePiece(), x, y, falseYOffset, moveTime);

            return gp;
        }
        return null;
    }

    ///<summary>Fills board, should be executed at the beginning and when matched game pieces break</summary>
    void FillBoard(int falseYOffset = 0, float moveTime = 0.1f)
    {
        int maxInterations = 100;
        int iterations = 0;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (AllPieces[i, j] == null && AllTiles[i, j].TileType != TileType.Obstacle)
                {
                    FillRandomGamePieceAt(i, j, falseYOffset, moveTime);
                    iterations = 0;

                    //ITERATES AS LONG IT HAS MATCH ON FILL
                    //IF IT STILL HAS MATCHES AFTER 100 ITERATIONS, BREAKS
                    while (HasMatchOnFill(i, j))
                    {
                        ClearPieceAt(i, j);
                        FillRandomGamePieceAt(i, j, falseYOffset, moveTime);

                        iterations++;

                        if (iterations >= maxInterations)
                        {  break; }
                    }
                }
            }
        }
    }

    ///<summary>Does it have match while filling</summary>
    bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        List<GamePiece> leftMatches = MatchFinder.FindMatches(AllPieces, x, y, new Vector2(-1, 0), minLength);
        List<GamePiece> downwardMatches = MatchFinder.FindMatches(AllPieces, x, y, new Vector2(0, -1), minLength);

        return (leftMatches.Count > 0 || downwardMatches.Count > 0);
    }

    public void ClickTile(Tile tile)
    {
        if (_clickedTile == null)
        {
            _clickedTile = tile;
        }
    }

    public void DragToTile(Tile tile)
    {
        if (_clickedTile != null && Helpers.IsNextTo(tile, _clickedTile))
        {
            _targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (_clickedTile != null && _targetTile != null)
        {
            SwitchTiles(_clickedTile, _targetTile);
        }

        _clickedTile = null;
        _targetTile = null;
    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
    }

    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        if (_playerInputEnabled && !GameManager.Instance.IsGameOver)
        {
            GamePiece clickedPiece = AllPieces[clickedTile.IndexX, clickedTile.IndexY];
            GamePiece targetPiece = AllPieces[targetTile.IndexX, targetTile.IndexY];

            if (targetPiece != null && clickedPiece != null)
            {
                clickedPiece.Move(targetTile.IndexX, targetTile.IndexY, _swapTime);
                targetPiece.Move(clickedTile.IndexX, clickedTile.IndexY, _swapTime);

                yield return new WaitForSeconds(_swapTime);

                List<GamePiece> clickedPieceMatches = new List<GamePiece>();
                List<GamePiece> targetPieceMatches = new List<GamePiece>();

                List<GamePiece> clickedPieceBombed = new List<GamePiece>();
                List<GamePiece> targetPieceBombed = new List<GamePiece>();

                if (clickedPiece.GetType() == typeof(Bomb))
                {
                    Bomb clickedBomb = (Bomb)clickedPiece;
                    if (clickedBomb.BombType == BombType.Color)
                    {
                        clickedBomb.MatchValue = targetPiece.MatchValue;
                    }                 
                    clickedPieceBombed = MatchFinder.FindBombedPieces(AllPieces, clickedBomb);
                }
                    
                else
                    clickedPieceMatches = MatchFinder.FindMatchesAt(AllPieces, clickedTile.IndexX, clickedTile.IndexY);

                if (targetPiece.GetType() == typeof(Bomb))
                {
                    Bomb targetBomb = (Bomb)clickedPiece;
                    if (targetBomb.BombType == BombType.Color)
                    {
                        targetBomb.MatchValue = clickedPiece.MatchValue;
                    }
                    clickedPieceBombed = MatchFinder.FindBombedPieces(AllPieces, targetBomb);
                }
                else
                    targetPieceMatches = MatchFinder.FindMatchesAt(AllPieces, targetTile.IndexX, targetTile.IndexY);

                if (targetPieceMatches.Count == 0 && clickedPieceMatches.Count == 0
                    && targetPieceBombed.Count == 0 && clickedPieceBombed.Count == 0)
                {
                    clickedPiece.Move(clickedTile.IndexX, clickedTile.IndexY, _swapTime);
                    targetPiece.Move(targetTile.IndexX, targetTile.IndexY, _swapTime);
                }
                else
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.UpdateMoves();
                    }

                    Vector2 swipeDirection = new Vector2(targetTile.IndexX - clickedTile.IndexX, targetTile.IndexY - clickedTile.IndexY);

                    _clickedTileBomb = DropBomb(clickedTile.IndexX, clickedTile.IndexY, swipeDirection, clickedPieceMatches);
                    _targetTileBomb = DropBomb(targetTile.IndexX, targetTile.IndexY, swipeDirection, targetPieceMatches);

                    ClearAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList()
                        .Union(targetPieceBombed).ToList().Union(clickedPieceBombed).ToList());
                }
            }
        }

    }

    void ClearBoard()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                ClearPieceAt(i, j);
            }
        }
    }

    void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = AllPieces[x, y];

        if (pieceToClear != null)
        {
            AllPieces[x, y] = null;
            Destroy(pieceToClear.gameObject);
        }
    }

    void ClearPieceAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                ClearPieceAt(piece.IndexX, piece.IndexY);
                ParticleManager.Instance.PlayClearFXAt(piece.IndexX, piece.IndexY);

                int bonus = (gamePieces.Count >= _minBonus) ? 20 : 0;
                piece.ScorePoints(_scoreMultiplier, bonus);

            }
        }
    }

    void BreakTileAt(int x, int y)
    {
        Tile tileToBreak = AllTiles[x, y];

        if (tileToBreak != null && tileToBreak.TileType == TileType.Breakable)
        {
            ParticleManager.Instance.PlayBreakFXAt(tileToBreak.BreakableValue, x, y, 0);
            tileToBreak.BreakTile();
        }
    }

    void BreakTileAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                BreakTileAt(piece.IndexX, piece.IndexY);
            }
        }
    }

    List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < _height - 1; i++)
        {
            if (AllPieces[column, i] == null && AllTiles[column, i].TileType != TileType.Obstacle)
            {
                for (int j = i + 1; j < _height; j++)
                {
                    if (AllPieces[column, j] != null)
                    {
                        AllPieces[column, j].Move(column, i, collapseTime * (j - i));
                        AllPieces[column, i] = AllPieces[column, j];
                        AllPieces[column, i].SetCoord(column, i);

                        if (!movingPieces.Contains(AllPieces[column, i]))
                        {
                            movingPieces.Add(AllPieces[column, i]);
                        }

                        AllPieces[column, j] = null;

                        break;
                    }
                }
            }
        }
        return movingPieces;
    }

    List<GamePiece> CollapseColumn(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        List<int> columnsToCollapse = Helpers.GetColumnsIndex(gamePieces);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;
    }

    List<GamePiece> CollapseColumn(List<int> columnsToCollapse)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }
        return movingPieces;
    }

    void ClearAndRefillBoard(List<GamePiece> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }

    IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
    {
        _playerInputEnabled = false;
        _isRefilling = true;

        List<GamePiece> matches = gamePieces;

        _scoreMultiplier = 0;
        do
        {
            _scoreMultiplier++;

            yield return StartCoroutine(ClearAndCollapseRoutine(matches));

            yield return null;

            yield return StartCoroutine(RefillRoutine());

            matches = MatchFinder.FindAllMatches(AllPieces);

            yield return new WaitForSeconds(0.2f);

        }
        while (matches.Count != 0);

        _playerInputEnabled = true;
        _isRefilling = false;
    }

    IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {

        List<GamePiece> movingPieces = new List<GamePiece>();
        List<GamePiece> matches = new List<GamePiece>();

        yield return new WaitForSeconds(0.2f);

        bool isFinished = false;
        while (!isFinished)
        {

            List<int> columnsToCollapse = Helpers.GetColumnsIndex(gamePieces);
            ClearPieceAt(gamePieces);
            BreakTileAt(gamePieces);

            if (_clickedTileBomb != null)
            {
                ActivateBomb(_clickedTileBomb);
                _clickedTileBomb = null;
            }

            if (_targetTileBomb != null)
            {
                ActivateBomb(_targetTileBomb);
                _targetTileBomb = null;

            }

            yield return new WaitForSeconds(0.25f);

            movingPieces = CollapseColumn(columnsToCollapse);

            while (!Helpers.HaveReachedDestination(movingPieces))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);

            matches = MatchFinder.FindMatchesAt(AllPieces, movingPieces);

            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                _scoreMultiplier++;

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBonusSound();
                }

                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
        yield return null;
    }

    IEnumerator RefillRoutine()
    {
        FillBoard(_fillOffsetPosition, _fillMoveTime);

        yield return null;

    }

    GameObject DropBomb(int x, int y, Vector2 swapDirection, List<GamePiece> gamePieces)
    {

        GameObject bomb = null;
        MatchValue matchValue = MatchValue.None;

        if (gamePieces != null)
        {
            matchValue = MatchFinder.FindMatchValue(gamePieces);
        }

        if (gamePieces.Count >= 5 && matchValue != MatchValue.None)
        {
            if (Helpers.IsCornerMatch(gamePieces))
            {
                bomb = _spawner.SpawnBomb(_neighbourBombPrefab, x, y).gameObject;
            }
            else
            {
                if (_colorBombPrefab != null)
                {
                    bomb = _spawner.SpawnBomb(_colorBombPrefab, x, y).gameObject;

                }
            }
        }

        else if (gamePieces.Count == 4 && matchValue != MatchValue.None)
        {
            if (swapDirection.x != 0)
            {
                    bomb = _spawner.SpawnBomb(_rowBombPrefabs, x, y).gameObject;

            }
            else
            {
                bomb = _spawner.SpawnBomb(_columnBombPrefabs, x, y).gameObject;
            }
        }
        return bomb;
    }

    void ActivateBomb(GameObject bomb)
    {
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;


        if (Helpers.IsWithinBounds(AllPieces, x, y))
        {
            AllPieces[x, y] = bomb.GetComponent<GamePiece>();
        }
    }

}
