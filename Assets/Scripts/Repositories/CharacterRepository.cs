using System.Collections.Generic;
using UnityEngine;

public static class CharacterRepository {
    private static List<int> characters = new();

    static CharacterRepository () {
        for (int i = 0; i< 4;i++) {
            characters.Add(-1);
        }
    }

    public static int AddCharacter(int photonViewID) {
        for (int i = 0; i < characters.Count; i++) {
            if(characters[i] == -1) {
                Debug.LogError($"Set Character {photonViewID}: {i}");
                characters[i] = photonViewID;
                return i;
            }
        }

        return -1;
    }

    public static void RemoveCharacter(int index) {
        Debug.LogError($"Remove Character: {index}");
        characters[index] = -1;
    }
}