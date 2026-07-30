using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Simple25DRPG.Editor.Input
{
    /// <summary>
    /// Creates or updates the reusable InputActionReference asset for player movement.
    /// </summary>
    public static class MoveInputActionReferenceCreator
    {
        private const string InputActionsPath = "Assets/InputSystem_Actions.inputactions";
        private const string OutputFolderPath = "Assets/Data/Player";
        private const string OutputAssetPath = OutputFolderPath + "/MoveInputActionReference.asset";
        private const string ActionMapName = "Player";
        private const string ActionName = "Move";

        /// <summary>
        /// Creates or updates the Player/Move InputActionReference asset.
        /// </summary>
        [MenuItem("Tools/Input/Create Move Input Action Reference")]
        public static void CreateMoveInputActionReference()
        {
            InputActionAsset inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            if (inputActions == null)
            {
                Debug.LogError($"Could not find Input Action Asset at '{InputActionsPath}'.");
                return;
            }

            InputAction moveAction = inputActions.FindActionMap(ActionMapName)?.FindAction(ActionName);
            if (moveAction == null)
            {
                Debug.LogError($"Could not find action '{ActionMapName}/{ActionName}' in '{InputActionsPath}'.");
                return;
            }

            EnsureOutputFolderExists();

            InputActionReference reference = AssetDatabase.LoadAssetAtPath<InputActionReference>(OutputAssetPath);
            if (reference == null)
            {
                reference = InputActionReference.Create(moveAction);
                AssetDatabase.CreateAsset(reference, OutputAssetPath);
                Debug.Log($"Created InputActionReference at '{OutputAssetPath}'.");
            }
            else
            {
                reference.Set(moveAction);
                EditorUtility.SetDirty(reference);
                Debug.Log($"Updated InputActionReference at '{OutputAssetPath}'.");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = reference;
        }

        private static void EnsureOutputFolderExists()
        {
            if (AssetDatabase.IsValidFolder(OutputFolderPath))
            {
                return;
            }

            Directory.CreateDirectory(OutputFolderPath);
            AssetDatabase.Refresh();
        }
    }
}
