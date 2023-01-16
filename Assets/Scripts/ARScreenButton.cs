using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARScreenButton : MonoBehaviour {
    public GameObject panel;
    
    public Button button;

    public GameObject arButton;

    // Start is called before the first frame update
    void Start() {
        button.onClick.AddListener(OpenOptionsMenu);
    }

    // Update is called once per frame
    void Update() {

    }

    public void OpenOptionsMenu() {
        panel.SetActive(true);
        arButton.SetActive(false);
    }
}
