using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuButton : MonoBehaviour {
    public GameObject panel;

    public Button button;

    public Button annotationButton;

    public GameObject arButton;

    // Start is called before the first frame update
    void Start() {
        button.onClick.AddListener(CloseOptionsMenu);

        RectTransform optionsScreenTransform = gameObject.GetComponent<RectTransform>();

        //optionsScreenTransform.sizeDelta = new Vector2(optionsScreenTransform.sizeDelta.x, Screen.height / 2);
        //optionsScreenTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height / 2);

        //optionsScreenTransform.sizeDelta = new Vector2(optionsScreenTransform.sizeDelta.x, Screen.height / 2);

        //optionsScreenTransform.anchoredPosition = new Vector2(optionsScreenTransform.anchoredPosition.x, -Screen.height / 4);
    }

    // Update is called once per frame
    void Update() {

    }

    public void CloseOptionsMenu() {
        panel.SetActive(false);
        arButton.SetActive(true);
    }

    public void OnClickAnnotationButton() {
        Debug.Log("Ich hoffe das kann man lesen");
    }
}
