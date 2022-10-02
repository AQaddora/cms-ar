using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof (TextMeshProUGUI))]
public class OpenHyperlinks : MonoBehaviour, IPointerClickHandler {
    private TMP_Text pTextMeshPro;
    private void Awake () {
        pTextMeshPro = GetComponent<TMP_Text> ();
    }

    public void OnPointerClick (PointerEventData eventData) {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink (pTextMeshPro, Input.mousePosition, null);
        if (linkIndex != -1) { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

            // open the link id as a url, which is the metadata we added in the text field
            string link = linkInfo.GetLinkID ();
            //if (link.StartsWith ("profile://"))
            //    AppProfileScreenUI.Instance.ShowProfile (int.Parse (link.Replace("profile://","")));
            //else if (link.StartsWith ("store://"))
            //    AppStoreProfileScreenUI.Instance.ShowProfile (int.Parse (link.Replace ("store://", "")));
            //else
            Application.OpenURL (linkInfo.GetLinkID ());
        } else {
            transform.parent.SendMessage ("OnPointerClick", eventData);
        }
    }
}