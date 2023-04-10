/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.Objects
{
    using UnityEngine;

    /// <summary>
    /// Specifices the target position and rotation of a Rigidbody moving platform.
    /// </summary>
    public interface IRigidbodyMovingPlatform
    {
        public Vector3 TargetPosition { get; }
        public Quaternion TargetRotation { get; }
    }
}