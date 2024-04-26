using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackheartBridgePuzzleManager : MonoBehaviour
{
    public GameObject Note;
    public GameObject BurntNote;
    void Start()
    {
        if (UIManager.Instance.TimeElapsed > 600)
        {
            BurntNote.SetActive(true);
        }
        else
        {
            Note.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
