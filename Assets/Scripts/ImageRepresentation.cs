using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class ImageRepresentation : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private Sprite imageSprite;
    private string imageURL;

    private bool firstEnable = true;

    void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // Resize image
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

        //Load image from Resources folder
        //spriteRenderer.sprite = Resources.Load<Sprite>("Images/HelloWorld");
    }

    private void OnEnable() {
        if (firstEnable) {
            firstEnable = false;
        } else {
            // Load image from valid URL
            if (IsValidURL(gameObject.GetComponentInParent<Node>().imageURL)) {
                //Debug.Log("Length: " + gameObject.GetComponentInParent<Node>().imageURL.Length);
                StartCoroutine(LoadImage(gameObject.GetComponentInParent<Node>().imageURL));
            }
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

    // https://stackoverflow.com/questions/7578857/how-to-check-whether-a-string-is-a-valid-http-url
    bool IsValidURL(string URL) {
        string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return Rgx.IsMatch(URL);
    }
}
