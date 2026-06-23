using System;
using Unity.Mathematics;
using UnityEngine;

public class HealItem : MonoBehaviour,iItem
{
    [SerializeField] int healAmount = 30;
    [SerializeField] GameObject healEffect;

    public void  GetItem( PlayerMainBahvior _player)
    {

        if (_player.HP == 100)
        {
            Destroy(gameObject);
        }

        else
        {

            if (_player.HP + healAmount > 100)
            {
                _player.HP = 100;
            }
            else
            {
                _player.HP += healAmount;
            }

            _player.PlayerUI.HPUI(_player.HP,false);

            Destroy(Instantiate( healEffect , transform.position+ new Vector3(0,0,-1), Quaternion.identity),2f);
            Destroy(gameObject);
        }
    }
}
