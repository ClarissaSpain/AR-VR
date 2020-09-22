using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.ARFoundation;

public class ARPlaceObject : MonoBehaviour
{
    // Reference to object prefab
    [SerializeField]
    private List<GameObject> arPrefab; // creates a list of prefabs

    private GameObject[] arInstance;
    

    // Reference to an instance of the object that will be moved.
    //private List<GameObject> arInstance = new List<GameObject>(); // create a list of instances



    // Reference to Raycast Manager used to make raycasts to detect surfaces.
    private ARRaycastManager arRaycaster;

    // Flag if an object was placed or it should be moved
    private bool objectPlaced = false;

    private int i = 0;

    private bool isTouching = false;
    Touch touch;
    
    /// <summary>
    /// Unity method called in before first frame.
    /// </summary>
    void Start()
    {
        arInstance = new GameObject[arPrefab.Count];
        print(arPrefab.Count);
        // Get reference to AR Raycast Manager within this game object
        arRaycaster = GetComponent<ARRaycastManager>();
        // Debug.Log("Count of prefabs: " + arPrefab.Count);
        // Create instance of our object and hide it until it won't be placed

        for (int j = 0; j < arPrefab.Count; j++)
        {
            arInstance[j] = Instantiate(arPrefab[j]); // add to the instance list
            arInstance[j].gameObject.SetActive(false);
        }

        // Debug.Log("Instance: " + arInstance.Count);

        // 

    }

    /// <summary>
    /// Unity method called every frame.
    /// </summary>
    void Update()
    {
        // If instance is placed then skip update.
        if (objectPlaced)
            return;

        // Make a list of AR hits
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        // Center point of screen with 4 units of depth what will be used to make raycast
        var screenPoint = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 4);

        // Trying to find a sufrace in world.
        if (arRaycaster.Raycast(screenPoint, hits))
        {
            // If we did hit something then we should place the instance in that point in space.

            // Order hits to find closest one.
            hits.OrderBy(h => h.distance);
            var pose = hits[0].pose;

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                isTouching = true;

            }
            // Activate instance and move it to position on detected surface
            //  for (int i = 0; i < arInstance.Count; i++)
            //  {
            if (isTouching && i < 6)
            {
                arInstance[i].gameObject.SetActive(true);
                _PinchtoZoom(arInstance[i]); // changing the size of this object?
                arInstance[i].transform.position = new Vector3(touch.position.x, touch.position.y);

                // arInstance[i].transform.position = new Vector3(pose.position.x + i, pose.position.y);
                arInstance[i].transform.up = pose.up;
                i++;
                //i = i + 1;
                isTouching = false;
            }

            //  }

        }
        else
        {
            // If we didn't hit anything than we should hide instance
            //arInstance.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Method used to disable object movement, called by Place Button.
    /// </summary>
    public void PlacingFinished()
    {
        objectPlaced = true;
    }

    /// <summary>
    /// Method used to enable object movement, called by Move Button.
    /// </summary>
    public void PlacingBegin()
    {
        objectPlaced = false;
    }

    public void _PinchtoZoom(GameObject ARObject)
    {
        if (Input.touchCount == 2)
        { // Store both touches. 
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float pinchAmount = deltaMagnitudeDiff * 0.02f * Time.deltaTime;
            ARObject.transform.localScale += new Vector3(pinchAmount, pinchAmount, pinchAmount);
        }
    }

}
