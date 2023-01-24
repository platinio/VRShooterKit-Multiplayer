// ----------------------------------------------------------------------------
// <copyright file="PhotonRigidbodyView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize rigidbodies via PUN.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using UnityEngine;


    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Photon Networking Extended/Photon Rigidbody View Extended")]
    public class PhotonRigidbodyViewExtended : MonoBehaviourPun, IPunObservable
    {
        private float m_Distance;
        private float m_Angle;

        private Rigidbody m_Body;

        private Vector3 m_NetworkPosition;

        private Quaternion m_NetworkRotation;

        [HideInInspector]
        public bool m_SynchronizeVelocity = true;
        [HideInInspector]
        public bool m_SynchronizeAngularVelocity = false;

        [HideInInspector]
        public bool m_TeleportEnabled = false;
        [HideInInspector]
        public float m_TeleportIfDistanceGreaterThan = 3.0f;

        [HideInInspector] 
        public bool m_SynchronizeIsKinematic = false;
        [HideInInspector] 
        public bool m_SynchronizeUseGravity = false;

        private bool m_StopReading = false;

        public void Awake()
        {
            this.m_Body = GetComponent<Rigidbody>();

            this.m_NetworkPosition = new Vector3();
            this.m_NetworkRotation = new Quaternion();
        }

        public void FixedUpdate()
        {
            if (!this.photonView.IsMine)
            {
                this.m_Body.position = Vector3.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                this.m_Body.rotation = Quaternion.RotateTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(this.m_Body.position);
                stream.SendNext(this.m_Body.rotation);
                stream.SendNext(this.m_Body.isKinematic);
                stream.SendNext(this.m_Body.useGravity);

                if (this.m_SynchronizeVelocity)
                {
                    stream.SendNext(this.m_Body.velocity);
                }

                if (this.m_SynchronizeAngularVelocity)
                {
                    stream.SendNext(this.m_Body.angularVelocity);
                }
            }
            else
            {
                if (m_StopReading)
                {
                    return;
                }

                this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();
                bool isKinematic = (bool)stream.ReceiveNext();
                bool useGravity = (bool)stream.ReceiveNext();

                if (this.m_TeleportEnabled)
                {
                    if (Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
                    {
                        this.m_Body.position = this.m_NetworkPosition;
                    }
                }
                
                if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                    if (this.m_SynchronizeVelocity)
                    {
                        this.m_Body.velocity = (Vector3)stream.ReceiveNext();

                        this.m_NetworkPosition += this.m_Body.velocity * lag;

                        this.m_Distance = Vector3.Distance(this.m_Body.position, this.m_NetworkPosition);
                    }

                    if (this.m_SynchronizeAngularVelocity)
                    {
                        this.m_Body.angularVelocity = (Vector3)stream.ReceiveNext();

                        this.m_NetworkRotation = Quaternion.Euler(this.m_Body.angularVelocity * lag) * this.m_NetworkRotation;

                        this.m_Angle = Quaternion.Angle(this.m_Body.rotation, this.m_NetworkRotation);
                    }
                }

                if (m_SynchronizeIsKinematic)
                {
                    this.m_Body.isKinematic = isKinematic;
                }

                if (m_SynchronizeUseGravity)
                {
                    this.m_Body.useGravity = useGravity;
                }

            }
        }

        public void StopSync()
        {
            m_StopReading = true;
        }
        
        public void ResumeSync()
        {
            m_StopReading = false;
        }
    }
}