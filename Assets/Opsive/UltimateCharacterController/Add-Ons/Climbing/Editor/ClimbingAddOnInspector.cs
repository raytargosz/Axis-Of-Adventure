/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.AddOns.Climbing.Editor
{
    using Opsive.Shared.Editor.UIElements.Managers;
    using Opsive.UltimateCharacterController.AddOns.Shared.Editor;
    using Opsive.UltimateCharacterController.Editor.Managers;
    using UnityEditor.Animations;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Draws the inspector for the climbing add-on.
    /// </summary>
    [OrderedEditorItem("Climbing Pack", 3)]
    public class ClimbingAddOnInspector : AddOnInspector, IAbilityAddOnInspector
    {
        private GameObject m_Character;
        private bool m_AddAbilities = true;
        private bool m_AddAnimations = true;
        private AnimatorController m_AnimatorController;
        private Button m_BuildButton;

        public GameObject Character { get { return m_Character; } set { m_Character = value; } }
        public bool AddAbilities { get { return m_AddAbilities; } set { m_AddAbilities = value; } }
        public bool AddAnimations { get { return m_AddAnimations; } set { m_AddAnimations = value; } }
        public AnimatorController AnimatorController { get { return m_AnimatorController; } set { m_AnimatorController = value; } }
        public AnimatorController FirstPersonAnimatorController { get { return null; } set { } }
        public string AddOnName { get { return "Climbing"; } }
        public string AbilityNamespace { get { return "Opsive.UltimateCharacterController.AddOns.Climbing"; } }
        public bool ShowFirstPersonAnimatorController { get { return false; } }
        public Button BuildButton { get { return m_BuildButton; } set { m_BuildButton = value; } }

        /// <summary>
        /// Draws the add-on inspector.
        /// </summary>
        /// <param name="container">The parent VisualElement container.</param>
        public override void ShowInspector(VisualElement container)
        {
            AddOnInspectorUtility.ShowInspector(this, container);
        }
    }
}