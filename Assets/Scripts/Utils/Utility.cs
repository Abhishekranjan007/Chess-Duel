using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utility : MonoBehaviour
{
    

    //Check if available moves are valid or not
    public static bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }

        return false;
    }
}
