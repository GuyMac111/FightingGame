using Player.Physics;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(Rigidbody))]
    public class AerialController: MonoBehaviour {

        private Rigidbody _rigidbody;
        public PlayerState currentState = PlayerState.AIRBOURNE_FALLING;

        public float risingForce = 0.3f;
        public float fallingForce = 0.7f;
        public bool enforceMaxJumpHeight = false;
        public float maxJumpHeight = 1;
        public int maxNumJumps = 1;
        

        private AerialSession currentAerialSession;

        void Start() {
            if (_rigidbody == null) {
                _rigidbody = gameObject.GetComponent<Rigidbody>();
            }
            _rigidbody.isKinematic = false;
        }

        void Update() {
            if (Input.GetKeyDown("space")) {
                OnJumpInput();
            }
            
            switch (currentState) {
                case PlayerState.AIRBOURNE_RISING:
                case PlayerState.AIRBOURNE_FALLING:
                    HandleAirbourneState();
                    break;
            }
        }

        private void OnJumpInput() {
            if (maxNumJumps <= 0) {
                //special case for characters we may not wish to have a jump,
                //or a potential mechanic to disable the opponent's jump ability.
                return;
            }
            if (currentAerialSession == null) {
                currentAerialSession = new AerialSession(maxJumpHeight, enforceMaxJumpHeight, maxNumJumps, this, _rigidbody);
            }
            currentAerialSession.RequestJump();
        }
        
        private void HandleAirbourneState() {
            if (currentAerialSession == null) {
                //Something to handle the sudden disappearance of a floor. E.g. game start, or walking off an edge
                currentAerialSession = new AerialSession(maxJumpHeight, enforceMaxJumpHeight, maxNumJumps-1,this, _rigidbody);
            }
            currentAerialSession.HandleAerialState();
        }
        
        private void OnCollisionEnter(Collision other) {
            EnvironmentID collidedWithWhat = other.gameObject.GetComponent<EnvironmentID>();
            if (!collidedWithWhat) {
                return;
            } 
            switch(collidedWithWhat.envType) {
                case EnvironmentID.EnvironmentObject.FLOOR:
                    HandleCollisionWithFloor();
                    break;
            }
        }

        private void HandleCollisionWithFloor() {
            switch (currentState) {
                case PlayerState.AIRBOURNE_FALLING:
                    OnGroundCollision();
                    break;
            }
        }

        private void OnGroundCollision() {
            currentAerialSession.Land();
            currentAerialSession = null;
        }
    }
    
    public enum PlayerState {
        GROUNDED,AIRBOURNE_RISING,AIRBOURNE_FALLING
    }
    
    
}