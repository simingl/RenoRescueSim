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
    private Camera cam2nd;
    private Drone drone;

    //detect if objects in the camera---------------
    private GameObject[] people;
    private GameObject[] cars;
    private Plane[] firstCamPlanes;
    private Plane[] secondCamPlanes;
    private Collider[] peopleColliders;
    Ray ray;
    RaycastHit hit;

    private List<GameObject> resizedObjects = new List<GameObject>();
    private Dictionary<GameObject, float> findObjectsMap = new Dictionary<GameObject, float>();
    
    //detect if objects in the camera---------------

    //-------------------
    private Dictionary<GameObject,KeyValuePair<float,bool>> NPCShowTimeDic;
    private KeyValuePair<float, bool> NPCShowTimePair;
    //---------
    private float startCheckNPCinCamera = 0;
    private float checkFrequence = 0.5f;

    void Start()
    {
        cam = this.GetComponent<Camera>();
        cam2nd = this.GetComponent<Camera>();
        drone = this.transform.parent.gameObject.GetComponent<Drone>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        people = GameObject.FindGameObjectsWithTag("People");
        cars = GameObject.FindGameObjectsWithTag("Car");
        peopleColliders = new Collider[people.Length];
        NPCShowTimeDic = new Dictionary<GameObject, KeyValuePair<float, bool>>();
        NPCShowTimePair = new KeyValuePair<float, bool>();
        for (int i = 0; i < people.Length; ++i)
        {
            peopleColliders[i] = people[i].GetComponent<Collider>();
        }
    }

    void Update()
    {
        //mouse hover on npc--------
        HoverMouseToResizePeople();
        //mouse hover on npc--------
        IsNPCMarked();
        //if (cam.tag == "Camera_1st_view")
        if (cam.tag == "Camera_2nd_view")
        {
            cam.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh = CameraExtention.GenerateFrustumMesh(cam);
        }
        if(Time.time >= startCheckNPCinCamera)
        {
            CheckPeopleInCam();
            startCheckNPCinCamera += checkFrequence;
        }
       
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
                            NPCShowTimePair = new KeyValuePair<float, bool>(Time.time, true);
                            if (!NPCShowTimeDic.ContainsKey(worldObject.gameObject))
                            {
                                NPCShowTimeDic.Add(worldObject.gameObject, NPCShowTimePair);
                            }
                            else
                            {
                                NPCShowTimeDic[worldObject.gameObject] = NPCShowTimePair;
                            }

                        }
                        else if (worldObject is Vehicle)
                        {
                            ((Vehicle)worldObject).Mark();
                            NPCShowTimePair = new KeyValuePair<float, bool>(Time.time,true);
                            if (!NPCShowTimeDic.ContainsKey(worldObject.gameObject))
                            {
                                NPCShowTimeDic.Add(worldObject.gameObject, NPCShowTimePair);
                            }
                            else
                            {
                                NPCShowTimeDic[worldObject.gameObject] = NPCShowTimePair;
                            }
                        }
                    }
                }
            }
        }
        //click on firstCamera to add a destination
        //else if (Input.GetMouseButtonDown(1) && player.hud.MouseInBoundsPIP() && cam.depth == Drone.PIP_DEPTH_ACTIVE
        //         && player.getSelectedObjects().Count > 0 && player.getSelectedObjects()[0] == drone)
        //{

        //    GameObject hitObject = FindHitObject();
        //    Vector3 hitPoint = FindHitPoint();

        //    if (player.getSelectedObjects().Count > 0)
        //    {
        //        foreach (WorldObject obj in player.getSelectedObjects())
        //        {
        //            obj.MouseClick(hitObject, hitPoint, player);
        //        }
        //        this.player.audioManager.playUnitMoveToSound();
        //    }
        //}
    }

    private void CheckPeopleInCam()
    {
        firstCamPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
        secondCamPlanes = GeometryUtility.CalculateFrustumPlanes(cam2nd);

        foreach (Collider collider in peopleColliders)
        {
            if (GeometryUtility.TestPlanesAABB(firstCamPlanes, collider.bounds)/* || GeometryUtility.TestPlanesAABB(secondCamPlanes, collider.bounds)*/)
            {
                for (int i = 0; i < people.Length; ++i)
                {
                    if (peopleColliders[i] == collider)
                    {
                        if (!FindHitObjectBetweenObjects(collider.gameObject))
                        {
                            if (!NPCShowTimeDic.ContainsKey(collider.gameObject))
                            {
                                NPCShowTimePair = new KeyValuePair<float, bool>(Time.time, false);
                                NPCShowTimeDic.Add(collider.gameObject, NPCShowTimePair);
                            }
                            else
                            {
                                NPCShowTimePair = new KeyValuePair<float, bool>(Time.time, NPCShowTimeDic[collider.gameObject].Value);
                                NPCShowTimeDic[collider.gameObject] = NPCShowTimePair;
                            }
                        }
                    }
                }
            }
        }
    }

    private void IsNPCMarked()
    {
        //bool result = false;
        foreach(GameObject p in people)
        {
            if (NPCShowTimeDic.ContainsKey(p))
            {
                if (NPCShowTimeDic[p].Value)
                {
                    Debug.Log(p.name + "is marked");
                }
            }
        }

        foreach (GameObject c in cars )
        {
            if (NPCShowTimeDic.ContainsKey(c))
            {
                if (NPCShowTimeDic[c].Value)
                {
                    Debug.Log(c.name + "is marked");
                }
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
            //if (cam.rect != ResourceManager.getInstance().getPIPCameraPosition())
            //{
            //    if (GUI.Button(new Rect(cam.pixelRect.x + cam.pixelWidth - 20, (Screen.height - cam.pixelRect.yMax), 20, 20), "x"))
            //    {
            //        cam.depth = Drone.PIP_DEPTH_DEACTIVE;
            //    }
            //}

            //draw drone icon on the top right of the camera
            Color color = drone.color;
            //if (drone.isSelected()) //if drone is selected, draw drone icon
            //{
            //    Texture droneTexture = player.hud.drone_2d;
            //    GUI.color = color;
            //    GUI.DrawTexture(new Rect(cam.pixelRect.x + cam.pixelWidth - 60, (Screen.height - cam.pixelRect.yMax), 40, 20), player.hud.drone_2d);
            //}
            
            Texture droneTexture = player.hud.drone_2d;
            GUI.color = color;
            GUI.DrawTexture(new Rect(cam.pixelRect.x + cam.pixelWidth - 60, (Screen.height - cam.pixelRect.yMax), 40, 20), player.hud.drone_2d);
            

            //double click PIP camera to select the cooresponding drone
            Event e = Event.current;
            if (e.isMouse && e.type == EventType.MouseDown && e.clickCount == 2 && this.MouseInBoundsPIP() && !IsMouseInFirstCamera())
            //cam.rect != ResourceManager.getInstance().getPIPCameraPosition()

            {
                //if (player.getSelectedObjects().Count > 0)
                //{
                //    Drone selectedDrone = (Drone)player.getSelectedObjects()[0];
                //    Camera dfcam = selectedDrone.getCameraFront();
                //    ResourceManager.getInstance().setCameraPosition(dfcam, cam.rect);
                //    //dfcam.rect = cam.rect;
                //}
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

    private bool FindHitObjectBetweenObjects(GameObject obj)
    {
        LayerMask entitylayerMask = (1 << 11);   //Entity layer, npc and vehicle
        LayerMask groundlayerMask = (1 << 12);    //ground layer, ground
        //Ray ray = cam.ScreenPointToRay(obj.transform.position);
        //Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
        Ray ray = new Ray(obj.transform.position, cam.transform.position - obj.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, entitylayerMask))
        {
            if (hit.collider.gameObject == obj)
                return false;
        }
        else if (Physics.Raycast(ray, out hit, groundlayerMask))
        {
            if (hit.collider.gameObject == obj)
                return false;
        }
        return true;
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
        if (IsMouseInFirstCamera() && FindHitObject())
        {
            float findOjbectTime = Time.timeSinceLevelLoad;
            if (FindHitObject().name == "Andy(Clone)" || FindHitObject().name == "car-alpha(Clone)")
            {
                if (!findObjectsMap.ContainsKey(FindHitObject()))
                {
                    findObjectsMap.Add(FindHitObject(), findOjbectTime);
                    FindHitObject().transform.localScale *= 2;
                }
                else
                {
                    findObjectsMap[FindHitObject()] = findOjbectTime;
                }
            }
        }
        if (findObjectsMap.Count > 0)
        {
            foreach (var item in findObjectsMap.Where(kvp => kvp.Value < Time.timeSinceLevelLoad - 3.0f).ToList())
            {
                item.Key.transform.localScale /= 2;
                findObjectsMap.Remove(item.Key);
            }
        }
    }

    public bool IsMouseInFirstCamera()
    {
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x < ResourceManager.getInstance().getPIPCameraPosition().xMax*Screen.width && mousePos.x > ResourceManager.getInstance().getPIPCameraPosition().xMin * Screen.width;        
        bool insideHeight = mousePos.y < ResourceManager.getInstance().getPIPCameraPosition().yMax*Screen.height && mousePos.y > ResourceManager.getInstance().getPIPCameraPosition().yMin*Screen.height;
        return insideWidth && insideHeight;
    }
}
