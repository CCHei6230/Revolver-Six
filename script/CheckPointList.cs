using UnityEngine;

public class CheckPointList : MonoBehaviour
{
    public GameObject[] checkPoints;

    public int CheckPointSlot(GameObject _checkPoint )
    {
        for (int i = 0; i < checkPoints.Length; i++)
        {
            if (_checkPoint == checkPoints[i])
            {
                return i;
            }
        }
        return 0;
    }

}
