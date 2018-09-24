using UnityEngine;

///<summary>Class for spawning GamePieces, automaticlly added when Board class exists</summary>
public class Spawner : MonoBehaviour {

    private Board _board;

    private void Start()
    {
        _board = GetComponent<Board>();
        if (_board == null)
        { Debug.LogError("Error! No board script attached to " + gameObject.name); }
    }

    ///<summary>Spawns tile at location</summary>
    public Tile SpawnTile(GameObject prefab, int x, int y, int z = 0)
    {
        if (prefab != null )
        {
            GameObject tileGO = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
            Tile tile = tileGO.GetComponent<Tile>();

            if (tile == null)
            { Debug.LogError("Error! " + prefab.name + " is not Tile prefab!"); }

            tileGO.name = "Tile (" + x + "," + y + ")";
            tileGO.transform.parent = transform;

            _board.AllTiles[x, y] = tile;
            _board.AllTiles[x,y].Init(x, y, _board);

            return tile;
        }
        return null;
    }

    ///<summary>Spawns GamePiece at location</summary>
    public GamePiece SpawnGamePiece(GameObject prefab, int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (prefab != null)
        {
            GameObject pieceGO = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
            GamePiece piece = pieceGO.GetComponent<GamePiece>();

            if (piece == null)
            { Debug.LogError("Error! " + prefab.name + " is not GamePiece prefab!");}

            _board.PlaceGamePiece(piece, x, y);
            piece.Init(_board);

            if (falseYOffset != 0)
            {
                piece.transform.position = new Vector3(x, y + falseYOffset, 0);
                piece.Move(x, y, moveTime);
            }
            piece.transform.parent = transform;

            return piece;
        }
        return null;
    }

    ///<summary>Spawns bomb at location</summary>
    public Bomb SpawnBomb(GameObject prefab, int x, int y)
    {
        if (prefab != null)
        {
            GameObject bombGO = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
            Bomb bomb = bombGO.GetComponent<Bomb>();

            if (bomb == null)
            { Debug.LogError("Error! " + prefab.name + " is not Bomb prefab!"); }

            bomb.Init(_board);
            bomb.SetCoord(x, y);
            bomb.transform.parent = transform;

            return bomb;
        }
        return null;
    }
}
