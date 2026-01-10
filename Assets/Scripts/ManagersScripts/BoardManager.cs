using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour {
    [SerializeField] private Tilemap _player0Tilemap;
    [SerializeField] private Tilemap _player1Tilemap;
    [SerializeField] private TileBase _tileBase;
    [SerializeField] private GridLabelGenerator _labelGenerator;

    private int _gridSize;
    private float _gridScale;
    private const int OneTilePixelSize = 64;
    private const int DefaultHeight = 10;

    public float GridScale => _gridScale;
    public int GridSize => _gridSize;

    private void Awake() {
        _gridSize = (int)PlayerPrefs.GetFloat("GridSlider", 3);
        _gridScale = (float)DefaultHeight / (OneTilePixelSize * _gridSize);

        GenerateTilemap(_player0Tilemap);
        GenerateTilemap(_player1Tilemap);

        _labelGenerator.setUpLabels(_gridSize, _gridScale, true);
    }

    public void UpdateVisibility(int activePlayer) {
        // Default logic: Player 0 can see Tilemap 1 and vice versa
        _player0Tilemap.gameObject.SetActive(activePlayer == 1);
        _player1Tilemap.gameObject.SetActive(activePlayer == 0);
    }

    public Tilemap GetActiveTilemap() { // Returns map where attack is currently display
        return _player0Tilemap.gameObject.activeSelf ? _player0Tilemap : _player1Tilemap;
    }

    private void GenerateTilemap(Tilemap map) {
        map.ClearAllTiles();
        for (int x = 0; x < _gridSize; x++) {
            for (int y = 0; y < _gridSize; y++) {
                map.SetTile(new Vector3Int(x, y, 0), _tileBase);
            }
        }
        map.transform.localScale = Vector3.one * _gridScale;
    }
}