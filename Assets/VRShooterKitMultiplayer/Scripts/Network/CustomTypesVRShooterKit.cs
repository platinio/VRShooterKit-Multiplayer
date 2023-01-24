using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using VRShooterKit.WeaponSystem;


namespace VRShooterKit.Multiplayer
{
    public class CustomTypesVRShooterKit : MonoBehaviour
    {
        public static readonly byte[] memVector3 = new byte[3 * 4];

        private static short SerializeQueueTransformInfo(StreamBuffer outStream, object customobject)
        {
            Queue<TransformInfo> vo = (Queue<TransformInfo>) customobject;

            int index = 0;
            lock (memVector3)
            {
                byte[] bytes = new byte[vo.Count * 10 * 4];

                foreach (var item in vo)
                {
                    Protocol.Serialize(item.position.x, bytes, ref index);
                    Protocol.Serialize(item.position.y, bytes, ref index);
                    Protocol.Serialize(item.position.z, bytes, ref index);
                    Protocol.Serialize(item.rotation.w, bytes, ref index);
                    Protocol.Serialize(item.rotation.x, bytes, ref index);
                    Protocol.Serialize(item.rotation.y, bytes, ref index);
                    Protocol.Serialize(item.rotation.z, bytes, ref index);
                    Protocol.Serialize(item.dir.x, bytes, ref index);
                    Protocol.Serialize(item.dir.y, bytes, ref index);
                    Protocol.Serialize(item.dir.z, bytes, ref index);
                }
                
                
                outStream.Write(bytes, 0, 3 * 4);
            }

            return 3 * 4;
        }

        private static object DeserializeVector3(StreamBuffer inStream, short length)
        {
            Vector3 vo = new Vector3();
            lock (memVector3)
            {
                inStream.Read(memVector3, 0, 3 * 4);
                int index = 0;
                Protocol.Deserialize(out vo.x, memVector3, ref index);
                Protocol.Deserialize(out vo.y, memVector3, ref index);
                Protocol.Deserialize(out vo.z, memVector3, ref index);
            }

            return vo;
        }
    }

}

