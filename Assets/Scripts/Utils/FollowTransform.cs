using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{

    private Transform followTransform;

    private void LateUpdate() {
        if (followTransform == null) {
            return;
        }

        transform.position = followTransform.position;
        transform.rotation = followTransform.rotation;
    }

    public void SetFollowTransform(Transform followTransform) {
        this.followTransform = followTransform;
    }

}
