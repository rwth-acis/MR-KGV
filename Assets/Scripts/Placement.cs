using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Placement : MonoBehaviour {
    // Center point of the graph visualization; used in layout
    public GameObject centerPoint;

    // AR UI related
    public ARRaycastManager raycastManager;
    public ARPlaneManager arPlaneManager;
    public Camera arCamera;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool graphSet = false;

    void Start() {

    }

    void Update() {
        Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0) && !graphSet) {
            if (raycastManager.Raycast(ray, hits, TrackableType.Planes)) {
                Pose hitPose = hits[0].pose;

                // Change center point position based on user input
                centerPoint.transform.position = hitPose.position + new Vector3(0, 1.1f, 0);

                //centerPoint = Instantiate(new GameObject(), hitPose.position + new Vector3(0, 1, 0), hitPose.rotation);

                graphSet = true;
                DeactivatePlaneManager();

                GameObject.Find("VisualizationHandler").GetComponent<Visualization>().Initizalization();
            }
        }
    }

    public void ReactivatePlacement() {
        ActivatePlaneManager();
        graphSet = false;
    }


    private void DeactivatePlaneManager() {
        arPlaneManager.enabled = false;

        foreach (ARPlane plane in arPlaneManager.trackables) {
            plane.gameObject.SetActive(false);
        }
    }

    private void ActivatePlaneManager() {
        arPlaneManager.enabled = true;

        foreach (ARPlane plane in arPlaneManager.trackables) {
            plane.gameObject.SetActive(true);
        }
    }
}
