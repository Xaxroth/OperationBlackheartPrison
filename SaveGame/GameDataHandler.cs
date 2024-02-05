using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameDataHandler : MonoBehaviour
{
    public static GameDataHandler Instance;
    public string CurrentSceneName;
    public Vector3 playerPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {

    }

    void OnApplicationQuit()
    {
        SaveEssentialsData();
    }

    public void SaveEssentialsData()
    {
        CurrentSceneName = SceneManager.GetActiveScene().name;

        EssentialsData data = new EssentialsData
        {
            playerPosition = PlayerControllerScript.Instance.transform.position,
            playerRotation = PlayerControllerScript.Instance.transform.rotation,
            playerHealth = PlayerControllerScript.Instance.playerHealth,
            playerEnergy = PlayerControllerScript.Instance.playerStamina,
            altarUnlocked = MainRoomPuzzle.AltarUnlocked,
            fire1 = JumpTrialPuzzle.JumpTrialPassed,
            fire2 = FlashbangPuzzle.FlashbangPuzzlePassed,
            fire3 = SpikeTrapPuzzle.SpikeTrialPassed,

            natiaAffection = Natia.Instance.CurrentAffectionLevel,
            natiaState = Natia.Instance.CurrentEnemyState,
            natiaAffectionLevel = Natia.Instance.Affection,

            currentSceneName = CurrentSceneName,
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText("essentialsData.json", json);
    }

    public void LoadEssentialsData()
    {
        if (File.Exists("essentialsData.json"))
        {
            string json = File.ReadAllText("essentialsData.json");
            Debug.Log("you are a faking noobz mens... you a re a faking RETARCH... BASTAAAAARCH");

            EssentialsData data = JsonUtility.FromJson<EssentialsData>(json);

            MainRoomPuzzle.AltarUnlocked = data.altarUnlocked;
            MainRoomPuzzle.AltarUnlocked = data.fire1;

            PlayerControllerScript.Instance.transform.position = data.playerPosition;
            PlayerControllerScript.Instance.transform.rotation = data.playerRotation;
            PlayerControllerScript.Instance.playerHealth = data.playerHealth;
            PlayerControllerScript.Instance.playerStamina = data.playerEnergy;

            JumpTrialPuzzle.JumpTrialPassed = data.fire1;
            FlashbangPuzzle.FlashbangPuzzlePassed = data.fire2;
            SpikeTrapPuzzle.SpikeTrialPassed = data.fire3;

            CurrentSceneName = data.currentSceneName;

            SceneManager.LoadScene(CurrentSceneName);

            Debug.Log("Essentials data loaded. Player position: " + playerPosition + ", Current scene: " + CurrentSceneName);
        }
        else
        {
            Debug.Log("No essentials data found.");
        }
    }

    public void LoadGame()
    {
        StartCoroutine(FadeOutScreen());
    }

    public IEnumerator FadeOutScreen()
    {
        UIManager.Instance.FadeInScreen();
        yield return new WaitForSeconds(3);
        LoadEssentialsData();
        UIManager.Instance.FadeOutScreen();
    }

    [System.Serializable]
    class EssentialsData
    {
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public int playerHealth;
        public int playerEnergy;
        public string currentSceneName;

        public bool altarUnlocked;

        public bool fire1;
        public bool fire2;
        public bool fire3;
        public bool fire4;

        public Natia.NatiaState natiaState;
        public Natia.AffectionLevel natiaAffection;

        public int natiaAffectionLevel;
        // Add other essential data fields here
    }
}
