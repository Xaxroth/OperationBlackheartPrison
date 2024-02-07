using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PitDoor : MonoBehaviour
{
    public Scene SceneToLoad;
    public string SceneToLoadString;
    public Transform SpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LoadScene());
        }
    }

    private IEnumerator LoadScene()
    {
        UIManager.Instance.FadeInScreen();
        yield return new WaitForSeconds(3);
        UIManager.Instance.SetTransform(SpawnPosition);
        SceneManager.LoadScene(SceneToLoadString);
    }
}
