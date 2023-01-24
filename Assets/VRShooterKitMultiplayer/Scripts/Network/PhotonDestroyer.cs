using System.Collections.Generic;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    //This scripts just destroy components in case this vr avatar doesn't bellow to the current player
    public class PhotonDestroyer : MonoBehaviour
    {
        [SerializeField] private List<Object> destroyIfNoMineList = null;
       
        public void DestroyUnnecessaryComponents()
        {
            foreach (var obj in destroyIfNoMineList)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            
            Destroy(gameObject);
        }
    }

}

