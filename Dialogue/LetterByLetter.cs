using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterByLetter : MonoBehaviour
{
    public float delay = 0.1f;
    public string fullText;
    public string currentText = "";

    public bool _dialogueFinished;

    TextMeshProUGUI TMP;
    float currentTime;

    private void Awake()
    {
        TMP = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        _dialogueFinished = false;

        StartCoroutine(TextSoundTest());

        if (!_dialogueFinished)
        {
            for (int i = 0; i < (fullText.Length + 1); i++)
            {
                if (_dialogueFinished)
                {
                    break;
                }
                currentText = fullText.Substring(0, i);
                TMP.text = currentText;
                yield return new WaitForSeconds(delay);

                if (i == fullText.Length)
                {
                    _dialogueFinished = true;
                    currentText = fullText;

                    for (int j = 0; j < DialogueManagerScript.Instance.choicesToBeDisplayed; j++)
                    {
                        DialogueManagerScript.Instance.choiceBoxes[j].gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    private IEnumerator TextSoundTest()
    {
        yield return new WaitForSeconds(3);
        if (!_dialogueFinished)
        {
            StartCoroutine(TextSoundTest());
        }
    }
}
