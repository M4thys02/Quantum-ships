using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TMPLinkHandling : MonoBehaviour, IPointerClickHandler { // This script was AI generated
    [SerializeField] private TMP_Text text;

    private void Awake() {
        if (text == null) text = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        Camera cam = null;
        if (text.canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
            cam = eventData.pressEventCamera;
        }

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, cam);

        if (linkIndex != -1) {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();
            Application.OpenURL(linkID);
        }
    }
}