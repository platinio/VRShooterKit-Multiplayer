﻿using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{


    /// <summary>
    /// Basic singleton class
    /// </summary>
    public class PhotonSingleton<T> : MonoBehaviourPun where T : PhotonSingleton<T>, IMatchmakingCallbacks
    {
        private static T m_instance = null;

        public static T instance
        {
            get
            {
                if(m_instance == null)
                {
                    //get all the singletones
                    T[] singletons = GameObject.FindObjectsOfType( typeof(T) ) as T[];

                    if(singletons != null)
                    {
                        if(singletons.Length == 1)
                        {

                            m_instance = singletons[0];
                            return m_instance;
                        }

                        else if(singletons.Length > 1)
                        {
                            Debug.LogWarning("You have more thah one " + typeof( T ).Name + " In the scene, you only need one , all the instances will be destroyed for create a new one");

                            m_instance = singletons[0];

                            for(int n = 1 ; n < singletons.Length ; n++)
                            {
                                Destroy( singletons[n].gameObject );
                            }

                            return m_instance;
                        }
                    }
                }

                return m_instance;

            }
        }



        public static bool ApplicationIsQuitting = false;
        public static event Action<T> OnInstanceCreated = null;
		

        public static bool DestroyOnLoad
        {
            get
            {
                return m_instance.m_destroyOnLoad;
            }
        }


        [SerializeField] protected bool m_destroyOnLoad	= true;
        [SerializeField] private int m_viewId = 999;
		
        protected virtual void Awake()
        {
            if( !ReferenceEquals( (object)instance , (object)gameObject.GetComponent<T>() ))
            {
                //destroy repeat instance
                Destroy(gameObject);
            }
        }

        protected virtual void Start()
        {
            //get the instance
            T singleton = instance;

            if (!DestroyOnLoad && instance.transform.parent != null)
            {
                instance.transform.parent = null;
            }
           

            if(!DestroyOnLoad)
            {
                DontDestroyOnLoad( m_instance );
            }
			
            OnInstanceCreated?.Invoke(instance);
        }

        protected virtual void OnApplicationQuit()
        {
            ApplicationIsQuitting = true;
        }
        
        public virtual void OnJoinedRoom()
        {
            AddPhotonView();
        }
        
        private void AddPhotonView()
        {
            var view = gameObject.AddComponent<PhotonView>();
            view.ViewID = m_viewId;
        }
        
        public virtual void OnLeftRoom()
        {
            RemovePhotonView();
        }
        
        private void RemovePhotonView()
        {
            var view = GetComponent<PhotonView>();

            if (view != null)
            {
                Destroy(view);
            }
        }
    }

}