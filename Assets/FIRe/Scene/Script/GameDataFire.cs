using UnityEngine;

// Stores persistent info between gameplay and result canvas
public static class GameDataFire
{
    public static string objective = "Extinguish all fires!";
    public static int firesExtinguished; // Number of fires the player put out
    public static int totalFires;        // Total fires in the level
    public static float elapsedTime;     // How fast the player finished
    public static int finalScore;        // Score based on performance
    public static int starsEarned;       // Number of stars earned (0-3)
}
