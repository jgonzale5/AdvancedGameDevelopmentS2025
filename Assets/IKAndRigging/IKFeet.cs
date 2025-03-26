using UnityEngine;

public class IKFeet : MonoBehaviour
{
    //A reference to the animator
    public Animator animator;

    //The vertical distance between the ankle and the sole
    public float ankleOffset = 1f;
    //The length of the raycast cast from the sole
    public float rayLength = 1f;

    //At start we get the animator to make sure it's there
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //Set the IK of both feet
        //We get the weights from the animator
        FootIK(AvatarIKGoal.LeftFoot, animator.GetFloat("LeftFootIKWeight"));
        FootIK(AvatarIKGoal.RightFoot, animator.GetFloat("RightFootIKWeight"));
    }

    private void FootIK(AvatarIKGoal goal, float weight)
    {
        //We override the animation for this body part, based on the weight passed down to this function
        animator.SetIKPositionWeight(goal, weight);
        animator.SetIKRotationWeight(goal, weight);

        //The ray starts at the ankle, pointing down
        Ray ray = new Ray(animator.GetIKPosition(goal), Vector3.down);

        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * (ankleOffset + rayLength));

        //If the ray hits something
        if (Physics.Raycast(ray, out hit, ankleOffset + rayLength))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.blue);

            //The sole is set to match the hit point
            Vector3 footPos = hit.point;
            footPos.y += ankleOffset;

            //The IK position and rotation are updated
            animator.SetIKPosition(goal, footPos);
            //We maintain the forward vector but make the up vector match the normal of the surface
            animator.SetIKRotation(goal, Quaternion.LookRotation(transform.forward, hit.normal));

        }
    }
}
