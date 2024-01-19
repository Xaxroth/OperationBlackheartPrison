using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    // The script for the on-screen keyboard buttons.

    [SerializeField] private TextMeshProUGUI _letter;
    [SerializeField] private string _letterText;

    [SerializeField] private char[] _allChars;

    [SerializeField] public char _letterChar;

    [SerializeField] private int _letterNumber;

    private bool _correct;

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioClip _inputSound;
    [SerializeField] private WordClass _wordClass;

    private void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        _gameManager.keyboardButtons.Add(_letterText, this);

    }

    void Start() // Sets the keyboard button's char to be equal to that of the first letter of the Text asset's text. Used to compare against character inputs made by the player.
    {
        _letter = GetComponentInChildren<TextMeshProUGUI>();

        if (_letterText != null)
        {
            _letter.text = _letterText;
        }

        _allChars = _letterText.ToCharArray();

        _letterChar = _allChars[0];
    }

    public void ManualCharacterInput() // Translates the button's letter data into the wordguess array of characters.
    {
        _gameManager.EnterKey(_letterChar);

        _gameManager.managerAudioSource.PlayOneShot(_inputSound);
    }

    public void SetColor(int value)
    {
        switch (value)
        {
            case 0:
                gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                break;
            case 1:
                if (!_correct)
                {
                    gameObject.GetComponent<Image>().color = new Color32(255, 220, 90, 255);
                }
                break;
            case 2:
                gameObject.GetComponent<Image>().color = new Color32(80, 255, 70, 255);
                _correct = true;
                break;
        }
    }
}
