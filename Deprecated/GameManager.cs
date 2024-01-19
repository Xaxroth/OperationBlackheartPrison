using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // The game manager script - handles all data structures and game logistics. Also handles win conditions and input from the player.

    public static GameManager Instance { get; set; }

    [Header("Logistics")]

    [SerializeField] const int _numberOfLettersPerWord = 10;
    [SerializeField] private int _numberOfRows = 5;

    [SerializeField] private int _numberOfKeyboardButtons = 26;

    [SerializeField] private int _index = 0;
    [SerializeField] private int _currentRow = 0;

    [SerializeField] private LetterScript _letterBoxPrefab;
    [SerializeField] private GridLayoutGroup _grid = null;
    [SerializeField] private WordClass _wordClass;

    [SerializeField] private bool _gameOver;

    public List<LetterScript> allLetters = null; // The list of all letter boxes

    public Dictionary<string, ButtonScript> keyboardButtons = new Dictionary<string, ButtonScript>(); // Dictionary of all keyboard buttons. Each button can be accessed through a string.

    public char?[] wordGuess = new char?[_numberOfLettersPerWord];

    [Header("Audio")]

    [SerializeField] public AudioSource managerAudioSource;

    [SerializeField] private AudioClip _wrongWord;
    [SerializeField] private AudioClip _victory;
    [SerializeField] private AudioClip _insufficientLetters;
    [SerializeField] private AudioClip _lose;

    [Header("Error Messages")]

    [SerializeField] private GameObject _ErrorMessageWordTooShort;
    [SerializeField] private GameObject _ErrorMessageWordDoesNotExist;
    [SerializeField] private GameObject _VictoryScreen;
    [SerializeField] private GameObject _LoseScreen;

    private void Awake()
    {
        InitializeGame();
        _ErrorMessageWordTooShort.SetActive(false);
        _ErrorMessageWordDoesNotExist.SetActive(false);
        _VictoryScreen.SetActive(false);
    }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown && !_gameOver) // Checks for player inputs during runtime, and translates whatever input entered.
        {
            if (Phone.Instance.CurrentAppState != Phone.CurrentApp.Call)
            {
                TranslateInput(Input.inputString);
            }
        }
    }

    public void TranslateInput(string keyInput)
    {
        foreach(char c in keyInput)
        {
            if (c == '\b') // Backspace deletes the letter in the current slot.
            {
                DeleteKey();
            }
            else
            {
                if (c != '\b' && c != '\n' && c != '\r' && c != ' ') // As long as the input is not Enter, Return, Newline, Space the key pressed is added as a part of the list.
                {
                    EnterKey(c);
                }
                else
                {
                    if (_index == _numberOfLettersPerWord) // If the index is at the last slot of the word (the word's total length), it will be a valid guess. If not, an error message appears.
                    {
                        Guess();
                    }
                    else
                    {
                        StartCoroutine(WordTooShort());
                    }
                }
            }
        }
    }

    public void ManualTranslateInput() // The on-screen keyboard submit function - runs on clicking the "Submit" key on the screen. Works the same as submitting through keyboard.
    {
        if (_index == _numberOfLettersPerWord) // If the index is at the last slot of the word (the word's total length), it will be a valid guess. If not, an error message appears.
        {
            Guess();
        }
        else
        {
            StartCoroutine(WordTooShort());
        }
    }

    private void InitializeGame() // Runs on awake and establishes the grid based on the number of rows and word length. Standard is 5 rows with a word length of 5 characters.
    {
        if (allLetters == null)
        {
            allLetters = new List<LetterScript>(); 
        }

        for (int i = 0; i < _numberOfRows; i++) // Initilalizes rows, and then for each row it makes letter boxes according to the total length of the words.
        {
            for (int j = 0; j < _numberOfLettersPerWord; j++)
            {
                LetterScript letterBox = Instantiate(_letterBoxPrefab);
                letterBox.transform.SetParent(_grid.transform);
                allLetters.Add(letterBox);
            }
        }
    }

    public void EnterKey(char c)
    {
        if (_index < _numberOfLettersPerWord && !_gameOver) // As long as there are not 5 characters present, the player can input letters. For each letter entered it moves forward in the current row index.
        {
            c = char.ToUpper(c); // Converts whatever input is given into upper-case letters so that the string comparison matches. All words in the wordlist are uppercase, so the input must match.

            allLetters[(_currentRow * _numberOfLettersPerWord) + _index].InputLetter(c); // Changes the text value of the current letter box to the char given.
            wordGuess[_index] = c;
            _index++;
        }
    }

    public void DeleteKey()
    {
        if (_index > 0) // As long as the index is greater than 0, we move back in the row. We clear any visual text and set the slot to be null.
        {
            _index--;
            allLetters[(_currentRow * _numberOfLettersPerWord) + _index].DeleteLetter();
            wordGuess[_index] = null;
        }
    }

    public void Guess()
    {
        StringBuilder word = new StringBuilder();

        for (int i = 0; i < _numberOfLettersPerWord; i++) // Puts the current row's characters into a stringbuilder which is then converted to a string for comparison.
        {
            word.Append(wordGuess[i]);
        }

        if (word.ToString() == _wordClass._secretNumber)
        {
            managerAudioSource.PlayOneShot(_victory);
            return;
        }

        if (_wordClass._wordList.Contains(word.ToString())) // The guess is a 5 letter word that exists in the wordlist.
        {
            for (int i = 0; i < _numberOfLettersPerWord; i++)
            {

                bool correct = wordGuess[i] == _wordClass.wordToBeGuessed[i]; // Returns true if the character's position in the word is equal to that of the word to be guessed. The bool changes the letterbox background to green.

                if (!correct) // If the letter is not in the correct spot, a second check is made to see if the letter exists at all in the current row.
                {
                    bool letterExists = false;

                    for (int j = 0; j < _numberOfLettersPerWord; j++)
                    {
                        letterExists = wordGuess[i] == _wordClass.wordToBeGuessed[j]; // Returns true if the character's position in the word is wrong, but exists somewhere in the word to be guessed. The bool changes the letterbox background to yellow.

                        if (letterExists)
                        {
                            allLetters[i + (_currentRow * _numberOfLettersPerWord)].SetLetterState(StateHandler.WrongLocation);
                            break;
                        }
                        else // The letter entered is not a part of the word at all.
                        {
                            allLetters[i + (_currentRow * _numberOfLettersPerWord)].SetLetterState(StateHandler.Default); 
                        }
                    }
                }
                else // If the defined "correct" bool returns true, we automatically set the state as correct for that letter.
                {
                    allLetters[i + (_currentRow * _numberOfLettersPerWord)].SetLetterState(StateHandler.Correct);
                }
            }

            if (word.ToString().Equals(_wordClass.wordToBeGuessed) && _wordClass.wordToBeGuessed != null) // Compiles the array of characters entered into a string and compares it to the word generated from the wordlist. If it matches, the player has correctly guessed the word and won the game.
            {
                managerAudioSource.PlayOneShot(_victory);
                _VictoryScreen.SetActive(true);
                _gameOver = true;
            }
            else
            {
                managerAudioSource.PlayOneShot(_wrongWord);
            }

            if (_currentRow < _numberOfRows) // With every guess the player makes, it shifts to the next row and resets the index as we only need to check the current row for conditions.
            {
                _currentRow++;
                _index = 0;
            }
            else
            {
                managerAudioSource.PlayOneShot(_victory);
                return;
            }

            if (_currentRow == _numberOfRows && !_gameOver) // If the player runs out of guesses, they lose the game.
            {
                managerAudioSource.PlayOneShot(_lose);
                _LoseScreen.SetActive(true);
                _currentRow = 0;
                _gameOver = true;
            }
        }
        else
        {
            StartCoroutine(WordDoesNotExist());
            return;
        }
    }

    private IEnumerator WordTooShort()
    {
        managerAudioSource.PlayOneShot(_insufficientLetters);
        _ErrorMessageWordTooShort.SetActive(true);
        yield return new WaitForSeconds(2);
        _ErrorMessageWordTooShort.SetActive(false);
    }

    private IEnumerator WordDoesNotExist()
    {
        managerAudioSource.PlayOneShot(_insufficientLetters);
        _ErrorMessageWordDoesNotExist.SetActive(true);
        yield return new WaitForSeconds(2);
        _ErrorMessageWordDoesNotExist.SetActive(false);
    }

    public void ResetGame() // Resets all input data and starts a new game session with a new word.
    {
        _LoseScreen.SetActive(false);
        _VictoryScreen.SetActive(false);

        _wordClass.Retry(); // Calls the retry method of the wordClass script which picks a new word from the word list to be guessed. It can be the same as the previous word.

        for (int i = 0; i < allLetters.Count; i++) // Clears all boxes of information - current index is counted down on DeleteKey() until index reaches 0.
        {
            allLetters[i].ClearAll();
            allLetters[i].SetLetterState(StateHandler.Reset);
            DeleteKey();
        }

        _currentRow = 0;

        _gameOver = false;
    }
}
