using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterScript : MonoBehaviour
{
    // This is attached to every square in the game space, set up into a grid by the GameManager. Handles the info regarding what letter the box has storage wise, as well as the visuals.

    [SerializeField] private char? Letter { get; set; } = null;
    public StateHandler stateHandler { get; set; } = StateHandler.Default;

    public GameObject _default;

    public Text LetterText = null;

    public Image letterBackground;

    public Animator letterAnimator;

    private Color _reset = new Color32(75, 75, 75, 255);
    private Color _wrong = new Color32(35, 35, 35, 255);
    private Color _close = new Color32(255, 220, 90, 255);
    private Color _correct = new Color32(80, 255, 70, 255);

    [SerializeField] private AudioClip[] _inputLetter;
    [SerializeField] private AudioClip _deleteLetter;

    [SerializeField] private GameManager _gameManager;

    public void Awake()
    {
        LetterText = GetComponentInChildren<Text>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void InputLetter(char c)
    {
        Letter = c;
        LetterText.text = c.ToString().ToUpper();
        StartCoroutine(ExpandCoroutine());
    }

    public void DeleteLetter()
    {
        _gameManager.managerAudioSource.PlayOneShot(_deleteLetter);
        Letter = null;
        LetterText.text = null;
    }

    public void SetLetterState(StateHandler state) // Sets the state for the letterbox, as well as the visual indication for the player.
    {
        stateHandler = state;

        //StartCoroutine(FlipLetter());
    }

    public void ClearAll() // Sets the state of the current letterbox to default and removes any stored string/text data.
    {
        stateHandler = StateHandler.Default;

        if (_gameManager.keyboardButtons.ContainsKey(LetterText.text))
        {
            _gameManager.keyboardButtons[LetterText.text].SetColor(0);
        }

        Letter = null;
        LetterText.text = null;
    }

    private IEnumerator FlipLetter()
    {
        letterAnimator.SetBool("Flip", true);
        yield return new WaitForSeconds(0.4f);

        switch (stateHandler)
        {
            case StateHandler.Default:

                letterBackground.color = _wrong; // Changes the background color of the letters to their appropriate color.

                break;
            case StateHandler.WrongLocation:

                letterBackground.color = _close;

                _gameManager.keyboardButtons[LetterText.text].SetColor(1); // Takes in the letterbox's letter and uses it as a key to access the corresponding keyboard button.

                break;

            case StateHandler.Correct:

                letterBackground.color = _correct;

                _gameManager.keyboardButtons[LetterText.text].SetColor(2);

                break;

            case StateHandler.Reset:

                letterBackground.color = _reset;

                break;
        }

        letterAnimator.SetBool("Flip", false);
    }

    private IEnumerator ExpandCoroutine()
    {
        letterAnimator.SetTrigger("Impact");
        yield return new WaitForSeconds(0.1f);
    }
}
