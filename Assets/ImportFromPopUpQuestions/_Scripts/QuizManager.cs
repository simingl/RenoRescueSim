using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;
using RTS;


public class QuizManager : MonoBehaviour
{

  //  static string studentID;
    public Text ques;
    public Texture2D btnTexture;
    public GUIStyle btnGuiStyle;
    public GameObject RenoFolder;
    public GameObject Questions;
    public GameObject ScoreText;
    public InputField InputNumber;
    public GameObject OptionsButton;
    public GameObject StartButton;
    public GameObject ResumeButton;
    public GameObject PlayerFolder;
    private bool playerFolderActive = true;
    [HideInInspector]
    public bool startQuestion = false;
    private float timeNow = 0f;
    // public GameObject backGround;
    //private bool write = true;
    private bool write ;
    [HideInInspector]
    public int startAnswerTime;
    [HideInInspector]
    public int endAnswerTime;
    //minimapPosition---
    private Vector3 minimapPosition;
    private Camera minimapCam;
    public GameObject ButtonsOnMiniMap;
    private int HButtonsNum;
    private int VButtonsNum;
    private Vector2 minimapSize;
    public GameObject ButtonsOnMiniMapFolder;
    public RectTransform questionPanel;
    //minimapPosition---
    public GameObject resultTextPrefab;
    //   public Text resultTextInPrefab;
    public Text resultTextBoard;
    public GameObject resultBoard;
    [HideInInspector]
    public bool displayResultBoard;
    private bool startPopupQuestion;
    //    public GameObject btnChangeScene;
    private ConfigManager configManager;
    private QuizSettingContainer quizSettings;
    private QuizSettingContainer writeToStudentID;
    private HUD hud;
    private SceneManager sceneManager;

    private Vehicle vehicle;
    private NPC npc;
    public Dictionary<GameObject, KeyValuePair<float, bool>> NPCShowTimeDic;
    public int markedPeople;
    public int markedCars;
    private float quizStartTime = 0;
    private CameraPIP cameraPIP;
    public bool NasaTaskLoadIndexComplete;
    private TaskLoader taskLoader;
    void Start()
    {
        instance = this;
        markedPeople = 0;
        markedCars = 0;
        startAnswerTime = 0;
        endAnswerTime = 0;
        InputNumber.gameObject.SetActive(false);
        ques.text = "";
        AnswerStateButton.SetActive(false);
        StartButton.GetComponentInChildren<Text>().text = "Start";
        resultTextBoard.text = "";
        QuizManager.getInstance().displayResultBoard = false;
        resultBoard.gameObject.SetActive(false);
        //       btnChangeScene.SetActive(false);
        configManager = ConfigManager.getInstance();
        HButtonsNum = configManager.getSceneHorizontalButtonsNum();
        VButtonsNum = configManager.getSceneVerticalButtonsNum();
        minimapCam = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<Camera>();
        minimapPosition = new Vector3(minimapCam.pixelRect.x, minimapCam.pixelRect.yMax, 0);
        minimapSize = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>().lastMinimapSize;
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
  //      writeToStudentID = new QuizSettingContainer();
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();

        cameraPIP = GameObject.FindGameObjectWithTag("Camera_2nd_view").GetComponent<CameraPIP>();
        vehicle = GameObject.FindGameObjectWithTag("Car").GetComponent<Vehicle>();
        NPCShowTimeDic = new Dictionary<GameObject, KeyValuePair<float, bool>>();
        npc = GameObject.FindGameObjectWithTag("People").GetComponent<NPC>();
        QuizManager.getInstance().isWriteToXML = true;
        QuizManager.getInstance().looploadControl = getQuizStartTime();
        QuizManager.getInstance().write  = true;
        QuizManager.getInstance().startPopupQuestion = true;
        QuizManager.getInstance().answered = false;
        QuizManager.getInstance().NasaTaskLoadIndexComplete = false;
        taskLoader = GameObject.FindGameObjectWithTag("TaskLoader").GetComponent<TaskLoader>();
}

    //private bool isWriteToXML = true;
    private bool isWriteToXML;
    private float writeToXMLFrequency = 0.5f;
    private float looploadControl;

	public string GenerateFileName(string context)
	{
        //return context + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
        return context + "_NASATask_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xml"; 

    }

    void Update()
    {
        //int playingNASATask = PlayerPrefs.GetInt("PlayingNASATaskLoad");
        //if (playingNASATask == 1)
        //{
        //    return;
        //}
        //else if (PlayerPrefs.GetInt("PlayQuestions") == 1)
        //{
        //    PlayerPrefs.SetInt("PlayQuestions", 0);
        //    OnPopUpQuestionButtonClick();
        //}
        timeNow = Time.timeSinceLevelLoad;
        //if (getQuizStartTime()- timeNow <= writeToXMLFrequency && isWriteToXML)
        if (QuizManager.getInstance().looploadControl - timeNow <= writeToXMLFrequency && QuizManager.getInstance().isWriteToXML)
        {
            if (playerFolderActive)
            {
                QuizManager.getInstance().quizStartTime = Time.timeSinceLevelLoad;
                GetArea();
                GetPeopleAndCarNums();
                GetLastPeopleOrCarIndex();
                GetPeopleOrCarPosition();
                WriterResourceStatus();
            }
            QuizManager.getInstance().isWriteToXML = false;
        }
        if (QuizManager.getInstance().answered)
        {
            destroyOptions();
            //AnswerState(QuizManager.getInstance().answerNum);

            if (QuizManager.getInstance().write  == false && QuizManager.getInstance().questionButtonCounter + 1 == QuizManager.getInstance().getQuizSettings().quiz.question.Count)
            {                        
                ResumeSceneButtonClick();
            }
        }
        
        if (timeNow > QuizManager.getInstance().looploadControl && QuizManager.getInstance().write )
        {
            if (QuizManager.getInstance().questionButtonCounter + 1 == QuizManager.getInstance().getQuizSettings().quiz.question.Count)
            {
                QuizManager.getInstance().write = false;
            }
            else
            {
                //PlayerPrefs.SetInt("PlayingNASATaskLoad", 1);

                //if (!QuizManager.getInstance().NasaTaskLoadIndexComplete)
                //{
                //    XMLLogWriter.Instance.setFileName(GenerateFileName("NASATaskLoadIndex") + ".xml");
                //    Application.LoadLevel("NasaTaskLoadIndex");
                //}
                if (!QuizManager.getInstance().NasaTaskLoadIndexComplete)
                {
                    //XMLLogWriter.Instance.setFileName(GenerateFileName("NASATaskLoadIndex") + ".xml");
                    XMLLogWriter.Instance.setFileName(GenerateFileName(GetRunTimes()));
                    NasaTaskQuestionsStart();                    
                }
                if (QuizManager.getInstance().NasaTaskLoadIndexComplete)
                {
                    OnPopUpQuestionButtonClick();
                }
            }
        }
        //if (QuizManager.getInstance().questionButtonCounter == QuizManager.getInstance().getQuizSettings().quiz.question.Count 
        //    )
        //{

        //    if (QuizManager.getInstance().displayResultBoard) {
        //        QuizManager.getInstance().displayResultBoard = false;
        //        DisplayResult();
        //    }            
        //}
    }
    private string GetRunTimes()
    {
        string result= ConfigManager.getInstance().studentID +"_"+ HumanFriendlyInteger.IntegerToWritten(ConfigManager.getInstance().questionaryIndex); 
        return result;
    }

    private static QuizManager instance = new QuizManager();

    private XmlDocument doc = new XmlDocument();

    public static QuizManager getInstance()
    {
        return instance;
    }
    public QuizSettingContainer getQuizSettings()
    {
        if (quizSettings == null)
        {
            quizSettings = QuizSettingContainer.readData();
            //quizSettings.writeData();
        }
        return quizSettings;
    }
    
    public GameObject AnswerStateButton;
    [HideInInspector]
    public int answerNum;
    public void AnswerState(int n)
    {
        destroyOptions();
        AnswerStateButton.SetActive(true);
        if (n == 0)
        {
            AnswerStateButton.GetComponentInChildren<Text>().text = "wrong";
        }
        else
        {
            AnswerStateButton.GetComponentInChildren<Text>().text = "right";
        }
    }
    [HideInInspector]
    public int optionCounter = 0;
    [HideInInspector]
    public int questionButtonCounter = -1;
    private GameObject CloneOptionsButton;
    public List<GameObject> tmpCloneOptionsButton = new List<GameObject>();

    private void InstantiateOptionsButtons()
    {
        Vector3 myVector3 = new Vector3(0, -40 * (optionCounter + 1));
        string str = QuizManager.getInstance().quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt[optionCounter].name + ": " + QuizManager.getInstance().quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt[optionCounter].optDescription;
        ClonePrefabs(OptionsButton, ques.transform, myVector3, str);// 
    }

    private int getQuizStartTime()
    {
        return configManager.getSceneQuizStartTime();
    }

    private void InstantiateButtonsInSquare(int n)
    {
        int num = 0;
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                string str = num++.ToString();
                Vector3 myVector3 = new Vector3(-50 * (n - 1) + 100 * j, -50 - 40 * (i + 1), 0);
                ClonePrefabs(OptionsButton, ques.transform, myVector3, str);//
            }
        }
    }

    private void InstantiateButtonsInMiniMap()
    {
        float buttonWidth = minimapSize.x / HButtonsNum;
        float buttonHeight = minimapSize.y / VButtonsNum;
        int num = 0;
        for (int i = 0; i < VButtonsNum; ++i)
        {
            for (int j = 0; j < HButtonsNum; ++j)
            {
                string str = num++.ToString();
                Vector3 myVector3 = minimapPosition + new Vector3(buttonWidth * j, -buttonHeight * (i+1), 0);
                //          Vector3 myVector3 = new Vector3(buttonWidth * j, -buttonHeight * i, 0);
                //Debug.Log ("minimapPosition is: "+minimapPosition );
                //Debug.Log("myvector3 is: "+ myVector3);
                ClonePrefabs(ButtonsOnMiniMap, ButtonsOnMiniMapFolder.transform, myVector3, str);
            }
        }
    }
    public bool answered;

    private void showNextQuestion()
    {
        int sizeOfStr;
        sizeOfStr = (int)QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].description.Length;
        ques.rectTransform.sizeDelta = new Vector2(ques.GetComponent<Text>().fontSize * sizeOfStr, 30);
        ques.text = QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].description;
        setQuestionToPanel();
   }


    void setQuestionToPanel()
    {

        Question currentQuestion = QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter];
        QuestionpanelController controller = questionPanel.GetComponent<QuestionpanelController>();
        controller.setQuestion(currentQuestion);
    }

    private void destroyOptions()
    {
        for (int i = 0; i < QuizManager.getInstance().tmpCloneOptionsButton.Count; ++i)
        {
            Destroy(QuizManager.getInstance().tmpCloneOptionsButton[i].gameObject);
        }
        QuizManager.getInstance().tmpCloneOptionsButton.Clear();
    }

    private void CheckInputNumber()
    {
        if (InputNumber.text == QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].answer.ToString())
        {
            QuizManager.getInstance().answerNum = 1;
        }
        else
        {
            QuizManager.getInstance().answerNum = 0;
        }
        QuizManager.getInstance().endAnswerTime =(int) Time.timeSinceLevelLoad;
        string timeConsumed = (QuizManager.getInstance().endAnswerTime - QuizManager.getInstance().startAnswerTime).ToString();
        string str;
        str = InputNumber.text;
        QuizManager.getInstance().WriteToXml(str, QuizManager.getInstance().questionButtonCounter, 6);
        QuizManager.getInstance().WriteToXml(timeConsumed, QuizManager.getInstance().questionButtonCounter, 7);
        // quizManager.WriteToXml(str, quizManager.questionButtonCounter % quizManager.getQuizSettings().quiz.question.Count, 6);
        InputNumber.gameObject.SetActive(false);
        QuizManager.getInstance().answered = true;
        QuizManager.getInstance().displayResultBoard = true;
    }

    private void GetArea()  //n is the number of drones
    {
        GameObject[] writeTheNumberOfDroneToXML = GameObject.FindGameObjectsWithTag("Drone");
        GameObject[] writeTheNumberOfPeopleToXML = GameObject.FindGameObjectsWithTag("People");
        GameObject[] writeTheNumberOfCarsToXML = GameObject.FindGameObjectsWithTag("Car");
        //var drone = writeTheNumberOfDroneToXML.GetComponent<Drone>();
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.DroneArea)
            {
                foreach (GameObject obj in writeTheNumberOfDroneToXML)
                {
                    if (obj.GetComponent<Drone>().droneNumber.ToString() == QuizManager.getInstance().getQuizSettings().quiz.question[i].number)
                    {
                        WriteToXml(obj.GetComponent<Drone>().getDroneArea().ToString(), i, 5);
                    }
                }
            }
            else if(QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.CarArea)
            {
                for(int j=0; j<writeTheNumberOfCarsToXML.Length; ++j)
                {
                    if(j.ToString() == QuizManager.getInstance().getQuizSettings().quiz.question[i].number)
                    {
                        WriteToXml(writeTheNumberOfCarsToXML[j].GetComponent<Vehicle>().GetVehicleArea().ToString(), i, 5);
                    }
                }
            }
            else if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.PeopleArea)
            {
                for (int j = 0; j < writeTheNumberOfPeopleToXML.Length; ++j)
                {
                    if (j.ToString() == QuizManager.getInstance().getQuizSettings().quiz.question[i].number)
                    {
                        WriteToXml(writeTheNumberOfPeopleToXML[j].GetComponent<NPC>().GetNPCArea().ToString(), i, 5);
                    }
                }
            }

        }
    }

    private void GetPeopleOrCarPosition()
    {
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.PeopleArea)
            {
                WriteToXml(npc.GetNPCArea().ToString(), i, 5);
            }
            else if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.CarArea)
            {
                WriteToXml(vehicle.GetVehicleArea().ToString(), i, 5);
            }

        }
    }

    private void GetLastPeopleOrCarIndex()
    {
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.PeopleArea)
            {
                if (cameraPIP.GetLastPersonOrCarIndex("npc") == -1)
                {}
                else
                {
                    WriteToXml(cameraPIP.GetLastPersonOrCarIndex("npc").ToString(), i, 8);
                }
            }
            else if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.CarArea)
            {
                if (cameraPIP.GetLastPersonOrCarIndex("car") == -1)
                {}
                else
                {
                    WriteToXml(cameraPIP.GetLastPersonOrCarIndex("car").ToString(), i, 8);
                }
            }
        }
    }
    private void GetPeopleAndCarNums() //identify,rescue,untag num
    {
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.InputNumberWithPeopleIdentify)
            {
                WriteToXml(cameraPIP.GetIdentifyPeoPleNum().ToString(), i, 5);
            }
            else if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.InputNumberWithCarIdentify)
            {
                WriteToXml(cameraPIP.GetIdentifyCarNum().ToString(), i, 5);
            }
            else if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.InputNumberWithPeopleRescue)
            {
                WriteToXml(QuizManager.getInstance().markedPeople.ToString(), i, 5);
            }
            else if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.InputNumberWithCarRescue)
            {
                WriteToXml(QuizManager.getInstance().markedCars.ToString(), i, 5);
            }
            else if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.InputNumberWithPeopleUntag)
            {
                WriteToXml(cameraPIP.GetUnableTagPeopleNum().ToString(), i, 5);
            }
            else if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.InputNumberWithCarUntag)
            {
                WriteToXml(cameraPIP.GetUnableTagCarsNum().ToString(), i, 5);
            }
        }
    }
    private void WriterResourceStatus()
    {
        Drone[] allDrones= sceneManager.getAllDrones();        
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.InputNumberWithBattery)
            {
                WriteToXml(GetResourceStatus(allDrones[Int32.Parse(QuizManager.getInstance().getQuizSettings().quiz.question[i].number)], "Battery"), i, 5);
            }
            else if(QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.InputNumberWithHeight)
            {
                //         Debug.Log("number is: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].number);
                WriteToXml(GetResourceStatus(allDrones[Int32.Parse(QuizManager.getInstance().getQuizSettings().quiz.question[i].number)], "Height"), i, 5);
            }
            else if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.InputNumberwithSpeed)
            {
                //        Debug.Log("number is: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].number);
                WriteToXml(GetResourceStatus(allDrones[Int32.Parse(QuizManager.getInstance().getQuizSettings().quiz.question[i].number)], "Speed"), i, 5);
            }
        }
    }

    private string GetResourceStatus(Drone obj, string str)
    {
        Drone unit = (Drone)obj;
        string battery="";
        string speed="";
        string height="";
        battery = (int)(unit.currentBattery) / 60 + "." + ((int)unit.currentBattery) % 60;
        speed = unit.speed.ToString();
        height = unit.transform.position.y.ToString();
        if (str == "Battery")
        {
            return battery;
        }
        else if(str == "Height")
        {
            return height;
        }
        else if (str=="Speed")
        {
            return speed;
        }
        return "";
    }


    //private string GetPeopleAndCarsNum(string str)
    //{
    //    string result = "";
    //    string resultInt = 0;
    //    if(str == "npc")
    //    {
    //        resultInt = cameraPIP.GetIdentifyCarNum();
    //    }

    //    return result;

    //}


    //write to XML file
    public void WriteToXml(string str, int questionCount, int num)
    {
        QuizSettingContainer myContainer = QuizManager.getInstance().getQuizSettings();
        if (num == 5)   //writeToXML answer;
        {
            myContainer.quiz.question[questionCount].answer = str;
        }
        if (num == 6) //writeToXML userAnswer
        {
            myContainer.quiz.question[questionCount].userAnswer = str;
        }
        if (num == 7) //writeToXML timeConsuming
        {
            myContainer.quiz.question[questionCount].timeConsumed= str;
        }
        if(num==8)
        {
            myContainer.quiz.question[questionCount].number = str;
        }
        //QuizSettingContainer.Serialize(myContainer, ConfigManager.getInstance().studentID);
        string fileName = GetRunTimes() + "_Questionnaire";
        QuizSettingContainer.Serialize(myContainer, fileName);
        //QuizSettingContainer.WriteData(myContainer, configManager.studentID);
    }


    void enableQuestionPanel(bool enableStatus)
    {
        QuestionpanelController controller = questionPanel.GetComponent<QuestionpanelController>();
        if (enableStatus)
        {
            controller.startQuizMode();
        }
        else
        {
            controller.stopQuizMode();
        }
    }

    private void NasaTaskQuestionsStart()  //start nasa task questions
    {
        QuestionpanelController controller = questionPanel.GetComponent<QuestionpanelController>();
        ScoreText.SetActive(false);
        PlayerFolder.SetActive(false);
        MinimapManagement mapManagement = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>();
        CameraMain cameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMain>();
        //   mapManagement.cameraTurnOff(true);
        mapManagement.cameraTurnOff(true);
        cameraMain.turnOff(true);
        MapNav mapNav = controller.ground.GetComponent<MapNav>();
        mapNav.switchOffToggleButton(true);
        taskLoader.NasaTaskLoadIndex.SetActive(true);
        controller.bgCamera.gameObject.SetActive(true);
    }

    public void OnPopUpQuestionButtonClick()
    {
        if (QuizManager.getInstance().answered || QuizManager.getInstance().startPopupQuestion)
            {                
                enableQuestionPanel(true);
                QuizManager.getInstance().startAnswerTime = (int)Time.timeSinceLevelLoad;
                int droneCount = configManager.getSceneDroneCount();
                QuizManager.getInstance().questionButtonCounter = (++QuizManager.getInstance().questionButtonCounter) % QuizManager.getInstance().getQuizSettings().quiz.question.Count; // problem
                showNextQuestion();
                StartButton.GetComponentInChildren<Text>().text = "Next";
                AnswerStateButton.SetActive(false);
                if (getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.Single)
                {
                    for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt.Count; ++i)
                    {
                        optionCounter = i;
                        InstantiateOptionsButtons();
                    }
                }
                if (getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumber                   ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumberWithHeight         ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumberWithBattery        ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumberwithSpeed          ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumberWithPeopleIdentify ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumberWithPeopleRescue   ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumberWithPeopleUntag    ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumberWithCarUntag       ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumberWithCarRescue)

                {
                    InputNumber.text = "";
                    InputNumber.gameObject.SetActive(true);
                }
                if (getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.DroneArea ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.CarArea ||
                    getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.PeopleArea)
                {
               //     GetDronesArea(droneCount);
              //      InstantiateButtonsInSquare((int)Mathf.Sqrt(droneCount));
                  InstantiateButtonsInMiniMap();
                }
            //   backGround.SetActive(true);
            /*       ScoreText.SetActive(false);
                   RenoFolder.SetActive(false);
                   Questions.SetActive(true);
                   PlayerFolder.SetActive(false);
                   */
     //       btnChangeScene.SetActive(true);
           

            //   backGround.SetActive(true);
            ScoreText.SetActive(false);
            //RenoFolder.SetActive(false);
            Questions.SetActive(true);
            PlayerFolder.SetActive(false);
            playerFolderActive = false;
            //hideShowGameObjects
            MinimapManagement mapManagement = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>();
            CameraMain cameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMain>();
            //   mapManagement.cameraTurnOff(true);
            mapManagement.cameraTurnOff(false);
            cameraMain.turnOff(true);
            //   backGround.SetActive(true);

        }
        QuizManager.getInstance().startPopupQuestion = false;
        QuizManager.getInstance().answered = false;
        QuizManager.getInstance().displayResultBoard = false;
        //}
    }

    private void ClonePrefabs(GameObject CloneObjs, Transform parrentTransform, Vector3 offSetVect3, string description)
    {
        CloneOptionsButton = (GameObject)Instantiate(CloneObjs, parrentTransform.position + offSetVect3, Quaternion.identity);
        CloneOptionsButton.transform.parent = parrentTransform;

        CloneOptionsButton.GetComponentInChildren<Text>().text = description;
        QuizManager.getInstance().tmpCloneOptionsButton.Add(CloneOptionsButton);
    }

    private void DisplayResult()  //Result board.
    {
        resultBoard.gameObject.SetActive(true);
        int max = 0;
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            if (max < QuizManager.getInstance().getQuizSettings().quiz.question[i].description.Length)
            {
                max = QuizManager.getInstance().getQuizSettings().quiz.question[i].description.Length;
            }
        }
        int maxLength = max * 12;
        //       resultTextInPrefab.rectTransform.sizeDelta = new Vector2(maxLength, 25);
        resultTextBoard.rectTransform.sizeDelta = new Vector2(maxLength, 25 * 3 * QuizManager.getInstance().getQuizSettings().quiz.question.Count);
        resultTextBoard.text = "Results:\n";
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            resultTextBoard.text += 
                i+1 + ": " + QuizManager.getInstance().getQuizSettings().quiz.question[i].description + "\n"
                 + "Answer: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].answer
                 + "  " + "UserAnswer: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].userAnswer
                 + "  " + "TimeConsuming: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].timeConsumed + "sec" + "\n";
        }
    }

    public void ResumeSceneButtonClick()
    {
        enableQuestionPanel(false);
        StartButton.GetComponentInChildren<Text>().text = "Start";
        resultBoard.gameObject.SetActive(false);
        /*    RenoFolder.SetActive(true);
            Questions.SetActive(false);
            ScoreText.SetActive(true);
            PlayerFolder.SetActive(true);
            // backGround.SetActive(false);*/

        //BackgroundManager backgroundManager = GameObject.FindGameObjectWithTag("AnswerStateButton").GetComponent<BackgroundManager>();
        //backgroundManager.reset();

        //RenoFolder.SetActive(true);
   //     btnChangeScene.SetActive(false);
        Questions.SetActive(false);
        ScoreText.SetActive(true);
        PlayerFolder.SetActive(true);
        playerFolderActive = true;

        // backGround.SetActive(false);
        //Camera_minimap
        MinimapManagement mapManagement = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>();
        CameraMain cameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMain>();
        mapManagement.cameraTurnOff(false);
        cameraMain.turnOff(false);
        //QuizManager.getInstance().answered = true;
        QuizManager.getInstance().answered = false;
        float timeNow = Time.timeSinceLevelLoad;
        QuizManager.getInstance().looploadControl = QuizManager.getInstance().looploadControl+getQuizStartTime()+ timeNow-QuizManager.getInstance().quizStartTime;
        QuizManager.getInstance().startPopupQuestion = true;
        QuizManager.getInstance().isWriteToXML = true;
        QuizManager.getInstance().write  = true;
        QuizManager.getInstance().questionButtonCounter = -1;
        QuizManager.getInstance().NasaTaskLoadIndexComplete = false;
        ++ConfigManager.getInstance().questionaryIndex;
    }
}