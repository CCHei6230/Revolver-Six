using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
   public bool playerInRange;
   public Transform player;
   private void OnTriggerStay2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         playerInRange = true;
         player = other.transform;
      }
   }

   private void OnTriggerExit2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         playerInRange = false;
      }
   }
}
