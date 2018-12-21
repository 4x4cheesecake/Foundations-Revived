//Originally written by forum user Sparkle: https://forum.kerbalspaceprogram.com/index.php?/profile/91081-sparkle/
//Link to the original thread: https://forum.kerbalspaceprogram.com/index.php?/topic/51430-plugin-022-wip-foundations-update-alpha-release-02/&tab=comments#comment-739075

using UnityEngine;

namespace Foundations
{
    class Foundation : PartModule
    {
        [KSPField(isPersistant = true)]
        public float breakForce = 500f;

        [KSPField(isPersistant = true)]
        public float breakTorque = 500f;

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
            Debug.Log("Foundations: AttachEvent()");
            
            if (!part.GroundContact)
            {
                Debug.Log("Foundations: No ground contact, aborting.");

                Message("Foundations not touching the ground.");
            }
            else
            {
                Attach();
            }
        }


        [KSPEvent(active = false, guiActive = true, guiName = "Detach Foundations")]
        public void DetachEvent()
        {
            Debug.Log("Foundations: DetachEvent()");

            Detach();
        }

        private void Attach()
        {
            Debug.Log("Foundations: Attach()");

            Events["AttachEvent"].active = false;
            Events["DetachEvent"].active = true;            
            attachOffset = Vector3.zero;            
            attachRotation = transform.rotation;
            isAttached = true;
            
            CreateAttachment();
        }

        private void Detach()
        {
            Debug.Log("Foundations: Detach()");

            Events["AttachEvent"].active = true;
            Events["DetachEvent"].active = false;
            attachOffset = Vector3.zero;
            attachRotation = Quaternion.identity;
            isAttached = false;

            DestroyAttachment();
        }

        public void OnPartUnpack()
        {
            Debug.Log(string.Format("Foundations: OnPartUnpack(isAttached = {0})", isAttached));

            if (!isAttached)
            {
                return;
            }

            DestroyAttachment();
            CreateAttachment();
        }
        
        public override void OnUpdate()
        {
            if (isAttached)
            {
                Events["AttachEvent"].active = false;
                Events["DetachEvent"].active = true;
            }
                        
            //Vector3 attachOffset = (fixedObject.transform.position - part.transform.position);

        }
        
        public void OnJointBreak(float force)
        {
            Debug.LogWarning(string.Format("Foundations: OnJointBreak(force = {0}, isAttached = {1})", force, isAttached));

            Detach();
        }

        private void CreateAttachment()
        {
            Debug.Log("Foundations: CreateAttachment()");
            
            Vector3 position = (part.transform.position + attachOffset);

            Debug.Log("Foundations: Creating object.");

            fixedObject = new GameObject("FoundationsBody");
            fixedObject.AddComponent<Rigidbody>();
            fixedObject.GetComponent<Rigidbody>().isKinematic = true;
            fixedObject.transform.position = position;
            fixedObject.transform.rotation = attachRotation;

            Debug.Log("Foundations: Creating joint.");
            fixedJoint = part.gameObject.AddComponent<FixedJoint>();
                        
            fixedJoint.breakForce = breakForce;
            fixedJoint.breakTorque = breakTorque;
                        
            fixedJoint.connectedBody = fixedObject.GetComponent<Rigidbody>();
        }

        private void DestroyAttachment()
        {
            Debug.Log("Foundations: DestroyAttachment()");
                        
            if (fixedJoint != null)
            {
                Debug.Log("Foundations: Destroying joint.");

                Destroy(fixedJoint);

                fixedJoint = null;
            }
                                    
            if (fixedObject != null)
            {
                Debug.Log("Foundations: Destroying object.");

                Destroy(fixedObject);
                fixedObject = null;                
            }
            else
            {
                return;
            }
                       
        }

        private void Message(string format, params object[] args)
        {
            ScreenMessages.PostScreenMessage(string.Format(format, args), 3f, 0);
        }

        public Foundation()
        {
            
        }
    }
}
 

