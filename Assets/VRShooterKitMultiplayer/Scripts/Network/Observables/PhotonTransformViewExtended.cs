// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using UnityEngine;

    [AddComponentMenu("Photon Networking/Photon Transform View Extended")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    public class PhotonTransformViewExtended : MonoBehaviourPun, IPunObservable
    {
        private float m_Distance;
        private float m_Angle;

        private Vector3 m_Direction;
        private Vector3 m_NetworkPosition;
        private Vector3 m_StoredPosition;

        private Quaternion m_NetworkRotation;

        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = false;
        public bool m_SynchronizeInLocalSpace = true;

        bool m_firstTake = false;

        public void Awake()
        {
            m_StoredPosition = transform.localPosition;
            m_NetworkPosition = Vector3.zero;

            m_NetworkRotation = Quaternion.identity;
        }

        void OnEnable()
        {
            m_firstTake = true;
        }

        public void Update()
        {
            if (!this.photonView.IsMine)
            {
                if (m_SynchronizeInLocalSpace)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
                }

                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
                }
                
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (this.m_SynchronizePosition)
                {
                    if (m_SynchronizeInLocalSpace)
                    {
                        this.m_Direction = transform.localPosition - this.m_StoredPosition;
                        this.m_StoredPosition = transform.localPosition;

                        stream.SendNext(transform.localPosition);
                        stream.SendNext(this.m_Direction);
                    }
                    else
                    {
                        this.m_Direction = transform.position - this.m_StoredPosition;
                        this.m_StoredPosition = transform.position;

                        stream.SendNext(transform.position);
                        stream.SendNext(this.m_Direction);
                    }
                    
                }

                if (this.m_SynchronizeRotation)
                {
                    if (m_SynchronizeInLocalSpace)
                    {
                        stream.SendNext(transform.localRotation);
                    }
                    else
                    {
                        stream.SendNext(transform.rotation);
                    }
                }

                if (this.m_SynchronizeScale)
                {
                    stream.SendNext(transform.localScale);
                }
            }
            else
            {


                if (this.m_SynchronizePosition)
                {
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                    this.m_Direction = (Vector3)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        if (m_SynchronizeInLocalSpace)
                        {
                            transform.localPosition = this.m_NetworkPosition;
                        }
                        else
                        {
                            transform.position = this.m_NetworkPosition;
                        }
                        
                        this.m_Distance = 0f;
                    }
                    else
                    {
                        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                        this.m_NetworkPosition += this.m_Direction * lag;

                        if (m_SynchronizeInLocalSpace)
                        {
                            this.m_Distance = Vector3.Distance(transform.localPosition, this.m_NetworkPosition);
                        }
                        else
                        {
                            this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
                        }
                        
                    }


                }

                if (this.m_SynchronizeRotation)
                {
                    this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        if (m_SynchronizeInLocalSpace)
                        {
                            this.m_Angle = 0f;
                            transform.localRotation = this.m_NetworkRotation;
                        }
                        else
                        {
                            this.m_Angle = 0f;
                            transform.rotation = this.m_NetworkRotation;
                        }
                    }
                    else
                    {
                        if (m_SynchronizeInLocalSpace)
                        {
                            this.m_Angle = Quaternion.Angle(transform.localRotation, this.m_NetworkRotation);
                        }
                        else
                        {
                            this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
                        }
                        
                    }
                }

                if (this.m_SynchronizeScale)
                {
                    transform.localScale = (Vector3)stream.ReceiveNext();
                }

                if (m_firstTake)
                {
                    m_firstTake = false;
                }
            }
        }
    }
}