using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationScreen : MonoBehaviour {
    public Node selectedNode;

    private GameObject annotationScreen;
    private GameObject optionsButton;
    private Button annotationBackButton;
    private TMPro.TMP_Dropdown changeRepresentationSingleDropdown;

    // Variables to read/write information from UI
    private TMPro.TextMeshProUGUI tmpLabel;
    private TMPro.TextMeshProUGUI tmpImageURL;
    private TMPro.TextMeshProUGUI tmpModelURL;
    private TMPro.TextMeshProUGUI tmpAnnotation;

    // Input fields
    private TMPro.TMP_InputField tmpInputImageURL;
    private TMPro.TMP_InputField tmpInputModelURL;
    private TMPro.TMP_InputField tmpInputAnnotation;

    void Start() {
        
    }

    // Initialization in Awake, since annotation screen is deactivated at the beginning (fixes bug for first selected node)
    void Awake() {
        // Find canvas
        Canvas mainScreen = GameObject.Find("MainScreen").GetComponent<Canvas>();

        // Find buttons and screens in the scene
        annotationScreen = mainScreen.transform.Find("AnnotationScreen").gameObject;
        optionsButton = mainScreen.transform.Find("OptionsButton").gameObject;
        annotationBackButton = GameObject.Find("AnnotationBackButton").GetComponent<Button>();
        changeRepresentationSingleDropdown = GameObject.Find("DropdownSingle").GetComponent<TMPro.TMP_Dropdown>();

        // Find relevant text fields for node-specific texts
        tmpLabel = GameObject.Find("Label").GetComponent<TMPro.TextMeshProUGUI>();
        tmpImageURL = GameObject.Find("ImageURLText").GetComponent<TMPro.TextMeshProUGUI>();
        tmpModelURL = GameObject.Find("ModelURLText").GetComponent<TMPro.TextMeshProUGUI>();
        tmpAnnotation = GameObject.Find("AnnotationText").GetComponent<TMPro.TextMeshProUGUI>();

        // Find relevant input fields
        tmpInputImageURL = GameObject.Find("InputImageURL").GetComponent<TMPro.TMP_InputField>();
        tmpInputModelURL = GameObject.Find("InputModelURL").GetComponent<TMPro.TMP_InputField>();
        tmpInputAnnotation = GameObject.Find("InputAnnotation").GetComponent<TMPro.TMP_InputField>();

        // Add listener for when the back button is clicked
        annotationBackButton.onClick.AddListener(CloseAnnotationScreen);
        changeRepresentationSingleDropdown.onValueChanged.AddListener(ChangeRepresentation);
    }

    void Update() {
        // Update label (probably inefficient, but OnEnable updates too late)
        tmpLabel.text = selectedNode.GetComponent<TextMesh>().text;
    }

    void OnEnable() {
        tmpInputImageURL.text = selectedNode.imageURL;
        tmpInputModelURL.text = selectedNode.modelURL;
        tmpInputAnnotation.text = selectedNode.annotation;
        changeRepresentationSingleDropdown.value = GameObject.Find("VisualizationHandler").GetComponent<Visualization>().CheckRepresentation(selectedNode.gameObject);
    }

    // Annotation back button logic
    public void CloseAnnotationScreen() {
        annotationScreen.SetActive(false);
        optionsButton.SetActive(true);
        
        selectedNode.UnselectedColor();

        // Write back annotation (could be changed by user)
        selectedNode.imageURL = tmpInputImageURL.text;
        selectedNode.modelURL = tmpInputModelURL.text;
        selectedNode.annotation = tmpInputAnnotation.text;
    }

    // Change single representation dropdown logic
    public void ChangeRepresentation(int value) {
        switch (value) {
            // Sphere
            case 0:
                //Debug.Log("Changed to Sphere Representation.");
                GameObject.Find("VisualizationHandler").GetComponent<Visualization>().ActivateSphereRepresentationSingle(selectedNode.gameObject);
                break;
            // Image
            case 1:
                //Debug.Log("Changed to Image Representation.");
                GameObject.Find("VisualizationHandler").GetComponent<Visualization>().ActivateImageRepresentationSingle(selectedNode.gameObject);
                break;
            // Model
            case 2:
                //Debug.Log("Changed to Image Representation.");
                GameObject.Find("VisualizationHandler").GetComponent<Visualization>().ActivateModelRepresentationSingle(selectedNode.gameObject);
                break;
            // Invalid
            default:
                break;
        }
    }
}
