using UnityEngine;
using System.Collections;

//[ExecuteInEditMode()]
public class MultiScreenCamera : MonoBehaviour
{
    public Camera multiScreenCamera;

    public ScreenPlane screenPlane;
    
	void Update ()
    {
        SetPerspetive();
	}

    void SetPerspetive()
    {
        multiScreenCamera.GetComponent<Camera>().ResetProjectionMatrix();

        Transform center = screenPlane.lookAt;

        multiScreenCamera.transform.LookAt(center);

        float dist = Vector3.Distance(multiScreenCamera.transform.position, center.position);

        float rate = multiScreenCamera.nearClipPlane / dist;

        float _left, _right, _top, _bottom;

        Transform trns = center.parent;

        center.parent = screenPlane.transform;

        _left = rate * Vector3.Distance(screenPlane.left.position, center.position) * Mathf.Sign(center.transform.localPosition.x - screenPlane.left.localPosition.x);

        _right = rate * Vector3.Distance(screenPlane.right.position, center.position) * Mathf.Sign(center.transform.localPosition.x - screenPlane.right.localPosition.x);

        _top = rate * (screenPlane.top.position.y - center.position.y);

        _bottom = rate * (screenPlane.bottom.position.y - center.position.y);

        center.parent = trns;

        multiScreenCamera.projectionMatrix = PerspectiveOffCenter(_left, _right, _bottom, _top, multiScreenCamera.nearClipPlane, multiScreenCamera.farClipPlane);
    }

    Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = (2.0f * near) / (right - left);
        float y = (2.0f * near) / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0f * far * near) / (far - near);
        float e = -1.0f;

        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x; m[0, 1] = 0; m[0, 2] = a; m[0, 3] = 0;
        m[1, 0] = 0; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0;
        m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = c; m[2, 3] = d;
        m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = e; m[3, 3] = 0;
        return m;
    }
}
