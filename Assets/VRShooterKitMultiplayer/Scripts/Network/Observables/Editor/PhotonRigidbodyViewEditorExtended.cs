// ----------------------------------------------------------------------------
// <copyright file="PhotonRigidbodyViewEditor.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   This is a custom editor for the RigidbodyView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using UnityEditor;
    using UnityEngine;


    [CustomEditor(typeof (PhotonRigidbodyViewExtended))]
    public class PhotonRigidbodyViewExtendedEditor : MonoBehaviourPunEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Editing is disabled in play mode.", MessageType.Info);
                return;
            }

            PhotonRigidbodyViewExtended view = (PhotonRigidbodyViewExtended)target;

            view.m_TeleportEnabled = PhotonGUI.ContainerHeaderToggle("Enable teleport for large distances", view.m_TeleportEnabled);

            if (view.m_TeleportEnabled)
            {
                Rect rect = PhotonGUI.ContainerBody(20.0f);
                view.m_TeleportIfDistanceGreaterThan = EditorGUI.FloatField(rect, "Teleport if distance greater than", view.m_TeleportIfDistanceGreaterThan);
            }

            view.m_SynchronizeVelocity = PhotonGUI.ContainerHeaderToggle("Synchronize Velocity", view.m_SynchronizeVelocity);
            view.m_SynchronizeAngularVelocity = PhotonGUI.ContainerHeaderToggle("Synchronize Angular Velocity", view.m_SynchronizeAngularVelocity);
            view.m_SynchronizeIsKinematic = PhotonGUI.ContainerHeaderToggle("Synchronize Is Kinematic", view.m_SynchronizeIsKinematic);
            view.m_SynchronizeUseGravity = PhotonGUI.ContainerHeaderToggle("Synchronize Use Gravity", view.m_SynchronizeUseGravity);
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(view);
            }
        }
    }
}