
using Photon.Pun;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public static class PhotonNetworkUtil
    {
        public static T Instantiate<T>(T prefab) where T : Object
        {
            var clone = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
            return clone.GetComponent<T>();
        }
    }
}

