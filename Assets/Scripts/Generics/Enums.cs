///<summary>A MatchValue to determine if GamePiece it forms a match</summary>
public enum MatchValue
{
    Yellow,
    Blue,
    Green,
    Red,
    Purple,
    None
}

public enum TileType
{
    Normal,
    Obstacle,
    Breakable
}

public enum BombType
{
    None,
    Column,
    Row,
    Neighbour,
    Color
}

///<summary>Type of interpolation for ingame moving action</summary>
public enum InterpType
{
    Linear,
    EaseOut,
    EaseIn,
    SmoothStep,
    SmootherStep
}

