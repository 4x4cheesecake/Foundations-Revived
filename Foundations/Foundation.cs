using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Foundations
{
    class Foundation : PartModule
    {
        [KSPField]
        public float breakForce;

        [KSPField]
        public float breakTorque;

        [KSPField(isPersistant = true)]
        private bool isAttached;

        [KSPField(isPersistant = true)]
        private Vector3 attachOffset;

        [KSPField(isPersistant = true)]
        private Quaternion attachRotation;

        private FixedJoint fixedJoint;
        private GameObject fixedObject;

        [KSPEvent(guiActive = true, guiName = "Attach Foundations")]
        public void AttachEvent()
        {
            Debug.Log((object)"Foundations: AttachEvent()");
            if (!(bool)this.get_part().GroundContact)
            {
                Debug.Log((object)"Foundations: No ground contact, aborting.");
                this.Message("Foundations not touching the ground.");
            }
            else
                this.Attach();
        }

        [KSPEvent(active = false, guiActive = true, guiName = "Detach Foundations")]
        public void DetachEvent()
        {
            Debug.Log((object)"Foundations: DetachEvent()");
            this.Detach();
        }

        private void Attach()
        {
            Debug.Log((object)"Foundations: Attach()");
            this.get_Events().get_Item("AttachEvent").active = (__Null)0;
            this.get_Events().get_Item("DetachEvent").active = (__Null)1;
            this.attachOffset = Vector3.get_zero();
            this.attachRotation = ((Component)this).get_transform().get_rotation();
            this.isAttached = true;
            this.CreateAttachment();
        }

        private void Detach()
        {
            Debug.Log((object)"Foundations: Detach()");
            this.get_Events().get_Item("AttachEvent").active = (__Null)1;
            this.get_Events().get_Item("DetachEvent").active = (__Null)0;
            this.attachOffset = Vector3.get_zero();
            this.attachRotation = Quaternion.get_identity();
            this.isAttached = false;
            this.DestroyAttachment();
        }

        public void OnPartUnpack()
        {
            Debug.Log((object)string.Format("Foundations: OnPartUnpack(isAttached = {0})", (object)this.isAttached));
            if (!this.isAttached)
                return;
            this.DestroyAttachment();
            this.CreateAttachment();
        }

        public virtual void OnUpdate()
        {
            if (!this.isAttached)
                return;
            this.attachOffset = Vector3.op_Subtraction(this.fixedObject.get_transform().get_position(), ((Component)this.get_part()).get_transform().get_position());
        }

        public void OnJointBreak(float force)
        {
            Debug.LogWarning((object)string.Format("Foundations: OnJointBreak(force = {0}, isAttached = {1})", (object)force, (object)this.isAttached));
            this.Detach();
        }

        private void CreateAttachment()
        {
            Debug.Log((object)"Foundations: CreateAttachment()");
            Vector3 vector3 = Vector3.op_Addition(((Component)this.get_part()).get_transform().get_position(), this.attachOffset);
            Debug.Log((object)"Foundations: Creating object.");
            this.fixedObject = new GameObject("FoundationsBody");
            this.fixedObject.AddComponent<Rigidbody>();
            this.fixedObject.get_rigidbody().set_isKinematic(true);
            this.fixedObject.get_transform().set_position(vector3);
            this.fixedObject.get_transform().set_rotation(this.attachRotation);
            Debug.Log((object)"Foundations: Creating joint.");
            this.fixedJoint = (FixedJoint)((Component)this.get_part()).get_gameObject().AddComponent<FixedJoint>();
            ((Joint)this.fixedJoint).set_breakForce(this.breakForce);
            ((Joint)this.fixedJoint).set_breakTorque(this.breakTorque);
            ((Joint)this.fixedJoint).set_connectedBody(this.fixedObject.get_rigidbody());
        }

        private void DestroyAttachment()
        {
            Debug.Log((object)"Foundations: DestroyAttachment()");
            if (Object.op_Inequality((Object)this.fixedJoint, (Object)null))
            {
                Debug.Log((object)"Foundations: Destroying joint.");
                Object.Destroy((Object)this.fixedJoint);
                this.fixedJoint = (FixedJoint)null;
            }
            if (!Object.op_Inequality((Object)this.fixedObject, (Object)null))
                return;
            Debug.Log((object)"Foundations: Destroying object.");
            Object.Destroy((Object)this.fixedObject);
            this.fixedObject = (GameObject)null;
        }

        private void Message(string format, params object[] args)
        {
            ScreenMessages.PostScreenMessage(string.Format(format, args), 3f, (ScreenMessageStyle)0);
        }

        public Foundation()
        {
            base.\u002Ector();
        }
    }
}
 

