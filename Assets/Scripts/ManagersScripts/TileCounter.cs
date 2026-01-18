using TMPro;
using UnityEngine;

public class TileCounter : MonoBehaviour {
    [SerializeField] private TMP_Text _counterText;

    public void UpdateVisuals(int count, int gridSize) {
        _counterText.text = count.ToString();
        float desiredGlobalScale = (gridSize >= 6) ? 0.25f : 0.5f;
        if (count >= 100) desiredGlobalScale *= 0.75f;

        ApplyGlobalScale(desiredGlobalScale);
    }

    public void SetText(int guessed, int actual, int gridSize) {
        bool isMultiLine;
        _counterText.text = RevealRealCountLabel(guessed, actual, gridSize, out isMultiLine);
        float baseScale = (gridSize >= 6) ? 0.18f : 0.35f;

        if (isMultiLine) {
            baseScale *= 0.8f;
            _counterText.lineSpacing = 50f;
        }
        else {
            _counterText.lineSpacing = 0f;
        }

        ApplyGlobalScale(baseScale);
    }

    private void ApplyGlobalScale(float desiredGlobalScale) {
        if (transform.parent != null) {
            Vector3 parentScale = transform.parent.lossyScale;
            float x = parentScale.x != 0 ? desiredGlobalScale / parentScale.x : desiredGlobalScale;
            float y = parentScale.y != 0 ? desiredGlobalScale / parentScale.y : desiredGlobalScale;
            transform.localScale = new Vector3(x, y, 1f);
        }
        else {
            transform.localScale = Vector3.one * desiredGlobalScale;
        }
    }

    private string RevealRealCountLabel(int guessed, int actual, int gridSize, out bool isMultiLine) {
        isMultiLine = (gridSize >= 6) || (guessed >= 10) || (actual >= 10);

        if (guessed == 0 && actual > 0) {
            isMultiLine = false;
            return $"({actual})";
        }

        if (isMultiLine) {
            return $"{guessed}\n({actual})";
        }
        else {
            return $"{guessed} ({actual})";
        }
    }
}