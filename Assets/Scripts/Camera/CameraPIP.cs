using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using RTS;
using System.Linq;

public class CameraPIP : MonoBehaviour
{
    public GUISkin mySkin;
    private Player player;
    private Camera cam;
    private Drone drone;

    //detect if objects in the camera---------------
    private GameObject[] people;
    private Plane[] firstCamPlanes;
    private Collider[] peopleColliders;
    Ray ray;
    RaycastHit hit;

    private List<GameObject> resizedObjects = new List<GameObject>();
    private Dictionary<GameObject, float> findObjectsMap = new Dictionary<GameObject, float>();
    private GameObject findObject;

    //detect if objects in the camera---------------
    void Start()
    {
        cam = this.GetComponent<Camera>();
        drone = this.transform.parent.gameObject.GetComponent<Drone>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        people = GameObject.FindGameObjectsWithTag("People");
        peopleColliders = new Collider[people.Length];
        for (int i = 0; i < people.Length; ++i)
        {
            peopleColliders[i] = people[i].GetComponent<Collider>();
        }

        findObject = new GameObject();

        //int k = 0;
        //while (k < firstCamPlanes.Length)
        //{
        //    GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //    p.name = "Plane " + k.ToString();
        //    p.transform.position = -firstCamPlanes[k].normal * firstCamPlanes[k].distance;
        //    p.transform.rotation = Quaternion.FromToRotation(Vector3.up, firstCamPlanes[k].normal);
        //    k++;
        //}
    }

    void Update()
    {
        //mouse hover on npc--------
        HoverMouseToResizePeople();
        if(IsMouseInFirstCamera())
        {
            Debug.Log("");
        }
        //mouse hover on npc--------

        if (cam.tag == "Camera_1st_view")
        {
            cam.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh = CameraExtention.GenerateFrustumMesh(cam);
        }
        CheckPeopleInCam();
        if (Input.GetMouseButton(0) && player.hud.MouseInBoundsPIP() && cam.depth == Drone.PIP_DEPTH_ACTIVE)
        {
            GameObject hitObject = FindHitObject();
            if (hitObject)
            {
                if (hitObject.name != "Ground")
                {
                    WorldObject worldObject = hitObject.GetComponent<WorldObject>();
                    if (worldObject)
                    {
                        worldObject.GetComponent<MapItem>().setColor(Color.green);
                        if (worldObject is NPC)
                        {
                            ((NPC)worldObject).Mark();

                        }
                        else if (worldObject is Vehicle)
                        {
                            ((Vehicle)worldObject).Mark();
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1) && player.hud.MouseInBoundsPIP() && cam.depth == Drone.PIP_DEPTH_ACTIVE
                 && player.getSelectedObjects().Count > 0 && player.getSelectedObjects()[0] == drone)
        {

            GameObject hitObject = FindHitObject();
            Vector3 hitPoint = FindHitPoint();

            if (player.getSelectedObjects().Count > 0)
            {
                foreach (WorldObject obj in player.getSelectedObjects())
                {
                    obj.MouseClick(hitObject, hitPoint, player);
                }
                this.player.audioManager.playUnitMoveToSound();
            }
        }
    }

    private void CheckPeopleInCam()
    {
        firstCamPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
        foreach (Collider collider in peopleColliders)
        {
            if (GeometryUtility.TestPlanesAABB(firstCamPlanes, collider.bounds))
            {
                for (int i = 0; i < people.Length; ++i)
                {
                    if (peopleColliders[i] == collider)
                    {
                        //  Debug.Log(i);
                    }
                }
                //        Debug.Log(collider.name + " has been detected!");
            }
        }
    }

    void OnGUI()
    {
        if (cam.depth != Drone.PIP_DEPTH_DEACTIVE)
        {
            GUI.skin = mySkin;
            GUI.Box(new Rect(cam.pixelRect.x, (Screen.height - cam.pixelRect.yMax), cam.pixelWidth, cam.pixelHeight), "");

            //draw drone icon on the top right of the camera
            if (cam.rect != ResourceManager.getInstance().getPIPCameraPosition())
            {
                if (GUI.Button(new Rect(cam.pixelRect.x + cam.pixelWidth - 20, (Screen.height - cam.pixelRect.yMax), 20, 20), "x"))
                {
                    cam.depth = Drone.PIP_DEPTH_DEACTIVE;
                }
            }

            //draw drone icon on the top right of the camera
            Color color = drone.color;
            if (drone.isSelected())
            {
                Texture droneTexture = player.hud.drone_2d;
                GUI.color = color;
                GUI.DrawTexture(new Rect(cam.pixelRect.x + cam.pixelWidth - 60, (Screen.height - cam.pixelRect.yMax), 40, 20), player.hud.drone_2d);
            }

            //double click PIP camera to select the cooresponding drone
            Event e = Event.current;
            if (e.isMouse && e.type == EventType.MouseDown && e.clickCount == 2 && this.MouseInBoundsPIP() && !IsMouseInFirstCamera())
            //cam.rect != ResourceManager.getInstance().getPIPCameraPosition()

            {
                if (player.getSelectedObjects().Count > 0)
                {
                    Drone selectedDrone = (Drone)player.getSelectedObjects()[0];
                    Camera dfcam = selectedDrone.getCameraFront();
                    ResourceManager.getInstance().setCameraPosition(dfcam, cam.rect);
                    //dfcam.rect = cam.rect;
                }
                player.setSelectedObject(drone);
                e.Use();
            }
        }
    }

    private GameObject FindHitObject()
    {
        LayerMask entitylayerMask = (1 << 11);   //Entity layer, npc and vehicle
        LayerMask groundlayerMask = (1 << 12);    //ground layer, ground
        Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20f, entitylayerMask))
        {
            return hit.collider.gameObject;
        }
        else if (Physics.Raycast(ray, out hit, 20f, groundlayerMask))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    private Vector3 FindHitPoint()
    {
        Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) return hit.point;
        return ResourceManager.InvalidPosition;
    }

    private bool MouseInBoundsPIP()
    {
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x >= cam.pixelRect.x && mousePos.x <= cam.pixelRect.x + cam.pixelWidth;
        bool insideHeight = mousePos.y >= cam.pixelRect.y && mousePos.y < cam.pixelRect.yMax;
        return insideWidth && insideHeight;
    }
    public bool GetMousInBoundsPIP()
    {
        return MouseInBoundsPIP();
    }

    private void HoverMouseToResizePeople()
    {
        //if (player.hud.MouseInBoundsPIP() && FindHitObject())
        if (IsMouseInFirstCamera() && FindHitObject())
        {
            float findOjbectTime = Time.timeSinceLevelLoad;
            if (FindHitObject().name == "Andy(Clone)")
            {
                
                findObject = FindHitObject();
                if (!findObjectsMap.ContainsKey(findObject))
                {
                    findObjectsMap.Add(findObject, findOjbectTime);
                    findObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                }
                else
                {
                    Debug.Log("Enter else statement");
                    findObjectsMap[findObject] = findOjbectTime;
                }
                //CapsuleCollider myCollider = findObject.GetComponentInChildren<CapsuleCollider>();
                //myCollider.radius = 3f;
                //myCollider.height = 16f;              
            }
            if (findObjectsMap.Count > 0)
            {
                //foreach (KeyValuePair<GameObject, float> tmpDic in findObjectsMap)
                //{
                //    if (findOjbectTime - tmpDic.Value > 3.0f)
                //    {
                //        tmpDic.Key.transform.localScale = new Vector3(0.4f,0.4f,0.4f);
                //        findObjectsMap.Clear();
                //        break;
                //    }
                //}
                foreach (var item in findObjectsMap.Where(kvp => kvp.Value < Time.timeSinceLevelLoad -3.0f).ToList())
                {
                    item.Key.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    findObjectsMap.Remove(item.Key);
                }
            }
        }
    }

    //public bool IsMouseInFirstCamera()
    //{
    //    Vector3 mousePos = Input.mousePosition;
    //    bool insideWidth = mousePos.x < ResourceManager.getInstance().getPIPCameraPosition().xMax && mousePos.x > ResourceManager.getInstance().getPIPCameraPosition().xMin;
    //    bool insideHeight = mousePos.y < ResourceManager.getInstance().getPIPCameraPosition().yMax && mousePos.y > ResourceManager.getInstance().getPIPCameraPosition().yMin;
    //    return insideWidth && insideHeight;
    //}
    public bool IsMouseInFirstCamera()
    {
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x < 1.0f && mousePos.x > 0.8f;
        bool insideHeight = mousePos.y < 0.33f && mousePos.y > 0;
        return insideWidth && insideHeight;
    }
}
