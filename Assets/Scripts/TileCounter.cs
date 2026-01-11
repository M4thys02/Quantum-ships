using TMPro;
using UnityEngine;

public class TileCounter : MonoBehaviour {
    [SerializeField] private TMP_Text _text;

    public void UpdateVisuals(int count, int gridSize) {
        _text.text = count.ToString();

        float scale = (gridSize >= 6) ? 0.25f : 0.5f;
        if (count >= 100) scale *= 0.75f;

        transform.localScale = Vector3.one * scale;
    }
}