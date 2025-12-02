using UnityEngine;

public class SetGameKeyOnStart : MonoBehaviour
{
    [Tooltip("Leaderboard game key for this scene")] public string gameKey = "Joe_Rescuing";

    private void Awake()
    {
        GameData.gameName = gameKey;
    }
}
