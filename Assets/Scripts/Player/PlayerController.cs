using Player.Physics;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController: MonoBehaviour {

        private Rigidbody _rigidbody;
        private PlayerState _currentState = PlayerState.AIRBOURNE_FALLING;

        public float risingForce = 0.3f;
        public float fallingForce = 0.7f;
        public bool enforceMaxJumpHeight = false;
        public float maxJumpHeight = 1;
        public int maxNumJumps = 1;

        private Jump currentJump;

        private void Start() {
            if (_rigidbody == null) {
                _rigidbody = gameObject.GetComponent<Rigidbody>();
            }
            _rigidbody.isKinematic = false;
        }

        void Update() {
            if (Input.GetKeyDown("space") && canJump) {
                InitiateJump();
            }
            switch (_currentState) {
                case PlayerState.AIRBOURNE_RISING:
                case PlayerState.AIRBOURNE_FALLING:
                    HandleAirbourneState();
                    break;
            }
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
            switch (_currentState) {
                case PlayerState.AIRBOURNE_FALLING:
                    Land();
                    break;
            }
        }

        private void Land() {
            Debug.Log("PLayerController:: Land()");
            currentJump = null;
            _currentState = PlayerState.STANDING;
            _rigidbody.isKinematic = true;
        }

        private void InitiateJump() {
            Debug.Log("PLayerController:: InitiateJump()");
            _currentState = PlayerState.AIRBOURNE_RISING;
            if (currentJump == null) {
                currentJump = new Jump(maxJumpHeight, maxNumJumps, this);
            }
            
            _rigidbody.isKinematic = false;
            /////////
            //Set velocity to be a fraction above what it was, to avoid immediately going into the fall state
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0.01f, _rigidbody.velocity.z);
            /////////
            currentJump.IncJumpCounter();
            currentJump.ResetJumpStartingPos();
            
            _rigidbody.AddForce(0, risingForce, 0, ForceMode.Impulse);
        }

        private void InitiateFall() {
            Debug.Log("PLayerController:: InitiateFall()");
            _currentState = PlayerState.AIRBOURNE_FALLING;
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            _rigidbody.AddForce(0, -fallingForce, 0, ForceMode.Impulse);
        }

        private void HandleAirbourneState() {
            switch (_currentState) {
                    case PlayerState.AIRBOURNE_RISING:
                        if (!hasUpwardVerticalMomentum || hasReachedMaxJumpHeight) {
                            InitiateFall();
                        }
                        break;       
            }
        }

        private bool hasUpwardVerticalMomentum {
            get { return _rigidbody.velocity.y > 0; }
        }

        private bool hasReachedMaxJumpHeight {
            get { return enforceMaxJumpHeight && isJumping && currentJump.hasReachedMaxJumpHeight; }
        }

        private bool isJumping {
            get { return currentJump != null; }
        }

        private bool canJump {
            get {return _currentState == PlayerState.STANDING || (currentJump != null && currentJump.canJump); }
        }
    }
    
    enum PlayerState {
        STANDING,AIRBOURNE_RISING,AIRBOURNE_FALLING
    }
    
    
}