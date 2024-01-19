using UnityEngine;
using UnityEngine.UI;

public class ButtonChoice : MonoBehaviour
{
    [SerializeField] private Button button1;
    [SerializeField] private int choiceNumber;
    [SerializeField] private KeyCode choiceKey;

    void Awake()
    {
        button1 = GetComponent<Button>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(choiceKey))
        {
            DialogueManagerScript.Instance.selectedOption = choiceNumber;
            button1.onClick.Invoke();
        }
    }
}
