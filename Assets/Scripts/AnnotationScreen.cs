using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationScreen : MonoBehaviour {
    public Node selectedNode;

    private GameObject annotationScreen;
    private GameObject optionsButton;
    private Button annotationBackButton;

    // Variables to read/write information from UI
    private TMPro.TextMeshProUGUI tmpLabel;
    private TMPro.TextMeshProUGUI tmpImageURL;
    private TMPro.TextMeshProUGUI tmpAnnotation;

    // Input fields
    private TMPro.TMP_InputField tmpInputImageURL;
    private TMPro.TMP_InputField tmpInputAnnotation;

    void Start() {
        // Find canvas
        Canvas mainScreen = GameObject.Find("MainScreen").GetComponent<Canvas>();

        // Find buttons and screens in the scene
        annotationScreen = mainScreen.transform.Find("AnnotationScreen").gameObject;
        optionsButton = mainScreen.transform.Find("OptionsButton").gameObject;
        annotationBackButton = GameObject.Find("AnnotationBackButton").GetComponent<Button>();

        // Find relevant text fields for node-specific texts
        tmpLabel = GameObject.Find("Label").GetComponent<TMPro.TextMeshProUGUI>();
        tmpImageURL = GameObject.Find("ImageURLText").GetComponent<TMPro.TextMeshProUGUI>();
        tmpAnnotation = GameObject.Find("AnnotationText").GetComponent<TMPro.TextMeshProUGUI>();

        // Find relevant input fields
        tmpInputImageURL = GameObject.Find("InputImageURL").GetComponent<TMPro.TMP_InputField>();
        tmpInputAnnotation = GameObject.Find("InputAnnotation").GetComponent<TMPro.TMP_InputField>();

        // Add listener for when the back button is clicked
        annotationBackButton.onClick.AddListener(CloseAnnotationScreen);
    }

    void Update() {
        // Update label (probably inefficient, but OnEnable updates too late)
        tmpLabel.text = selectedNode.GetComponent<TextMesh>().text;
    }

    void OnEnable() {
        tmpInputImageURL.text = selectedNode.imageURL;
        tmpInputAnnotation.text = selectedNode.annotation;
    }

    // Annotation back button logic
    public void CloseAnnotationScreen() {
        annotationScreen.SetActive(false);
        optionsButton.SetActive(true);
        
        selectedNode.UnselectedColor();

        // Write back annotation (could be changed by user)
        selectedNode.imageURL = tmpInputImageURL.text;
        selectedNode.annotation = tmpInputAnnotation.text;
    }
}
