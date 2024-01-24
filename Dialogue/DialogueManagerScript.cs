using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManagerScript : MonoBehaviour
{
    public static DialogueManagerScript Instance;

    [SerializeField] private GameObject startDialogue;

    [SerializeField] private GameObject panel;

    [SerializeField] private TextMeshProUGUI DialogueBoxTMP;

    [SerializeField] private OptionsLetterByLetter _optionsLetterByLetterScript;

    [SerializeField] private Image _spriteImage;

    [SerializeField] private Sprite _sprite;

    [SerializeField] private bool isClosingDialogue;

    public LetterByLetter letterByLetterScript;

    public Color NatiaLetterColor;
    public Color PlayerLetterColor;
    public Color WarningLetterColor;

    public GameObject DialogueBox;
    public GameObject[] choiceBoxesButtons;
    public OptionsLetterByLetter[] choiceBoxes;

    public float DistanceToNatia = 0f;
    private float TargetAlpha = 1.0f;
    private float fadeOutProgress = 0.0f;
    private float fadeOutThreshold = 0.01f;

    public bool InProgress;
    public bool isInDialogue;
    private bool setEventFunction = false;
    private bool _inProgress;

    private int currentDialogueNode = 1;
    public int selectedOption;
    public int choicesToBeDisplayed;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DialogueBoxTMP = DialogueBox.GetComponent<TextMeshProUGUI>();
        letterByLetterScript = DialogueBox.GetComponent<LetterByLetter>();

        DontDestroyOnLoad(gameObject);
    }

    public void SetColor(Color color)
    {
        DialogueBoxTMP.color = color;
    }

    public void PlayDialogue(string textToBeDisplayed, int numberOfChoices, string choice1, string choice2, string choice3, string choice4)
    {
        panel.SetActive(true);

        choicesToBeDisplayed = numberOfChoices;

        DialogueBox.SetActive(false);

        DialogueBoxTMP.text = textToBeDisplayed;
        letterByLetterScript.fullText = textToBeDisplayed;

        choiceBoxes[0].fullText = choice1;
        choiceBoxes[1].fullText = choice2;
        choiceBoxes[2].fullText = choice3;
        choiceBoxes[3].fullText = choice4;

        DialogueBox.SetActive(true);
    }

    void Update()
    {
        if (panel.activeInHierarchy == true)
        {
            isInDialogue = true;
        }

        if (InProgress)
        {
            FadeInText(10f);
            DialogueBoxTMP.color = NatiaLetterColor;
        }
        else
        {
            FadeOutText(10f);
            DialogueBoxTMP.color = NatiaLetterColor;
        }
    }

    public void StartOfDialogue()
    {
        CheckDistance();

        InProgress = true;
        setEventFunction = false;

        for (int i = 0; i < choiceBoxes.Length; i++)
        {
            choiceBoxes[i].gameObject.SetActive(false);
            choiceBoxes[i].ButtonFunction.onClick.RemoveAllListeners();
        }
    }

    public void SetSprite(Sprite image)
    {
        _spriteImage.sprite = image;
    }

    public void EndOfDialogue()
    {
        InProgress = false;
        if (Natia.Instance != null)
        {
            Natia.Instance.InConversation = true;
        }

        for (int i = 0; i < choiceBoxes.Length; i++)
        {
            choiceBoxes[i].gameObject.SetActive(false);
            choiceBoxes[i].ButtonFunction.onClick.RemoveAllListeners();
        }
    }

    public void CloseDialogue()
    {
        currentDialogueNode = 0;

        isInDialogue = false;
        isClosingDialogue = false;
    }

    public void CheckDistance()
    {
        if (PlayerControllerScript.Instance != null && Natia.Instance != null) 
        {
            float Distance = Vector3.Distance(PlayerControllerScript.Instance.transform.position, Natia.Instance.transform.position);
        }
    }

    #region Events

    public void FadeInText(float lerpSpeed)
    {
        NatiaLetterColor.a = Mathf.Lerp(NatiaLetterColor.a, TargetAlpha, lerpSpeed * Time.deltaTime);
    }
    public void FadeOutText(float lerpSpeed)
    {
        float targetAlpha = 0.0f;
        NatiaLetterColor.a = Mathf.Lerp(NatiaLetterColor.a, targetAlpha, lerpSpeed * Time.deltaTime);

        fadeOutProgress = Mathf.Abs(NatiaLetterColor.a - targetAlpha);

        if (fadeOutProgress <= 0.01f && panel.activeInHierarchy)
        {
            NatiaLetterColor.a = targetAlpha;
            panel.SetActive(false);
            DialogueBox.SetActive(false);
            currentDialogueNode = 0;

            setEventFunction = false;
            isInDialogue = false;
            isClosingDialogue = false;
        }
    }

    public void ShowChoices()
    {
        for (int j = 0; j < choicesToBeDisplayed; j++)
        {
            choiceBoxes[j].gameObject.SetActive(true);
        }
    }

    public void NatiaChangedAffectionDialogue(Natia.AffectionLevel AffectionLevel)
    {
        StartOfDialogue();

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("NatiaPickedUp", 3.0f);
                SetColor(WarningLetterColor);
                SetSprite(_sprite);

                switch (AffectionLevel)
                {
                    case Natia.AffectionLevel.Enemy:
                        PlayDialogue("You know what, you bastard? I hate you. I fucking hate you, and I'm going to kill you if it's the last thing I do!", 1, null, null, null, null);
                        break;
                    case Natia.AffectionLevel.Rival:
                        PlayDialogue("You don't make this easy, Halicon. If you keep being an arsehole, I'll fucking leave you here to rot. Got it?!", 1, null, null, null, null);
                        break;
                    case Natia.AffectionLevel.Stranger:
                        PlayDialogue("I'm never asking The Guild for help ever again...", 1, null, null, null, null);
                        break;
                    case Natia.AffectionLevel.Acquaintance:
                        PlayDialogue("I guess you Half-Orcs aren't 'Half-Bad' after all.", 1, null, null, null, null);
                        break;
                    case Natia.AffectionLevel.Friend:
                        PlayDialogue("Maybe I was wrong about you. You're actually quite good at your job. I wish I had gotten someone like you sooner.", 1, null, null, null, null);
                        break;
                    case Natia.AffectionLevel.Partner:
                        PlayDialogue("I'm glad you came, Halicon. Without you, I don't think I would have made it this far. Thank you.", 1, null, null, null, null);
                        break;
                    case Natia.AffectionLevel.Lover:
                        PlayDialogue("Halicon... We need to talk.", 1, null, null, null, null);
                        break;
                }
                break;
            case 1:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void NatiaPickedUp()
    {
        StartOfDialogue();

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("NatiaPickedUp", 1.0f);
                SetColor(WarningLetterColor);
                SetSprite(_sprite);
                PlayDialogue("Hey! Let me go!", 1, null, null, null, null);
                break;
            case 1:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void NatiaDropped()
    {
        StartOfDialogue();

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("NatiaDropped", 2.0f);
                SetColor(WarningLetterColor);
                SetSprite(_sprite);
                PlayDialogue("Ouch! I'll kill you for that.", 1, null, null, null, null);
                break;
            case 1:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void NatiaDied()
    {
        StartOfDialogue();

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("NatiaDied", 5.0f);
                SetColor(WarningLetterColor);
                SetSprite(_sprite);
                PlayDialogue("Natia is dead. Her life snuffed out forever. How you proceed from here is up to you.", 1, null, null, null, null);
                break;
            case 1:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }
    public void NatiaFriendlyFireEvent()
    {
        StartOfDialogue();

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("NatiaFriendlyFireEvent", 2.0f);
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);

                int RandomNumber = Random.Range(1, 8);

                switch (RandomNumber)
                {
                    case 1:
                        PlayDialogue("Fuck! You're hurting me!", 1, null, null, null, null);
                        break;
                    case 2:
                        PlayDialogue("Nnngh! Halicon! You idiot!", 1, null, null, null, null);
                        break;
                    case 3:
                        PlayDialogue("My eyes! I can't... I can't see!", 1, null, null, null, null);
                        break;
                    case 4:
                        PlayDialogue("Ahh! What in the hells are you doing?!", 1, null, null, null, null);
                        break;
                    case 5:
                        PlayDialogue("You piece of shit!", 1, null, null, null, null);
                        break;
                    case 6:
                        PlayDialogue("Stop throwing those in my face!", 1, null, null, null, null);
                        break;
                    case 7:
                        PlayDialogue("You BASTARD!", 1, null, null, null, null);
                        break;
                }

                break;
            case 1:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void PlayerTooFarAway()
    {
        StartOfDialogue();

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("PlayerTooFarAway", 2.0f);
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);

                int RandomNumber = Random.Range(1, 5);

                Natia.Instance.CurrentEnemyState = Natia.NatiaState.Waiting;

                switch (RandomNumber)
                {
                    case 1:
                        PlayDialogue("Halicon?", 1, null, null, null, null);
                        break;
                    case 2:
                        PlayDialogue("Wait! Come back!", 1, null, null, null, null);
                        break;
                    case 3:
                        PlayDialogue("Halicon...? Where are you?", 1, null, null, null, null);
                        break;
                    case 4:
                        PlayDialogue("Hey! Don't just leave me here!", 1, null, null, null, null);
                        break;
                }

                break;
            case 1:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void PlayerComesBack()
    {
        StartOfDialogue();

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("PlayerComesBack", 2.0f);
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);

                int RandomNumber = Random.Range(1, 5);

                switch (RandomNumber)
                {
                    case 1:
                        PlayDialogue("Oh, there you are! Don't scare me like that.", 1, null, null, null, null);
                        break;
                    case 2:
                        PlayDialogue("Hello there. Did you find anything interesting?", 1, null, null, null, null);
                        break;
                    case 3:
                        PlayDialogue("Is it safe?", 1, null, null, null, null);
                        break;
                    case 4:
                        PlayDialogue("Great... I was just starting to enjoy the peace and quiet...", 1, null, null, null, null);
                        break;
                }

                break;
            case 1:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void NatiaLockpicking()
    {
        StartOfDialogue();

        Natia.Instance.CurrentEnemyState = Natia.NatiaState.Lockpicking;

        switch (currentDialogueNode)
        {
            // ENTRY POINT FOR DIALOGUE
            case 0:

                SetColor(NatiaLetterColor);
                SetSprite(_sprite);

                int RandomNumber = Random.Range(1, 4);

                switch (RandomNumber)
                {
                    case 1:
                        PlayDialogue("Ooh, there's something good on the other side of this one. I can tell.", 1, null, null, null, null);
                        //AudioManager.Instance.PlaySound(AudioManager.Instance.Lockpicking1, 1.0f);
                        break;
                    case 2:
                        PlayDialogue("Watch my back. I'll get this thing open.", 1, null, null, null, null);
                        //AudioManager.Instance.PlaySound(AudioManager.Instance.Lockpicking2, 1.0f);
                        break;
                    case 3:
                        PlayDialogue("This will only take a second.", 1, null, null, null, null);
                        //AudioManager.Instance.PlaySound(AudioManager.Instance.Lockpicking3, 1.0f);
                        break;
                    case 4:
                        PlayDialogue("Ooh, there's something good on the other side of this one. I can tell.", 1, null, null, null, null);
                        //AudioManager.Instance.PlaySound(AudioManager.Instance.Lockpicking4, 1.0f);
                        break;
                }
                Invoke("NatiaLockpicking", 1.0f);

                break;
            case 1:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void AdaptabilityDialogue()
    {
        StartOfDialogue();

        ShowChoices();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].gameObject.SetActive(true);
                choiceBoxes[i].ButtonFunction.onClick.AddListener(AdaptabilityDialogue);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            // ENTRY POINT FOR DIALOGUE
            case 0:
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);
                PlayDialogue("You need something? Or are you just wasting our time?", 4, "Can you keep your distance?", "Can you stick closer to me?", "Stay here.", "Let's go.");
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Question1, 1.0f);

                break;
            // PLAYERS FIRST CHOICE
            case 1:
                //AudioManager.Instance.GlobalAudioSource.Stop();
                switch (selectedOption)
                {
                    case 0:
                        break;
                    case 1:
                        PlayDialogue("What did you just...?! Fine. But you better keep an eye out for me.", 0, "[Leave]", null, null, null);
                        //AudioManager.Instance.PlaySound(AudioManager.Instance.KeepDistance, 1.0f);
                        Natia.Instance.CurrentEnemyState = Natia.NatiaState.Relaxed;
                        break;
                    case 2:
                        PlayDialogue("I'll stay as close as I can.", 0, "[Leave]", null, null, null);
                        //AudioManager.Instance.PlaySound(AudioManager.Instance.StayClose, 1.0f);
                        Natia.Instance.CurrentEnemyState = Natia.NatiaState.Cautious;
                        break;
                    case 3:
                        PlayDialogue("Alright. I hope you know what you're doing. You better come back for me.", 0, "[Leave]", null, null, null);
                        //AudioManager.Instance.PlaySound(AudioManager.Instance.HoldPosition, 1.0f);
                        Natia.Instance.CurrentEnemyState = Natia.NatiaState.Waiting;
                        break;
                    case 4:
                        PlayDialogue("Lead the way.", 0, "[Leave]", null, null, null);
                        //AudioManager.Instance.PlaySound(AudioManager.Instance.FollowMe, 1.0f);
                        Natia.Instance.CurrentEnemyState = Natia.NatiaState.Following;
                        break;
                }

                for (int i = 0; i < choiceBoxes.Length; i++)
                {
                    choiceBoxes[i].gameObject.SetActive(false);
                    choiceBoxes[i].ButtonFunction.onClick.RemoveAllListeners();
                }

                Invoke("AdaptabilityDialogue", 3.0f);
                break;
            case 2:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event1()
    {
        StartOfDialogue();

        Natia.Instance.CanMove = true;
        Natia.Instance.InConversation = true;

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].gameObject.SetActive(true);
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event1);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            // ENTRY POINT FOR DIALOGUE
            case 0:
                Invoke("Event1", 3.0f);
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);
                PlayDialogue("Halicon... I should have known that The Guild would send you of all people...", 4, null, null, null, null);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.RevealStinger, 1.0f);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue1x1, 1.0f);
                break;
            // PLAYERS FIRST CHOICE
            case 1:
                PlayDialogue("Nevermind... Supposedly there's an ancient treasure somewhere within this prison. You are going to help me find it. Got it?", 4, "Who are you?", "What are we looking for?", "You're coming with me.", null);
                UIManager.Instance.ShowHint("You can select the options shown on screen by pressing '1', '2', '3' or '4' respectively.");
                break;
            case 2:
                switch (selectedOption)
                {
                    case 0:
                        break;
                    case 1:
                        PlayDialogue("I'm Natia. I'm quite capable you'll come to find out, but it would be foolish to go down here without any backup. I just hate the fact that it had to be... you.", 0, null, null, null, null);
                        currentDialogueNode = 3;
                        Invoke("Event1", 5.0f);
                        break;
                    case 2:
                        PlayDialogue("None of your business. All you need to think about is protecting me, and you'll get your pay.", 1, "That's what I do.", "I can tell we're gonna be good friends.", null, null);
                        currentDialogueNode = 4;
                        break;
                    case 3:
                        PlayDialogue("Your manners are as vile as they say. Gods... why did it have to be you?", 2, "I won't repeat myself.", "It's better if I go first. Don't you think?", null, null);
                        break;
                }

                break;
            case 3:

                switch (selectedOption)
                {
                    case 0:
                        break;
                    case 1:
                        PlayDialogue("You bastard! What did I ever do to you?! Fine, but if you touch me, I'll kill you. Got it?", 0, null, null, null, null);
                        Invoke("Event1", 4.0f);
                        break;
                    case 2:
                        PlayDialogue("I guess I can't argue with that... Let's go, Halicon.", 0, null, null, null, null);
                        Invoke("Event1", 3.0f);
                        break;
                }
                break;
            case 4:
                EndOfDialogue();
                CloseDialogue();
                UIManager.Instance.ShowHint("You can talk to Natia by pressing the interact key. ('E' by default)");
                Natia.Instance.CurrentEnemyState = Natia.NatiaState.Following;
                break;
            case 5:
                switch (selectedOption)
                {
                    case 0:
                        break;
                    case 1:
                        PlayDialogue("Ugh, you're the worst, Halicon. Let's just go already.", 0, null, null, null, null);
                        currentDialogueNode = 3;
                        Invoke("Event1", 3.0f);
                        break;
                    case 2:
                        PlayDialogue("Listen, Halicon... Do you think I would let my guard down in the presence of a well-known, bloodthirsty war criminal?", 0, null, null, null, null);
                        Invoke("Event1", 4.0f);
                        break;
                }
                break;
            case 6:
                PlayDialogue("You're going to do as I say, or you'll get us both killed. Got it? You go on ahead, and I'll be right behind you.", 0, null, null, null, null);
                Invoke("Event1", 3.0f);
                break;
            case 7:
                EndOfDialogue();
                CloseDialogue();
                UIManager.Instance.ShowHint("You can talk to Natia by pressing the interact key. ('E' by default)");
                Natia.Instance.CurrentEnemyState = Natia.NatiaState.Following;
                break;


        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event2()
    {
        if (Natia.Instance.CurrentEnemyState == Natia.NatiaState.Waiting)
        {
            return;
        }

        StartOfDialogue();

        switch (currentDialogueNode)
        {
            // ENTRY POINT FOR DIALOGUE
            case 0:
                Invoke("Event2", 3.0f);
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);
                PlayDialogue("It's so dark in here... I can't see a thing.", 2, null, null, null, null);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue2x1, 1.0f);
                break;
            // PLAYERS FIRST CHOICE
            case 1:
                // WHAT OPTION DID THE PLAYER SELECT?
                Invoke("Event2", 3.0f);
                PlayDialogue("Maybe we should turn back? I'm not going.", 2, null, null, null, null);
                UIManager.Instance.ShowHint("Natia can be picked up by pressing 'E' while in conversation with her.");
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue2x2, 1.0f);
                break;
            // PLAYERS SECOND CHOICE
            case 2:
                EndOfDialogue();
                CloseDialogue();
                Natia.Instance.CurrentEnemyState = Natia.NatiaState.Waiting;
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event3()
    {
        if (Natia.Instance.CurrentEnemyState == Natia.NatiaState.Waiting)
        {
            return;
        }

        StartOfDialogue();

        switch (currentDialogueNode)
        {
            // ENTRY POINT FOR DIALOGUE
            case 0:
                Invoke("Event3", 5.0f);
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);
                PlayDialogue("This place is awfully quiet... Almost too quiet.", 0, null, null, null, null);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue2x1, 1.0f);
                break;
            // PLAYERS FIRST CHOICE
            case 1:
                // WHAT OPTION DID THE PLAYER SELECT?
                Invoke("Event3", 5.0f);
                PlayDialogue("How long has this place been dormant?", 0, null, null, null, null);
                UIManager.Instance.ShowHint("Being a thief, Natia will try her best to pick the locks of chests and doors.");
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue2x2, 1.0f);
                break;
            // PLAYERS SECOND CHOICE
            case 2:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event4()
    {
        if (Natia.Instance.CurrentEnemyState == Natia.NatiaState.Waiting)
        {
            return;
        }

        StartOfDialogue();

        switch (currentDialogueNode)
        {
            // ENTRY POINT FOR DIALOGUE
            case 0:
                Invoke("Event4", 7.0f);
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);
                PlayDialogue("Explosives! Well, they can certainly come in handy... As long as we're careful.", 0, null, null, null, null);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue4x1, 1.0f);
                break;
            // PLAYERS FIRST CHOICE
            case 1:
                // WHAT OPTION DID THE PLAYER SELECT?
                Invoke("Event4", 5.0f);
                PlayDialogue("Why are you giving me that look? Don't even think about it.", 0, null, null, null, null);
                UIManager.Instance.ShowHint("You now have access to Flashbangs. Use the 'T' key to charge, and release to throw. Use them wisely.");
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue4x2, 1.0f);
                break;
            // PLAYERS SECOND CHOICE
            case 2:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event5()
    {
        if (Natia.Instance.CurrentEnemyState == Natia.NatiaState.Waiting)
        {
            return;
        }

        StartOfDialogue();

        switch (currentDialogueNode)
        {
            // ENTRY POINT FOR DIALOGUE
            case 0:
                Invoke("Event5", 3.0f);
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);
                PlayDialogue("These rooms... they all seem like trials of some sort. I wonder why?", 0, null, null, null, null);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue5x1, 1.0f);
                break;
            // PLAYERS FIRST CHOICE
            case 1:
                // WHAT OPTION DID THE PLAYER SELECT?
                Invoke("Event5", 4.0f);
                PlayDialogue("I mean, look there. I've never seen so many traps in one place.", 0, null, null, null, null);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue5x2, 1.0f);
                break;
            // PLAYERS SECOND CHOICE
            case 2:
                // WHAT OPTION DID THE PLAYER SELECT?
                Invoke("Event5", 5.0f);
                PlayDialogue("I think it's time you earned your pay. I'll stay here. And keep your hands off of me.", 0, null, null, null, null);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue5x3, 1.0f);
                break;
            case 3:
                EndOfDialogue();
                CloseDialogue();
                Natia.Instance.CurrentEnemyState = Natia.NatiaState.Waiting;
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event6()
    {
        if (Natia.Instance.CurrentEnemyState == Natia.NatiaState.Waiting)
        {
            return;
        }

        StartOfDialogue();

        switch (currentDialogueNode)
        {
            // ENTRY POINT FOR DIALOGUE
            case 0:
                Invoke("Event6", 3.0f);
                SetColor(NatiaLetterColor);
                SetSprite(_sprite);
                PlayDialogue("Halicon... The braziers, they're all lit.", 0, null, null, null, null);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue5x1, 1.0f);
                break;
            // PLAYERS FIRST CHOICE
            case 1:
                // WHAT OPTION DID THE PLAYER SELECT?
                Invoke("Event6", 8.0f);
                PlayDialogue("You knew what to do all along, didn't you? Hmmph... You're a bastard, but at least you're capable.", 0, null, null, null, null);
                //AudioManager.Instance.PlaySound(AudioManager.Instance.Dialogue5x3, 1.0f);
                break;

            case 2:
                // WHAT OPTION DID THE PLAYER SELECT?
                Invoke("Event6", 5.0f);
                PlayDialogue("I guess the only way forward is... down. What are we waiting for?", 0, null, null, null, null);
                break;
            case 3:
                EndOfDialogue();
                CloseDialogue();
                Natia.Instance.CurrentEnemyState = Natia.NatiaState.Waiting;
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    #endregion
}
