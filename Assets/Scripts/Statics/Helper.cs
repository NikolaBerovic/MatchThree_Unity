using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers {

    ///<summary>Checks are coords within specified bounds</summary>
    public static bool IsWithinBounds(int boundX, int boundY, int x, int y)
    {
        return (x >= 0 && x < boundX && y >= 0 && y < boundY);
    }

    ///<summary>Checks are coords within specified 2D array</summary>
    public static bool IsWithinBounds(object[,] array2D, int x, int y)
    {
        return (x >= 0 && x < array2D.GetLength(0) && y >= 0 && y < array2D.GetLength(1));
    }

    ///<summary>Checks are tiles next to each other</summary>
    public static bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.IndexX - end.IndexX) == 1 && start.IndexY == end.IndexY)
        {
            return true;
        }

        if (Mathf.Abs(start.IndexY - end.IndexY) == 1 && start.IndexX == end.IndexX)
        {
            return true;
        }

        return false;
    }

    ///<summary>Check if list of matching GamePieces forms an L shaped match</summary>
    public static bool IsCornerMatch(List<GamePiece> gamePieces)
    {
        bool vertical = false;
        bool horizontal = false;
        int xStart = -1;
        int yStart = -1;

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (xStart == -1 || yStart == -1)
                {
                    xStart = piece.IndexX;
                    yStart = piece.IndexY;
                    continue;
                }

                if (piece.IndexX != xStart && piece.IndexY == yStart)
                {
                    horizontal = true;
                }

                if (piece.IndexX == xStart && piece.IndexY != yStart)
                {
                    vertical = true;
                }
            }
        }

        return (horizontal && vertical);
    }

    ///<summary>Checks have listed pieces reached destination</summary>
    public static bool HaveReachedDestination(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.IndexY > 0.001f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    ///<summary>Gets row pieces from 2D array</summary>
    public static List<GamePiece> GetRowPieces(GamePiece[,] allPieces, int row)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < allPieces.GetLength(0); i++)
        {
            if (allPieces[i, row] != null)
            {
                gamePieces.Add(allPieces[i, row]);
            }
        }
        return gamePieces;
    }

    ///<summary>Gets column pieces from 2D array</summary>
    public static List<GamePiece> GetColumnPieces(GamePiece[,] allPieces, int column)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < allPieces.GetLength(1); i++)
        {
            if (allPieces[column, i] != null)
            {
                gamePieces.Add(allPieces[column, i]);
            }
        }
        return gamePieces;
    }

    ///<summary>Gets column pieces index from list</summary>
    public static List<int> GetColumnsIndex(List<GamePiece> piecesList)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in piecesList)
        {
            if (piece != null)
            {
                if (!columns.Contains(piece.IndexX))
                {
                    columns.Add(piece.IndexX);
                }
            }
        }
        return columns;
    }

    ///<summary>Gets neighbour pieces from 2D array</summary>
    public static List<GamePiece> GetNeighbourPieces(GamePiece[,] allPieces, int x, int y, int offset = 1)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (Helpers.IsWithinBounds(allPieces, i, j))
                {
                    gamePieces.Add(allPieces[i, j]);
                }

            }
        }

        return gamePieces;
    }

    ///<summary>Gets random GameObject from 2D array</summary>
    public static GameObject GetRandomGameObject(GameObject[] objectArray)
    {
        int randomIdx = Random.Range(0, objectArray.Length);
        if (objectArray[randomIdx] == null)
        {
            Debug.LogWarning("ERROR:  BOARD.GetRandomObject at index " + randomIdx + "does not contain a valid GameObject!");
        }
        return objectArray[randomIdx];
    }
}
