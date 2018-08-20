
using UnityEditor;
using UnityEngine;
namespace Player.Physics {
    public class Jump {
        
        private float maxJumpHeight;
        private int maxNumJumps;
        private int jumpsUsed;
        private float startingJumpHeight;
        private PlayerController player;
        
        public Jump(float maximumJumpHeight, int maximumNumJumps, PlayerController playerController) {
            maxJumpHeight = maximumJumpHeight;
            maxNumJumps = maximumNumJumps;
            player = playerController;
            jumpsUsed = 0;
            startingJumpHeight = player.transform.position.y;
        }

        public void IncJumpCounter() {
            jumpsUsed++;
        }

        public void ResetJumpStartingPos() {
            startingJumpHeight = player.transform.position.y;
        }

        public bool hasReachedMaxJumpHeight {
            get {
                bool result = player.transform.position.y - startingJumpHeight >=  maxJumpHeight;
                if(result) {
                    Debug.Log("Jump:: Maximum jump height reached");
                }
                return result;
            } 
        }

        public bool canJump {

            get {
                bool result = jumpsUsed < maxNumJumps;
                Debug.Log("Canjump = " + result.ToString());
                return jumpsUsed < maxNumJumps;
            }
        }

        public bool isDoubleJump {
            get { return jumpsUsed > 0; }
        }
        
    }
}