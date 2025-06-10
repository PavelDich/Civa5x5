using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour, IPointerClickHandler
{
    private void Start()
    {
        for (int y = 0; y < _gridHeight; y++)
        {
            Tiles.Add(new List<Tile>());
            for (int x = 0; x < _gridWidth; x++)
            {
                Tiles[y].Add(null);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(_grid.WorldToCell(eventData.position));
    }

    public void SetTile(Tile tile, Vector3Int position)
    {
        tile.transform.position = _grid.GetCellCenterWorld(_grid.WorldToCell(new Vector3Int(position.x, 0, position.y)));
        tile.Grid = this;
    }



    [SerializeField]
    private Tilemap _grid;
    [SerializeField]
    private List<List<Tile>> Tiles = new List<List<Tile>>();
    [SerializeField]
    private int _gridHeight;
    [SerializeField]
    private int _gridWidth;

    public UnityEvent<int, int> OnCellClickEvent = new UnityEvent<int, int>();
}
