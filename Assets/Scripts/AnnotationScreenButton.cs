using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationScreenButton : MonoBehaviour {
    public GameObject panel;

    public Button button;

    public GameObject arButton;

    public Node selectedNode;

    // Variables to read/write information from UI
    private TMPro.TextMeshProUGUI tmpLabel;
    private TMPro.TextMeshProUGUI tmpImageURL;
    private TMPro.TextMeshProUGUI tmpAnnotation;

    // Start is called before the first frame update
    void Start() {
        // Add listener for when the back button is clicked
        button.onClick.AddListener(CloseOptionsMenu);

        // Find relevant text fields for node-specific texts
        tmpLabel = GameObject.Find("Label").GetComponent<TMPro.TextMeshProUGUI>();
        tmpImageURL = GameObject.Find("ImageURLText").GetComponent<TMPro.TextMeshProUGUI>();
        tmpAnnotation = GameObject.Find("AnnotationText").GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        // Update label (probably inefficient, but OnEnable updates too late)
        tmpLabel.text = selectedNode.GetComponent<TextMesh>().text;
    }

    private void OnEnable() {
        tmpImageURL.text = selectedNode.imageURL;
        tmpAnnotation.text = selectedNode.annotation;
    }

    public void CloseOptionsMenu() {
        panel.SetActive(false);
        arButton.SetActive(true);
        selectedNode.UnselectedColor();

        // Write back annotation (could be changed by user)
        selectedNode.imageURL = tmpImageURL.text;
        selectedNode.annotation = tmpAnnotation.text;
    }
}
