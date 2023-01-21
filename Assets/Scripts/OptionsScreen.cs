using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour {
    private GameObject optionsScreen;
    private GameObject optionsButton;
    private Button optionsBackButton;
    private TMPro.TMP_Dropdown changeRepresentationDropdown;
    private Button loadGraph1Button;
    private Button loadGraph2Button;
    private Button fetchImagesButton;

    void Start() {
        // Find canvas
        Canvas mainScreen = GameObject.Find("MainScreen").GetComponent<Canvas>();

        // Find buttons and screens in the scene
        optionsScreen = mainScreen.transform.Find("OptionsScreen").gameObject;
        optionsButton = mainScreen.transform.Find("OptionsButton").gameObject;
        optionsBackButton = GameObject.Find("OptionsBackButton").GetComponent<Button>();
        changeRepresentationDropdown = GameObject.Find("Dropdown").GetComponent<TMPro.TMP_Dropdown>();
        loadGraph1Button = GameObject.Find("LoadGraph1Button").GetComponent<Button>();
        loadGraph2Button = GameObject.Find("LoadGraph2Button").GetComponent<Button>();
        fetchImagesButton = GameObject.Find("FetchImages").GetComponent<Button>();

        // Add listener for options back button
        optionsBackButton.onClick.AddListener(CloseOptionsScreen);
        changeRepresentationDropdown.onValueChanged.AddListener(ChangeRepresentation);
        loadGraph1Button.onClick.AddListener(LoadGraph1);
        loadGraph2Button.onClick.AddListener(LoadGraph2);
        fetchImagesButton.onClick.AddListener(FetchImages);
    }

    // Options back button logic
    public void CloseOptionsScreen() {
        optionsScreen.SetActive(false);
        optionsButton.SetActive(true);
    }

    // Change representation dropdown logic
    public void ChangeRepresentation(int value) {
        switch (value) {
            // Spheres
            case 0:
                //Debug.Log("Changed to Sphere Representation.");
                GameObject.Find("VisualizationHandler").GetComponent<Visualization>().ActivateSphereRepresentation();
                break;
            // Images
            case 1:
                //Debug.Log("Changed to Image Representation.");
                GameObject.Find("VisualizationHandler").GetComponent<Visualization>().ActivateImageRepresentation();
                break;
            // Invalid
            default:
                break;
        }
    }

    public void LoadGraph1() {
        GameObject.Find("VisualizationHandler").GetComponent<Visualization>().LoadGraph1FromFile();
    }

    public void LoadGraph2() {
        GameObject.Find("VisualizationHandler").GetComponent<Visualization>().LoadGraph2FromFile();
    }

    public void FetchImages() {
        GameObject.Find("VisualizationHandler").GetComponent<Visualization>().FetchImageURLsFromDic();
    }
}
