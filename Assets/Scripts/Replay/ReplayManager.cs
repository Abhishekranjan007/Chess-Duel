using System.Collections.Generic;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance;

    public List<ReplayMove> ReplayMoves = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ClearReplay()
    {
        ReplayMoves.Clear();
    }
}