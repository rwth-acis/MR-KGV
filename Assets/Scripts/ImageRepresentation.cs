using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageRepresentation : MonoBehaviour {

    public SpriteRenderer spriteRenderer;

    private Sprite imageSprite;

    private string imageURL;

    // Start is called before the first frame update
    void Start() {
        //Load image from Resources folder
        //spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = Resources.Load<Sprite>("Images/HelloWorld");

        // Resize image
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnEnable() {
        // Load image from URL
        if (!(gameObject.GetComponentInParent<Node>().imageURL.Length == 0)) {
            Debug.Log("Length: " + gameObject.GetComponentInParent<Node>().imageURL.Length);
            StartCoroutine(LoadImage(gameObject.GetComponentInParent<Node>().imageURL));
        }
    }


    // Loads an image from a specified URL
    IEnumerator LoadImage(string url) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log("Connection Error");
        } else if (www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log("Protocol Error");
        } else {
            Texture2D imageTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            imageSprite = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = imageSprite;
        }
    }
}
