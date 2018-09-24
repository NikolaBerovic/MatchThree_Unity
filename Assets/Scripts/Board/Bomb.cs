using UnityEngine;

public class Bomb : GamePiece 
{
    public BombType BombType
    { get { return _bombType; } set { _bombType = value; } }

    [SerializeField] private BombType _bombType;
}
