using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RadioButton : MonoBehaviour {

	public int value;
	public Sprite selectedImage;
	public Sprite unselectedImage;
	private bool isSelected;

	// Use this for initialization
	void Start () {
	
		Button b = this.gameObject.GetComponent<Button>();
		b.onClick.AddListener(() => buttonClicked());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setSelected(bool status){

		isSelected = status;
		//Button button = this.gameObject.GetComponent<Button>();
		Image image = this.gameObject.GetComponent<Image>();

		if (this.isSelected) {

			image.sprite = selectedImage;

		} else {

			image.sprite = unselectedImage;
		}
	}


	public void buttonClicked(){

		RadioButtonManager buttonManager = this.gameObject.GetComponentInParent<RadioButtonManager> ();
		buttonManager.radioButtonSelected (this.value);

	}

}
