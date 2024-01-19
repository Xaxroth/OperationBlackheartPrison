using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveGame
{
    public static void SavePlayer(PlayerControllerScript Player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.saveFile";
        FileStream stream = new FileStream(path, FileMode.Create);
    }
}
    
