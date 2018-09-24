using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MatchFinder
{
    ///<summary>Finds matches for specified direction. Direction should have normalized value</summary>
    public static List<GamePiece> FindMatches(GamePiece[,] allPieces, int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;

        if (Helpers.IsWithinBounds(allPieces, startX, startY))
        {
            startPiece = allPieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return new List<GamePiece>();
        }

        int nextX;
        int nextY;

        int maxValue = (allPieces.GetLength(0) > allPieces.GetLength(1)) ? allPieces.GetLength(0) : allPieces.GetLength(1);

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!Helpers.IsWithinBounds(allPieces, nextX, nextY))
            {
                break;
            }

            GamePiece nextPiece = allPieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }

            else
            {
                if (nextPiece.MatchValue == startPiece.MatchValue && !matches.Contains(nextPiece) && nextPiece.MatchValue != MatchValue.None)
                {
                    matches.Add(nextPiece);
                }

                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }

        return new List<GamePiece>();
    }

    ///<summary>Finds vertical matches</summary>
    public static List<GamePiece> FindVerticalMatches(GamePiece[,] allPieces, int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(allPieces, startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = FindMatches(allPieces, startX, startY, new Vector2(0, -1), 2);

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : new List<GamePiece>();
    }

    ///<summary>Finds horizontal matches</summary>
    public static List<GamePiece> FindHorizontalMatches(GamePiece[,] allPieces, int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = FindMatches(allPieces, startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = FindMatches(allPieces, startX, startY, new Vector2(-1, 0), 2);

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : new List<GamePiece>();
    }

    ///<summary>Finds all possible matches for a specified piece coords</summary>
    public static List<GamePiece> FindMatchesAt(GamePiece[,] allPieces, int x, int y, int minLength = 3)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(allPieces, x, y, minLength);
        List<GamePiece> vertMatches = FindVerticalMatches(allPieces, x, y, minLength);

        var combinedMatches = horizMatches.Union(vertMatches).ToList();
        return combinedMatches;
    }

    ///<summary>Finds all possible matches for a specified pieces list</summary>
    public static List<GamePiece> FindMatchesAt(GamePiece[,] allPieces, List<GamePiece> piecesList, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        foreach (GamePiece piece in piecesList)
        {
            matches = matches.Union(FindMatchesAt(allPieces, piece.IndexX, piece.IndexY, minLength)).ToList();
        }
        return matches;
    }

    ///<summary>Finds all possible matches for 2D array</summary>
    public static List<GamePiece> FindAllMatches(GamePiece[,] allPieces)
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();

        for (int i = 0; i < allPieces.GetLength(0); i++)
        {
            for (int j = 0; j < allPieces.GetLength(1); j++)
            {
                var matches = FindMatchesAt(allPieces, i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }

    ///<summary>Finds the first valid MatchValue in pieces list</summary>
    public static MatchValue FindMatchValue(List<GamePiece> piecesList)
    {
        foreach (GamePiece piece in piecesList)
        {
            if (piece != null)
            {
                return piece.MatchValue;
            }
        }

        return MatchValue.None;
    }

    ///<summary>Finds all GamePieces in 2D array with a specific match value</summary>
    public static List<GamePiece> FindAllMatchValue(GamePiece[,] allPieces, MatchValue mValue)
    {
        List<GamePiece> foundPieces = new List<GamePiece>();

        for (int i = 0; i < allPieces.GetLength(0); i++)
        {
            for (int j = 0; j < allPieces.GetLength(1); j++)
            {
                if (allPieces[i, j] != null)
                {
                    if (allPieces[i, j].MatchValue == mValue)
                    {
                        foundPieces.Add(allPieces[i, j]);
                    }
                }
            }
        }
        return foundPieces;
    }

    ///<summary>Finds GamePieces in prefab array with a soecific match value</summary>
    public static GameObject FindGamePieceByMatchValue(GameObject[] gamePiecePrefabs, MatchValue matchValue)
    {
        if (matchValue == MatchValue.None)
        {
            return null;
        }

        foreach (GameObject go in gamePiecePrefabs)
        {
            GamePiece piece = go.GetComponent<GamePiece>();

            if (piece != null)
            {
                if (piece.MatchValue == matchValue)
                {
                    return go;
                }
            }
        }
        return null;
    }

    ///<summary>Finds all GamePieces from the list of GamePieces that should be destroyed by bomb</summary>
    public static List<GamePiece> FindBombedPieces(GamePiece[,] allPieces, Bomb bomb)
    {
        List<GamePiece> piecesToClear = new List<GamePiece>();

        switch (bomb.BombType)
        {
            case BombType.Column:
                piecesToClear = Helpers.GetColumnPieces(allPieces, bomb.IndexX);
                break;
            case BombType.Row:
                piecesToClear = Helpers.GetRowPieces(allPieces, bomb.IndexY);
                break;
            case BombType.Neighbour:
                piecesToClear = Helpers.GetNeighbourPieces(allPieces, bomb.IndexX, bomb.IndexY, 1);
                break;
            case BombType.Color:
                piecesToClear = FindAllMatchValue(allPieces, bomb.MatchValue);
                break;
        }

        return piecesToClear;
    }

}
