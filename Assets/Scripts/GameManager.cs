using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameStates {ID, Calibration, Brief, Main, Pause, Check, Label, SubLabel};
    public static GameManager instance;

    public GameStates gameState;
    
    public GameObject idStateCanvas;
    public InputField idInputField;
    public InputField waitTimeField;
    public InputField mainDurationField;
    public InputField correctionDurationField;
    public GameObject offsetCanvas;
    public GameObject briefStateCanvas;
    public GameObject mainStateCanvas;
    public GameObject pauseStateCanvas;
    public GameObject labelStateCanvas;
    public GameObject warningText;
    public GameObject exitButton;

    public Transform surfaceTracker;
    public GameObject offsetSprite;
    public GameObject briefingSprite;
    public GameObject bellTestSprite;
    public GameObject drawingHandler;
    public GameObject indexDisplayHandler;

    private string subjectId;
    //[HideInInspector]
    public string dataPath;
    private float timeStamp;
    private float waitTime;
    private float mainTime;
    private float correctionTime;
    private int lineCount;
    private int labeledLineCount;
    private float startTime;
    private int width, height;
    private RenderTexture renderTexture;
    private Texture2D cameraView;
    private Rect cameraRect;
    private DrawingScript drawingScript;
    private BellSpriteScript bellSpriteScript;
    private byte[] bytes;
    private float xMax;
    private float yMax;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        gameState = GameStates.ID;
        if (PlayerPrefs.HasKey("Wait Time"))
        {
            waitTime = PlayerPrefs.GetFloat("Wait Time");
            waitTimeField.text = waitTime.ToString();
        }

        if (PlayerPrefs.HasKey("Main Time"))
        {
            mainTime = PlayerPrefs.GetFloat("Main Time");
            mainDurationField.text = mainTime.ToString();
        }

        if (PlayerPrefs.HasKey("Correction Time"))
        {
            correctionTime = PlayerPrefs.GetFloat("Correction Time");
            correctionDurationField.text = correctionTime.ToString();
        }

        width = 3840;
        height = 2160;
        lineCount = 0;
        labeledLineCount = 0;

        xMax = surfaceTracker.position.x;
        yMax = surfaceTracker.position.y;

        drawingScript = drawingHandler.GetComponent<DrawingScript>();
        bellSpriteScript = bellTestSprite.GetComponent<BellSpriteScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLineLabeling()
    {
        labeledLineCount++;
        if(labeledLineCount==lineCount)
        {
            warningText.SetActive(false);
            exitButton.SetActive(true);
        }
    }

    public float X_2_PupilLab(float x)
    {
        return 1 / (2 * xMax) * x + 0.5f;
    }

    public float Y_2_PupilLab(float y)
    {
        return 1 / (2 * yMax) * y + 0.5f;
    }

    public Vector2 Position2PupilLab(Vector2 position)
    {
        return new Vector2(X_2_PupilLab(position.x), Y_2_PupilLab(position.y));
    }

    public void StartCalibration()
    {
        subjectId = idInputField.text;
        dataPath = "Data/" + GameManager.instance.subjectId;
        Directory.CreateDirectory(dataPath);
        StreamWriter writer = File.CreateText(dataPath + "/Experiments Parameters.txt");
        writer.WriteLine("Subject ID \t\t" + subjectId);
        waitTime = float.Parse(waitTimeField.text);
        PlayerPrefs.SetFloat("Wait Time", waitTime);
        writer.WriteLine("Wait Time(s) \t\t" + waitTime.ToString());
        mainTime = float.Parse(mainDurationField.text);
        PlayerPrefs.SetFloat("Main Time", mainTime);
        writer.WriteLine("Main Duration(m) \t\t" + mainTime.ToString());
        mainTime *= 60f;
        correctionTime = float.Parse(correctionDurationField.text);
        PlayerPrefs.SetFloat("Correction Time", correctionTime);
        writer.WriteLine("Correction Duration(m) \t\t" + correctionTime.ToString());
        correctionTime *= 60f;
        Transform[] markerTransforms = offsetSprite.GetComponentsInChildren<Transform>();
        writer.WriteLine("Marker Index \t X \t Y");
        for (int i = 1; i < markerTransforms.Length; i++)
        {
            writer.WriteLine(i.ToString() + " \t " + X_2_PupilLab(markerTransforms[i].position.x).ToString() + "\t" + Y_2_PupilLab(markerTransforms[i].position.y).ToString());
        }
        writer.Flush();
        writer.Close();
        gameState = GameStates.Calibration;
        idStateCanvas.SetActive(false);
        offsetCanvas.SetActive(true);
        offsetSprite.SetActive(true);
    }

    public void StartBrief()
    {
        gameState = GameStates.Brief;
        offsetSprite.SetActive(false);
        offsetCanvas.SetActive(false);
        briefStateCanvas.SetActive(true);
        briefingSprite.SetActive(true);
    }

    public void StartMain()
    {
        gameState = GameStates.Main;
        briefingSprite.SetActive(false);
        bellTestSprite.SetActive(true);
        briefStateCanvas.SetActive(false);
        mainStateCanvas.SetActive(true);
        Invoke("StartPause", mainTime);
        startTime = Time.time + waitTime;
    }

    public int GetLineIndex()
    {
        lineCount++;
        return lineCount;
    }

    public void DeattachIndexDisplay(GameObject canvas)
    {
        canvas.transform.SetParent(indexDisplayHandler.transform,false);
    }

    public float GetTimeStamp()
    {
        return Time.time - startTime;
    }

    public void StartPause()
    {
        if (gameState == GameStates.Check)
            StartLabel();
        else if(gameState == GameStates.Main)
        {
            gameState = GameStates.Pause;
            mainStateCanvas.SetActive(false);
            indexDisplayHandler.SetActive(true);
            ScreenCapture(dataPath + "/Initial Bell Selection.png");
            indexDisplayHandler.SetActive(false);
            pauseStateCanvas.SetActive(true);
        }
    }

    public void StartCheck()
    {
        gameState = GameStates.Check;
        bellSpriteScript.ShowFixation();
        drawingHandler.SetActive(false);
        Invoke("ShowDrawing", 3f);
        pauseStateCanvas.SetActive(false);
        mainStateCanvas.SetActive(true);
        Invoke("StartLabel", correctionTime);
        startTime = Time.time + waitTime;
    }

    public void ShowDrawing()
    {
        drawingHandler.SetActive(true);
    }

    public void StartLabel()
    {
        drawingScript.DisableInteraction();
        gameState = GameStates.Label;
        pauseStateCanvas.SetActive(false);
        mainStateCanvas.SetActive(false);
        indexDisplayHandler.SetActive(true);
        ScreenCapture(dataPath + "/Corrected Bell Selection.png");
        indexDisplayHandler.SetActive(false);
        labelStateCanvas.SetActive(true);
    }

    public void CloseTest()
    {
        Application.Quit();
    }

    public void ScreenCapture(string path)
    {
        renderTexture = new RenderTexture(width, height, 24);
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();
        RenderTexture.active = renderTexture;
        cameraView = new Texture2D(width, height, TextureFormat.RGB24, false);
        cameraView.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        bytes = cameraView.EncodeToPNG();
        Destroy(cameraView);
        if (bytes != null)
        {
            File.WriteAllBytes(path, bytes);
        }
    }
}
