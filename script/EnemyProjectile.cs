using UnityEngine;

public class EnemyProjectile :ProjectileBase , iCanDamegePlayer
{

    public void DealDamageToPlayer(ref int _playerHP )
    {
        _playerHP -= damage;
    }
    void Start()
    {
        base.Start();
    }
    void Update()
    {
        base.Update();
    }

}
