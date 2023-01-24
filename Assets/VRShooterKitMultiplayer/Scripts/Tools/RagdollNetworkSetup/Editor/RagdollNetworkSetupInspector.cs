using UnityEditor;
using UnityEngine;
using VRShooterKit.Multiplayer;

namespace VRShooterKit.EditorCode
{
    [CustomEditor(typeof(RagdollNetworkSetup))]
    public class RagdollNetworkSetupInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Setup Ragdoll"))
            {
                var ragdollSetup = target as RagdollNetworkSetup;
                ragdollSetup.SetupRagdoll();
            }
        }
    }
}

