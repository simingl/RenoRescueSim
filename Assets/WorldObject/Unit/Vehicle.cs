using UnityEngine;
using System.Collections;
using RTS;

public class Vehicle : WorldObject {
	private GameObject mark;

	override protected void Start () {
		base.Start ();
		
		this._isSelectable = false;

		this.scoreValue = 200;

		mark = GameObject.CreatePrimitive (PrimitiveType.Cube);
		mark.layer = gameObject.layer;
		mark.GetComponent<Collider> ().enabled = false;
		
		mark.transform.parent = transform;
		mark.transform.localScale = Vector3.one * 3f;
		mark.transform.localPosition = new Vector3 (0, 5f, 0);
		mark.transform.rotation = gameObject.transform.rotation;
		mark.GetComponent<Renderer> ().material.color = Color.red;
	}
	public void setColor(Color color){
		mark.GetComponent<Renderer> ().material.color = color;
	}
	
	public void Mark(){
		mark.GetComponent<Renderer> ().material.color = Color.green;
		SetLayerRecursively (gameObject, gameObject.layer, ResourceManager.LayerEntitiesCommon);
		ScoreManager.score += this.scoreValue;
	}

    public int GetVehicleArea()
    {
        int HButtonsNum = ConfigManager.getInstance().getSceneHorizontalButtonsNum();
        int VButtonsNum = ConfigManager.getInstance().getSceneVerticalButtonsNum();

        float gridSizeOfSceneWidth = 160.0f / HButtonsNum; //200 size with 100 offset
        float gridSizeOfSceneHeight = 160.0f / VButtonsNum;
        int dronePostionOffset = 80;

        int result = 0;
        for (int i = 0; i < HButtonsNum; ++i)
        {
            for (int j = 0; j < VButtonsNum; ++j)
            {
                if (gridSizeOfSceneHeight * i < (this.transform.position.z + dronePostionOffset) &&
                   gridSizeOfSceneHeight * (i + 1) > (this.transform.position.z + dronePostionOffset) &&
                   gridSizeOfSceneWidth * j < (this.transform.position.x + dronePostionOffset) &&
                   gridSizeOfSceneWidth * (j + 1) > (this.transform.position.x + dronePostionOffset)
                   )
                {
                    result = j + (VButtonsNum - i - 1) * HButtonsNum;
                }
            }
        }
        return result;
    }


}
