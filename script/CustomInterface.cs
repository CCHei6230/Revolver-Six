public interface iCanDamegePlayer
{
    void DealDamageToPlayer(ref int _playerHP);
}
public interface iCanDamegeEnemy
{
    void DealDamageToEnemy(ref int _enemyHP);
}
public interface iItem
{
    void GetItem( PlayerMainBahvior _player);
}
