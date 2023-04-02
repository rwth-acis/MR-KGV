using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Threading.Tasks;
using i5.Toolkit.Core.Caching;
using i5.Toolkit.Core.ModelImporters;
using i5.Toolkit.Core.ServiceCore;

public class ModelRepresentation : MonoBehaviour {
    private bool firstEnable = false;

    void Start() {

    }

    void Update() {
        transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
    }

    private void OnEnable() {
        // Load 3d model from valid URL
        //if (IsValidURL(gameObject.GetComponentInParent<Node>().modelURL)) {
        //    ImportObject(gameObject.GetComponentInParent<Node>().modelURL);

        if (!firstEnable) {
            firstEnable = true;
        } else {
            // Only load 3d model if there is not one loaded already
            if (gameObject.transform.childCount == 0) {
                ImportObject(gameObject.GetComponentInParent<Node>().modelURL);
            }
        }
        
    }

    public async void ImportObject(string url) {
        GameObject obj = await ServiceManager.GetService<ObjImporter>().ImportAsync(url);

        obj.transform.SetParent(gameObject.transform);
        obj.transform.position = gameObject.transform.position;

        obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    // https://stackoverflow.com/questions/7578857/how-to-check-whether-a-string-is-a-valid-http-url
    bool IsValidURL(string URL) {
        string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return Rgx.IsMatch(URL);
    }

    // Delete old 3d model if a new one should be loaded
    public void DeleteChildren() {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

    }
}
