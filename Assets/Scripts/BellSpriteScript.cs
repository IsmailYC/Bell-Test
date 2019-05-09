using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BellSpriteScript : MonoBehaviour
{
    public Sprite[] sprites;
    public float countDownTime;

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StreamWriter writer = File.CreateText(GameManager.instance.dataPath+ "/Bells Location.csv");
        writer.WriteLine("Index;X;Y");
        Transform[] bellTransforms = GetComponentsInChildren<Transform>();
        for(int i=1; i<bellTransforms.Length; i++)
        {
            writer.WriteLine((i-1).ToString()+";"+GameManager.instance.X_2_PupilLab(bellTransforms[i].position.x).ToString()+";"+ GameManager.instance.Y_2_PupilLab(bellTransforms[i].position.y).ToString());
            Destroy(bellTransforms[i].gameObject);
        }
        writer.Flush();
        writer.Close();
        Invoke("ShowBells", countDownTime);
    }


    public void ShowBells()
    {
        spriteRenderer.sprite = sprites[1];
    }

    public void ShowFixation()
    {
        spriteRenderer.sprite = sprites[0];
        Invoke("ShowBells", countDownTime);
    }
}
