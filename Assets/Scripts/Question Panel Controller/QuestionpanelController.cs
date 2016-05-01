using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestionpanelController : MonoBehaviour {

    public Text instructionDetails;
    public Text questionText;
    Question question;
    public GameObject questionPanel;
    public GameObject answerPanel;
    public GameObject ground;
    public Camera bgCamera;
 

    // Use this for initialization
    void Start() {
        showQuestionPanel(false);
    }

    // Update is called once per frame
    void Update () {
	
	}

    void showQuestionPanel(bool hideStatus)
    {
        questionPanel.SetActive(hideStatus);
        bgCamera.gameObject.SetActive(hideStatus);
        
    }

    void activeScreenSpaceRender(bool screenSpaceStatus)
    {

/*        Canvas canvas = GameObject.FindGameObjectWithTag("AnswerStateButton").GetComponent<Canvas>();
        if (screenSpaceStatus)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        */
    }


    public void startQuizMode()
    {

        showQuestionPanel(true);
        activeScreenSpaceRender(true);
        MapNav mapNav = ground.GetComponent<MapNav>();
        mapNav.switchOffToggleButton(true);
    }
    public void StartNasaTask()
    {

    }


    public void stopQuizMode()
    {

        showQuestionPanel(false);
        activeScreenSpaceRender(false);
        MapNav mapNav = ground.GetComponent<MapNav>();
        mapNav.switchOffToggleButton(false);
    }


    public void setQuestion(Question inQuestion)
    {
        question = inQuestion;
         fillInstruction();
        fillQuestion();
        AnswerPanelController controller = answerPanel.GetComponent<AnswerPanelController>();
        controller.setQuestion(question);
    }

    void fillInstruction()
    {
        instructionDetails.text = "PLEASE ANSWER THE QUESTION:";
        switch (question.type)
        {
            case QuestionType.Single:
                instructionDetails.text = "PLEASE SELECT AN OPTION";
                break;
 
            case QuestionType.InputNumber:
                instructionDetails.text = "PLEASE ENTER THE ANSWER";
                break;

            case QuestionType.DroneArea:
                instructionDetails.text = "PLEASE SELECT THE DRONE FROM THE MAP";
                break;

            case QuestionType.PeopleArea:
                instructionDetails.text = "PLEASE SELECT THE PEOPLE POSITION FROM THE MAP";
                break;

            case QuestionType.CarArea:
                instructionDetails.text = "PLEASE SELECT THE CAR POSITION FROM THE MAP";
                break;

            default:
                instructionDetails.text = "PLEASE ENTER THE ANSWER"; 

                break;
        }

    }
    void fillQuestion()
    {
        questionText.text = question.description;


    }

}
