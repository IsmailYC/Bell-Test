using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrawingScript : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject labelStateCanvas;

    private GameObject currentLine;
    private LineHolderScript lineScript;
    private float previousTime;
    // Start is called before the first frame update
    void Start()
    {
        previousTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        if (GameManager.instance.gameState != GameManager.GameStates.Main && GameManager.instance.gameState != GameManager.GameStates.Check)
            return;
        if (Time.time - previousTime > 0.5f)
        {
            currentLine = Instantiate(linePrefab);
            currentLine.transform.parent = transform;
            lineScript = currentLine.GetComponent<LineHolderScript>();
            lineScript.InitializeReferences();
            Vector3 startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosition.z = 0;
            lineScript.InitializeLine(startPosition);
        }
        else
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0;
            lineScript.AddPosition(currentPosition);
        }
    }

    private void OnMouseDrag()
    {
        if (GameManager.instance.gameState != GameManager.GameStates.Main && GameManager.instance.gameState != GameManager.GameStates.Check)
            return;
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPosition.z = 0;
        lineScript.AddPosition(currentPosition);
    }

    public void DisableInteraction()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
