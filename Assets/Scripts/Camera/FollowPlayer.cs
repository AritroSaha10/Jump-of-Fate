/*
 * Authors: Tony 
 */
using UnityEngine;

namespace Assets.Scripts.Camera
{
    /// <summary>
    /// Makes camera locked onto player position, at all times 
    /// </summary>
    public class FollowPlayer : MonoBehaviour
    {
        /// <summary>
        /// Target the camera is locked onto
        /// </summary>
        public Transform target;
        /// <summary>
        /// How offset the camera should be
        /// </summary>
        public Vector3 offset;

        // Update is called once per frame
        void LateUpdate()
        {
            // get target position, apply offset, and set as camera position
            transform.position = target.position + offset;
        }
    }
}

// Citations:
// �Unity documentation,� Unity Documentation, https://docs.unity.com/ 
// Zaphod, �Unity camera follows player script,� Stack Overflow, https://stackoverflow.com/questions/65816546/unity-camera-follows-player-script