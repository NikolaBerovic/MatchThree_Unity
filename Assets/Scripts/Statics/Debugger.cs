using System.Collections.Generic;
using UnityEngine;

public static class Debugger {

    ///<summary>Deactivates highlights for specified coords</summary>
    public static void HighlightTileOff(Tile[,] allTiles, int x, int y)
    {
        if (allTiles[x, y].TileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }
    }

    ///<summary>Activates highlights</summary>
    public static void HighlightTileOn(Tile[,] allTiles, int x, int y, Color col)
    {
        if (allTiles[x, y].TileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = col;
        }
    }

    ///<summary>Activates highlights for matches</summary>
    public static void HighlightMatches(GamePiece[,] allPieces, Tile[,] allTiles)
    {
        for (int i = 0; i < allPieces.GetLength(0); i++)
        {
            for (int j = 0; j < allPieces.GetLength(1); j++)
            {
                HighlightMatchesAt(allPieces, allTiles, i, j);

            }
        }
    }

    ///<summary>Activates highlights for matches for specified coords</summary>
    public static void HighlightMatchesAt(GamePiece[,] allPieces, Tile[,] allTiles, int x, int y)
    {
        HighlightTileOff(allTiles, x, y);
        var combinedMatches = MatchFinder.FindMatchesAt(allPieces, x, y);
        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                HighlightTileOn(allTiles, piece.IndexX, piece.IndexY, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    ///<summary>Activates highlights for listed pieces</summary>
    public static void HighlightPieces(Tile[,] allTiles, List<GamePiece> piecesList)
    {
        foreach (GamePiece piece in piecesList)
        {
            if (piece != null)
            {
                HighlightTileOn(allTiles, piece.IndexX, piece.IndexY, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }
}
