using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TaskLoader : MonoBehaviour {

	public GameObject firstCotainerPanel;
	public GameObject secondCotainerPanel;

	public Button nextButton;
	public Button submitButton;

	public Text nameText;
	public Text taskText;
	public Text dateText;

	public GameObject MentalBtnPanel;
	public GameObject PhyscialBtnPanel;
	public GameObject TemporalBtnPanel;
	public GameObject PerformanceBtnPanel;
	public GameObject EffortBtnPanel;
	public GameObject FurstrationBtnPanel;


	// Use this for initialization
	void Start () {
	


		submitButton.gameObject.SetActive (false);
		secondCotainerPanel.SetActive (false);
		nextButton.gameObject.SetActive (true);
		firstCotainerPanel.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void nextButtonPressed(){

		secondCotainerPanel.SetActive (true);
		firstCotainerPanel.SetActive (false);
		submitButton.gameObject.SetActive (true);
		nextButton.gameObject.SetActive (false);
	}

	public void submitButtonPressed(){


		NASATasker newTasker = new NASATasker ();

		newTasker.name = nameText.text;
		newTasker.task = taskText.text;
		newTasker.date = dateText.text;

		RadioButtonManager mentalManager = MentalBtnPanel.GetComponent<RadioButtonManager> ();
		newTasker.mentalDemandValue = mentalManager.selectedValue;

		RadioButtonManager physcialManager = PhyscialBtnPanel.GetComponent<RadioButtonManager> ();
		newTasker.physicalDemandValue = physcialManager.selectedValue;

		RadioButtonManager temporalManager = TemporalBtnPanel.GetComponent<RadioButtonManager> ();
		newTasker.temporalDemandValue = temporalManager.selectedValue;

		RadioButtonManager performanceManager = PerformanceBtnPanel.GetComponent<RadioButtonManager> ();
		newTasker.performanceDemandValue = performanceManager.selectedValue;

		RadioButtonManager effortManager = EffortBtnPanel.GetComponent<RadioButtonManager> ();
		newTasker.effortDemandValue = effortManager.selectedValue;

		RadioButtonManager furstratioManager = FurstrationBtnPanel.GetComponent<RadioButtonManager> ();
		newTasker.fustationDemandValue = furstratioManager.selectedValue;

		XMLLogWriter.Instance.log (newTasker);

		PlayerPrefs.SetInt("PlayingNASATaskLoad", 0);
		PlayerPrefs.SetInt("PlayQuestions", 1);
		Application.LoadLevel ("RenoTahoe");

	}

}
