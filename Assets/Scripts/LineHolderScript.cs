using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LineHolderScript : MonoBehaviour
{
    public GameObject labelCanvas;
    public GameObject indexCanvas;
    public Text indexDisplay;

    private int lineIndex;
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private List<Vector2> linePoints;
    private List<float> timeStamps;
    private int positionIndex;
    private string linePeriod;
    StreamWriter writer;
    private bool labeled;
    // Start is called before the first frame update
    void Start()
    {
        labeled = false;
        if (GameManager.instance.gameState == GameManager.GameStates.Main)
            linePeriod = "Main";
        else if (GameManager.instance.gameState == GameManager.GameStates.Check)
            linePeriod = "Check";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeReferences()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        linePoints = new List<Vector2>();
        timeStamps = new List<float>();

        writer = null;
        lineIndex = GameManager.instance.GetLineIndex();
        indexDisplay.text = lineIndex.ToString();
        positionIndex = 0;
        GameManager.instance.DeattachIndexDisplay(indexCanvas);
    }

    public void InitializeLine(Vector3 initialPosition)
    {
        indexCanvas.transform.position = initialPosition;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(positionIndex, initialPosition);
        timeStamps.Add(GameManager.instance.GetTimeStamp());
        linePoints.Add(initialPosition);
        edgeCollider.points = linePoints.ToArray();
        positionIndex++;
    }

    public void AddPosition(Vector3 newPosition)
    {
        if (Vector3.Distance(lineRenderer.GetPosition(positionIndex-1),newPosition)>=.5)
        {
            lineRenderer.positionCount = positionIndex + 1;
            lineRenderer.SetPosition(positionIndex, newPosition);
            timeStamps.Add(GameManager.instance.GetTimeStamp());
            linePoints.Add(newPosition);
            edgeCollider.points = linePoints.ToArray();
            positionIndex++;
        }
    }

    public void WriteCoordinatesCorrected(int choice)
    {
        switch (choice-1)
        {
            case 0:
                WriteCoordinates("Corrected Bell");
                break;
            case 1:
                WriteCoordinates("Corrected House");
                break;
            case 2:
                WriteCoordinates("Corrected Pot");
                break;
            case 3:
                WriteCoordinates("Corrected Gun");
                break;
            case 4:
                WriteCoordinates("Corrected Horse");
                break;
            case 5:
                WriteCoordinates("Corrected Guitar");
                break;
            case 6:
                WriteCoordinates("Corrected Shark");
                break;
            case 7:
                WriteCoordinates("Corrected Apple");
                break;
            case 8:
                WriteCoordinates("Corrected Tree");
                break;
            case 9:
                WriteCoordinates("Corrected Bird");
                break;
            case 10:
                WriteCoordinates("Corrected Car");
                break;
            case 11:
                WriteCoordinates("Corrected Saw");
                break;
            case 12:
                WriteCoordinates("Corrected Hammer");
                break;
            case 13:
                WriteCoordinates("Corrected Key");
                break;
        }
    }

    public void WriteCoordinatesWrong(int choice)
    {
        switch (choice-1)
        {
            case 0:
                WriteCoordinates("Wrong House");
                break;
            case 1:
                WriteCoordinates("Wrong Pot");
                break;
            case 2:
                WriteCoordinates("Wrong Gun");
                break;
            case 3:
                WriteCoordinates("Wrong Horse");
                break;
            case 4:
                WriteCoordinates("Wrong Guitar");
                break;
            case 5:
                WriteCoordinates("Wrong Shark");
                break;
            case 6:
                WriteCoordinates("wrong Apple");
                break;
            case 7:
                WriteCoordinates("Wrong Tree");
                break;
            case 8:
                WriteCoordinates("Wrong Bird");
                break;
            case 9:
                WriteCoordinates("Wrong Car");
                break;
            case 10:
                WriteCoordinates("Wrong Saw");
                break;
            case 11:
                WriteCoordinates("Wrong Hammer");
                break;
            case 12:
                WriteCoordinates("Wrong Key");
                break;
        }
    }
    public void WriteCoordinates(string label)
    {
        if (!labeled)
        {
            GameManager.instance.UpdateLineLabeling();
            labeled = true;
        }
        writer = File.CreateText(GameManager.instance.dataPath+ "/" + label + " " + lineIndex + ".csv");
        writer.WriteLine("X;Y;Time;Period");
        Vector2 tempPoints;
        for (int i = 0; i < linePoints.Count; i++)
        {
            tempPoints = linePoints[i];
            writer.WriteLine(GameManager.instance.X_2_PupilLab(tempPoints.x).ToString() + ';' + GameManager.instance.Y_2_PupilLab(tempPoints.y).ToString() + ';' + timeStamps[i].ToString()+';'+linePeriod);
        }
        writer.Flush();
        writer.Close();
        lineRenderer.material.color = Color.green;
        labelCanvas.SetActive(false);
        GameManager.instance.gameState = GameManager.GameStates.Label;
    }

    private void OnMouseDown()
    {
        if (GameManager.instance.gameState == GameManager.GameStates.Label)
        {
            GameManager.instance.gameState = GameManager.GameStates.SubLabel;
            labelCanvas.SetActive(true);
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.instance.gameState == GameManager.GameStates.Label)
        {
            GameManager.instance.gameState = GameManager.GameStates.SubLabel;
            labelCanvas.SetActive(true);
        }
    }
}
