using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour {
    private GameObject optionsScreen;
    private GameObject optionsButton;

    void Start() {
        // Find canvas
        Canvas mainScreen = GameObject.Find("MainScreen").GetComponent<Canvas>();

        // Find buttons and screens in the scene
        optionsScreen = mainScreen.transform.Find("OptionsScreen").gameObject;
        optionsButton = mainScreen.transform.Find("OptionsButton").gameObject;

        // Add listener for options button
        optionsButton.GetComponent<Button>().onClick.AddListener(OpenOptionsScreen);
    }

    // Options button logic
    public void OpenOptionsScreen() {
        optionsScreen.SetActive(true);
        optionsButton.SetActive(false);
    }
}
