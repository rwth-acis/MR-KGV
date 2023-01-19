using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour {
    private GameObject optionsScreen;
    private GameObject optionsButton;
    private Button optionsBackButton;

    void Start() {
        // Find canvas
        Canvas mainScreen = GameObject.Find("MainScreen").GetComponent<Canvas>();

        // Find buttons and screens in the scene
        optionsScreen = mainScreen.transform.Find("OptionsScreen").gameObject;
        optionsButton = mainScreen.transform.Find("OptionsButton").gameObject;
        optionsBackButton = GameObject.Find("OptionsBackButton").GetComponent<Button>();

        // Add listener for options back button
        optionsBackButton.onClick.AddListener(CloseOptionsScreen);
    }

    // Options back button logic
    public void CloseOptionsScreen() {
        optionsScreen.SetActive(false);
        optionsButton.SetActive(true);
    }
}
