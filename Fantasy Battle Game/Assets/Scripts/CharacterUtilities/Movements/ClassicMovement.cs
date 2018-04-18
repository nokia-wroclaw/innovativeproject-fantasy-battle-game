using CharacterUtilities.Interfaces;
using UnityEngine;

namespace CharacterUtilities.Movements
{
    public class ClassicMovement:MonoBehaviour, IMovement
    {
        public float RotationSpeed = 5;
        public float Speed = 10;
        
        private Vector3 targetPosition_;
        private Vector3 lookAtTarget_;
        private Quaternion playerRotation_;
        private bool moving_ = false;

        private void Start()
        {
            
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                SetTargetPosition();
            }

            if (moving_)
            {
                 Move();   
            }
        }
        
        public void SetTargetPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                targetPosition_ = hit.point;
                lookAtTarget_ = new Vector3(targetPosition_.x - transform.position.x,
                    transform.position.y, targetPosition_.z - transform.position.z);
                playerRotation_ = Quaternion.LookRotation(lookAtTarget_);
                moving_ = true;
            }
        }

        public void Move()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                playerRotation_, RotationSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition_,
                Speed * Time.deltaTime);

            if (transform.position == targetPosition_)
            {
                moving_ = false;
            }
        }
    }
}