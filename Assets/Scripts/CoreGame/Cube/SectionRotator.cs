using UnityEngine;

public class SectionRotator : MonoBehaviour
{
    private readonly float arrivalTreshold = 1f;
    
    public bool RotationArrived(Quaternion targetRot, float speed)
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, Time.deltaTime * speed);
        
        return (Quaternion.Angle(transform.localRotation, targetRot) > arrivalTreshold);
    }
}