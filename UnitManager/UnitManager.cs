using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public List<Unit> allUnits = new List<Unit>();

    public List<Unit> playerTeam = new List<Unit>();
    public List<Unit> enemyTeam = new List<Unit>();

    [Header("Class Data")]
    public UnitClassData archerData;
    public UnitClassData cavalierData;
    public UnitClassData dragonRiderData;
    public UnitClassData fighterData;
    public UnitClassData mageData;
    public UnitClassData priestData;
    public UnitClassData warlockData;

    [Header("Promoted Class Data")]
    public UnitClassData sniperData;
    public UnitClassData paladinData;
    public UnitClassData dragonKnightData;
    public UnitClassData heroData;
    public UnitClassData sageData;
    public UnitClassData bishopData;
    public UnitClassData demonData;

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
}
