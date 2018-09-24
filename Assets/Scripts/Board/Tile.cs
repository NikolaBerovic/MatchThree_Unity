using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {


    public int IndexX { get; private set; }
    public int IndexY { get; private set; }
    public int BreakableValue { get { return _breakableValue; } }
    public TileType TileType { get { return _tileType; } }

    [SerializeField] private TileType _tileType = TileType.Normal;
    [SerializeField] private int _breakableValue = 0; //value before tile can break
    [SerializeField] private Sprite[] _breakableSprites;
    [SerializeField] private Color _normalColor; 

    private Board _board;
    private SpriteRenderer _spriteRenderer;

	void Awake () 
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

    ///<summary>Initializes board</summary>
    public void Init(int x, int y, Board board)
	{
		IndexX = x;
		IndexY = y;
		_board = board;

		if (_tileType == TileType.Breakable)
		{
			if (_breakableSprites[_breakableValue] !=null)
			{
				_spriteRenderer.sprite = _breakableSprites[_breakableValue];
			}
		}
	}

	void OnMouseDown()
	{
		if (_board !=null)
		{
			_board.ClickTile(this);
		}

	}

	void OnMouseEnter()
	{
		if (_board !=null)
		{
			_board.DragToTile(this);
		}
	}

	void OnMouseUp()
	{
		if (_board !=null)
		{
			_board.ReleaseTile();
		}
	}

    ///<summary>Breaks tile in case of breakable type, starts BreakTileRoutine()</summary>
    public void BreakTile()
	{
		if (_tileType != TileType.Breakable)
		{
			return;
		}

		StartCoroutine(BreakTileRoutine());
	}

	IEnumerator BreakTileRoutine()
	{
		_breakableValue = Mathf.Clamp(_breakableValue--, 0, _breakableValue);

		yield return new WaitForSeconds(0.25f);

		if (_breakableSprites[_breakableValue] !=null)
		{
			_spriteRenderer.sprite = _breakableSprites[_breakableValue];
		}

		if (_breakableValue == 0)
		{
			_tileType = TileType.Normal;
			_spriteRenderer.color = _normalColor;

		}

	}

}
