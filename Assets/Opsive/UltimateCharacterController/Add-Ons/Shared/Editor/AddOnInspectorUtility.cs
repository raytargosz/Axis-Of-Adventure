/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.AddOns.Shared.Editor
{
    using Opsive.Shared.Editor.Inspectors.Utility;
    using Opsive.UltimateCharacterController.Editor.Managers;
    using Opsive.UltimateCharacterController.Editor.Controls.Types.AbilityDrawers;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Shared methods for drawing the ability animation options.
    /// </summary>
    public static class AddOnInspectorUtility
    {
        private static IAbilityAddOnInspector s_AddOnInspector;

        /// <summary>
        /// Draws the add-on inspector.
        /// </summary>
        /// <param name="addOnInspector">A reference to the shown add-on insepctor.</param>
        /// <param name="container">The parent VisualElement container.</param>
        public static void ShowInspector(IAbilityAddOnInspector addOnInspector, VisualElement container)
        {
            s_AddOnInspector = addOnInspector;

            s_AddOnInspector.BuildButton = ManagerUtility.ShowControlBox(s_AddOnInspector.AddOnName + " Abilities & Animations", "This option will add the " + s_AddOnInspector.AddOnName.ToLower() + " abilities or animations to your character.",
                            ShowAgentSetup, "Setup Character", SetupCharacter, container, true, 4);
            s_AddOnInspector.BuildButton.SetEnabled(CanSetupCharacter());
        }

        /// <summary>
        /// Draws the additional controls for the animator.
        /// </summary>
        private static void ShowAgentSetup(VisualElement container)
        {
            container.Clear();

            var characterField = new ObjectField("Character");
            characterField.objectType = typeof(GameObject);
            characterField.allowSceneObjects = true;
            characterField.value = s_AddOnInspector.Character;
            characterField.RegisterValueChangedCallback(c =>
            {
                s_AddOnInspector.Character = (GameObject)c.newValue;
                s_AddOnInspector.AnimatorController = null;
                if (s_AddOnInspector.Character != null) {
                    var animatorMonitor = s_AddOnInspector.Character.GetComponentInChildren<Character.AnimatorMonitor>(true);
                    if (animatorMonitor != null) {
                        var animator = animatorMonitor.GetComponent<Animator>();
                        if (animator != null) {
                            s_AddOnInspector.AnimatorController = (AnimatorController)animator.runtimeAnimatorController;
                        }
                    }
#if FIRST_PERSON_CONTROLLER
                    var firstPersonBaseObjects = s_AddOnInspector.Character.GetComponentsInChildren<FirstPersonController.Character.Identifiers.FirstPersonBaseObject>(true);
                    if (firstPersonBaseObjects != null && firstPersonBaseObjects.Length > 0) {
                        var firstPersonBaseObject = firstPersonBaseObjects[0];
                        // Choose the base object with the lowest ID.
                        for (int i = 1; i < firstPersonBaseObjects.Length; ++i) {
                            if (firstPersonBaseObjects[i].ID < firstPersonBaseObject.ID) {
                                firstPersonBaseObject = firstPersonBaseObjects[i];
                            }
                        }

                        var animator = firstPersonBaseObject.GetComponent<Animator>();
                        if (animator != null) {
                            s_AddOnInspector.FirstPersonAnimatorController = (AnimatorController)animator.runtimeAnimatorController;
                        }
                    }
#endif
                }

                ShowAgentSetup(container);
            });
            container.Add(characterField);

            // The character must first be created by the Character Manager.
            if (s_AddOnInspector.Character != null && s_AddOnInspector.Character.GetComponent<Character.UltimateCharacterLocomotion>() == null) {
                var helpBox = new HelpBox("The character must first be setup by the Character Manager.", HelpBoxMessageType.Error);
                container.Add(helpBox);
            }

            var addAbilitiesToggle = new Toggle("Add Abilities");
            addAbilitiesToggle.value = s_AddOnInspector.AddAbilities;
            addAbilitiesToggle.RegisterValueChangedCallback(c =>
            {
                s_AddOnInspector.AddAbilities = c.newValue;
                ShowAgentSetup(container);
            });
            container.Add(addAbilitiesToggle);

            var addAnimationsToggle = new Toggle("Add Animations");
            addAnimationsToggle.value = s_AddOnInspector.AddAnimations;
            addAnimationsToggle.RegisterValueChangedCallback(c =>
            {
                s_AddOnInspector.AddAnimations = c.newValue;
                ShowAgentSetup(container);
            });
            container.Add(addAnimationsToggle);

            if (s_AddOnInspector.AddAnimations) {
                var animatorControllerField = new ObjectField("Animator Controller");
                animatorControllerField.Q<Label>().AddToClassList("indent");
                animatorControllerField.objectType = typeof(AnimatorController);
                animatorControllerField.value = s_AddOnInspector.AnimatorController;
                animatorControllerField.RegisterValueChangedCallback(c =>
                {
                    s_AddOnInspector.AnimatorController = c.newValue as AnimatorController;
                    ShowAgentSetup(container);
                });
                container.Add(animatorControllerField);

                s_AddOnInspector.AnimatorController = ClampAnimatorControllerField("Animator Controller", s_AddOnInspector.AnimatorController, 33);
#if FIRST_PERSON_CONTROLLER
                if (s_AddOnInspector.ShowFirstPersonAnimatorController) {
                    animatorControllerField = new ObjectField("First Person Animator Controller");
                    animatorControllerField.Q<Label>().AddToClassList("indent");
                    animatorControllerField.objectType = typeof(AnimatorController);
                    animatorControllerField.value = s_AddOnInspector.FirstPersonAnimatorController;
                    animatorControllerField.RegisterValueChangedCallback(c =>
                    {
                        s_AddOnInspector.FirstPersonAnimatorController = c.newValue as AnimatorController;
                        ShowAgentSetup(container);
                    });
                    container.Add(animatorControllerField);
                }
#endif
            }
            if (s_AddOnInspector.BuildButton != null) {
                s_AddOnInspector.BuildButton.SetEnabled(CanSetupCharacter());
            }
        }

        /// <summary>
        /// Returns true if the character can be setup.
        /// </summary>
        /// <returns>True if the character can be setup.</returns>
        private static bool CanSetupCharacter()
        {
            if (s_AddOnInspector.Character == null || s_AddOnInspector.Character.GetComponent<Character.UltimateCharacterLocomotion>() == null) {
                return false;
            }

            if (s_AddOnInspector.AddAnimations && s_AddOnInspector.AnimatorController == null) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds the abilities and animations to the animator controllers.
        /// </summary>
        private static void SetupCharacter()
        {
            var types = InspectorDrawerUtility.GetAllTypesWithinNamespace(s_AddOnInspector.AbilityNamespace);
            if (types == null) {
                return;
            }

            if (s_AddOnInspector.AddAbilities) {
                var characterLocomotion = s_AddOnInspector.Character.GetComponent<Character.UltimateCharacterLocomotion>();
                var abilities = characterLocomotion.Abilities;
                // Call AbilityBuilder on all of the abilities.
                for (int i = 0; i < types.Count; ++i) {
                    if (!typeof(Character.Abilities.Ability).IsAssignableFrom(types[i])) {
                        continue;
                    }
                    var hasAbility = false;
                    // Do not add duplicates.
                    for (int j = 0; j < abilities.Length; ++j) {
                        if (abilities[j] != null && abilities[j].GetType() == types[i]) {
                            hasAbility = true;
                            break;
                        }
                    }
                    if (hasAbility) {
                        continue;
                    }
                    Utility.Builders.AbilityBuilder.AddAbility(characterLocomotion, types[i], i);
                }
                Opsive.Shared.Editor.Utility.EditorUtility.SetDirty(characterLocomotion);
            }

            if (s_AddOnInspector.AddAnimations) {
                // Call BuildAnimator on all of the inspector drawers for the abilities.
                for (int i = 0; i < types.Count; ++i) {
                    var abilityDrawer = AbilityDrawerUtility.FindAbilityDrawer(types[i], true);
                    if (abilityDrawer == null || !abilityDrawer.CanBuildAnimator) {
                        continue;
                    }

                    abilityDrawer.BuildAnimator(new AnimatorController[] { s_AddOnInspector.AnimatorController }, new AnimatorController[] { s_AddOnInspector.FirstPersonAnimatorController });
                }
            }

            Debug.Log("The character was successfully setup." + (s_AddOnInspector.AddAbilities ? " Refer to the documentation for the steps to configure the abilities." : string.Empty));
        }

        /// <summary>
        /// Prevents the label from being too far away from the object field.
        /// </summary>
        /// <param name="label">The animator controller label.</param>
        /// <param name="animatorController">The animator controller value.</param>
        /// <param name="widthAddition">Any additional width to separate the label and the control.</param>
        /// <returns>The new animator controller.</returns>
        private static AnimatorController ClampAnimatorControllerField(string label, AnimatorController animatorController, int widthAddition)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = textDimensions.x + widthAddition;
            animatorController = EditorGUILayout.ObjectField(label, animatorController, typeof(AnimatorController), true) as AnimatorController;
            EditorGUIUtility.labelWidth = prevLabelWidth;
            return animatorController;
        }
    }
}