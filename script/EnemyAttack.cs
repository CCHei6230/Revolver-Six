using UnityEngine;

public class EnemyAttack : MonoBehaviour , iCanDamegePlayer
{

    public void DealDamageToPlayer(ref int _playerHP )
    {
        _playerHP -= damage;
    }
    [SerializeField]protected int damage;
}
