using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace IzumiTools
{
    /// <summary>
    /// Atttaches to specific component and reusable to another component.<br/>
    /// Automatically invert activeSelf upon target gameobject state (Destroyed, Disabled, etc.).<br/>
    /// Used for object marker, etc.<br/>
    /// </summary>
    /// <typeparam name="T">Target component type</typeparam>
    [DisallowMultipleComponent]
    public abstract class ReusableAttachment<T> : MonoBehaviour where T : Component
    {
        protected abstract T AttachedTarget { get; set; }
        public abstract bool HasTarget { get; }

        private void LateUpdate()
        {
            if (HasTarget)
            {
                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
            }
            else if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
        public ReusableAttachment<T> GetAttachment(Transform attachmentNest, T target)
        {
            ReusableAttachment<T> tmpAttachment = null;
            //search already attached
            foreach (Transform child in attachmentNest)
            {
                ReusableAttachment<T> attachment;
                if (!child.TryGetComponent(out attachment))
                    continue;
                if (attachment.AttachedTarget == target)
                    return attachment;
                if (tmpAttachment == null && !attachment.HasTarget)
                    tmpAttachment = attachment;
            }
            //search empty
            if (tmpAttachment != null)
            {
                tmpAttachment.AttachedTarget = target;
                return tmpAttachment;
            }
            //generate new one
            (tmpAttachment = Instantiate(this)).transform.SetParent(attachmentNest, false);
            tmpAttachment.AttachedTarget = target;
            return tmpAttachment;
        }
    }
}