
using System;
using UnityEditor;
using UnityEngine;
namespace Player.Physics {
    public class AerialSession {
        private float maxJumpHeight;
        private int maxNumJumps;
        private int jumpsUsed;
        private float startingJumpHeight;
        private bool shoulEnforceMaxJumpHeight;
        private AerialController aerialController;
        private Rigidbody rigidBody;
        
        public AerialSession(float maximumJumpHeight, bool enforceMaxJumpHeight, int maximumNumJumps, AerialController controller, Rigidbody rigidBody) {
            maxJumpHeight = maximumJumpHeight;
            shoulEnforceMaxJumpHeight = enforceMaxJumpHeight;
            maxNumJumps = maximumNumJumps;
            aerialController = controller;
            jumpsUsed = 0;
            startingJumpHeight = aerialController.transform.position.y;
            this.rigidBody = rigidBody;
        }

        public void RequestJump() {
            if (canJump) {
                rigidBody.isKinematic = false;
                /////////
                //Set velocity to be a fraction above what it was, to avoid immediately going into the fall state
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0.01f, rigidBody.velocity.z);
                /////////
                IncJumpCounter();
                ResetJumpStartingPos();
                aerialController.currentState = PlayerState.AIRBOURNE_RISING;
                rigidBody.AddForce(0, aerialController.risingForce, 0, ForceMode.Impulse);
            }
        }

        public void InitiateFall() {
            Debug.Log("AerialSession:: InitiateFall()");
            aerialController.currentState = PlayerState.AIRBOURNE_FALLING;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
            rigidBody.AddForce(0, -aerialController.fallingForce, 0, ForceMode.Impulse);
        }

        public void Land() {
            Debug.Log("AerialSession:: Land()");
            aerialController.currentState = PlayerState.GROUNDED;
            rigidBody.isKinematic = true;
        }

        public void HandleAerialState() {
            switch (aerialController.currentState) {
                case PlayerState.AIRBOURNE_RISING:
                    if (!hasUpwardVerticalMomentum || hasReachedMaxJumpHeight) {
                        InitiateFall();
                    }
                    break;       
            }
        }
        
        private bool canJump {
            get {
                bool result = jumpsUsed < maxNumJumps;
                Debug.Log("canJump = " + result);
                return jumpsUsed < maxNumJumps;
            }
        }
        
        private bool hasUpwardVerticalMomentum {
            get { return rigidBody.velocity.y > 0; }
        }
        
        private bool hasReachedMaxJumpHeight {
            get {
                bool result = aerialController.transform.position.y - startingJumpHeight >=  maxJumpHeight 
                              && shoulEnforceMaxJumpHeight;
                if(result) {
                    Debug.Log("AerialSession:: Maximum jump height reached and is being enforced");
                }
                return result;
            } 
        }
        
        private void ResetJumpStartingPos() {
            startingJumpHeight = aerialController.transform.position.y;
        }
        
        private void IncJumpCounter() {
            jumpsUsed++;
        }
    }
}
