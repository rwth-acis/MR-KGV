using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRepresentation : MonoBehaviour {

    public SpriteRenderer imageRenderer;

    // Start is called before the first frame update
    void Start() {
        imageRenderer.sprite = Resources.Load<Sprite>("Assets/Sprites/HelloWorld.png");

        //Debug.Log(imageRenderer.size);
    }

    // Update is called once per frame
    void Update() {

    }
}
