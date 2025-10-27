using UnityEngine;

public class RescueBoatSeatsManager : MonoBehaviour
{
    [Header("NPC Seats")]
    [SerializeField] private Transform[] npcSeats;

    [Header("Pet Seats (Max 2)")]
    [SerializeField] private Transform[] petSeats;

    private bool[] npcSeatOccupied;
    private bool[] petSeatOccupied;

    void Awake()
    {
        npcSeatOccupied = new bool[npcSeats.Length];
        petSeatOccupied = new bool[petSeats.Length];
    }

    // NPC seats only
    public Transform GetFreeNPCSeat()
    {
        for (int i = 0; i < npcSeats.Length; i++)
        {
            if (!npcSeatOccupied[i])
            {
                npcSeatOccupied[i] = true;
                return npcSeats[i];
            }
        }
        return null;
    }

    // Pet seats only
    public Transform GetFreePetSeat()
    {
        for (int i = 0; i < petSeats.Length; i++)
        {
            if (!petSeatOccupied[i])
            {
                petSeatOccupied[i] = true;
                return petSeats[i];
            }
        }
        return null;
    }

    public void FreeNPCSeats()
    {
        for (int i = 0; i < npcSeatOccupied.Length; i++)
            npcSeatOccupied[i] = false;
    }

    public void FreePetSeats()
    {
        for (int i = 0; i < petSeatOccupied.Length; i++)
            petSeatOccupied[i] = false;
    }

    public void FreeAllSeats()
    {
        FreeNPCSeats();
        FreePetSeats();
    }
}
