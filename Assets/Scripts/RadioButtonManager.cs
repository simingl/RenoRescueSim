using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RadioButtonManager : MonoBehaviour {


	public int selectedValue;
	public bool needDefaultSelection;

	// Use this for initialization
	void Start () {

		int index = 0;

		for (index = 0;index < transform.childCount;index++) {

			Transform child = transform.GetChild(index);
			//child is your child transform
			RadioButton button = child.gameObject.GetComponent<RadioButton> ();
			button.value = index+1;

		}

		if (needDefaultSelection) {
			this.radioButtonSelected (1);
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void radioButtonSelected(int buttonValue){

		selectedValue = buttonValue;

		foreach (Transform child in transform)
		{
			//child is your child transform
			RadioButton button = child.gameObject.GetComponent<RadioButton> ();

			if (button.value == buttonValue) {
				button.setSelected (true);
			} else {
				button.setSelected (false);
			}
				



		}


	}

}
