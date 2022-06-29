using UnityEngine;

static class ExtensionMethods
{
    public static void SmoothLookAt(this Transform transform, Transform target, float speed)
    {
        Quaternion originalRot = transform.rotation;
        transform.LookAt(target);
        Quaternion newRot = transform.rotation;
        transform.rotation = originalRot;
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, speed * Time.deltaTime);
    }
}
