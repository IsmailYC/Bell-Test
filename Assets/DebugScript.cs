using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.instance != null);
        Debug.Log(gameObject.name);
        Debug.Log(GameManager.instance.Position2PupilLab(transform.position).ToString("f5"));
        Debug.Log(Camera.main.WorldToScreenPoint(transform.position).ToString("f5"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
