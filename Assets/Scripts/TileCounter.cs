using TMPro;
using UnityEngine;

public class TileCounter : MonoBehaviour {
    [SerializeField] private TMP_Text _text;

    public void UpdateVisuals(int count, int gridSize) {
        _text.text = count.ToString();
        float desiredGlobalScale = (gridSize >= 6) ? 0.25f : 0.5f;
        if (count >= 100) desiredGlobalScale *= 0.75f;

        if (transform.parent != null) {
            Vector3 parentScale = transform.parent.lossyScale;

            float x = (parentScale.x != 0) ? desiredGlobalScale / parentScale.x : desiredGlobalScale;
            float y = (parentScale.y != 0) ? desiredGlobalScale / parentScale.y : desiredGlobalScale;
            float z = (parentScale.z != 0) ? desiredGlobalScale / parentScale.z : desiredGlobalScale;

            transform.localScale = new Vector3(x, y, z);
        }
        else {
            transform.localScale = Vector3.one * desiredGlobalScale;
        }
    }
}