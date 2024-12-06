using UnityEngine;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    public class SimpleTransformSetter : MonoBehaviour
    {
        public void SetLocalPos(Vector3 value) { transform.localPosition = value; }
        public void SetLocalPosXY(Vector2 value) { transform.localPosition = new Vector3(value.x, value.y, transform.localPosition.z); }
        public void SetLocalPosXZ(Vector2 value) { transform.localPosition = new Vector3(value.x, transform.localPosition.y, value.y); }
        public void SetLocalPosYZ(Vector2 value) { transform.localPosition = new Vector3(transform.localPosition.x, value.x, value.y); }
        public void SetLocalPosX(float value) { transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z); }
        public void SetLocalPosY(float value) { transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z); }
        public void SetLocalPosZ(float value) { transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value); }
        public void SetWorldPos(Vector3 value) { transform.position = value; }
        public void SetWorldPosXY(Vector2 value) { transform.position = new Vector3(value.x, value.y, transform.position.z); }
        public void SetWorldPosXZ(Vector2 value) { transform.position = new Vector3(value.x, transform.position.y, value.y); }
        public void SetWorldPosYZ(Vector2 value) { transform.position = new Vector3(transform.position.x, value.x, value.y); }
        public void SetWorldPosX(float value) { transform.position = new Vector3(value, transform.position.y, transform.position.z); }
        public void SetWorldPosY(float value) { transform.position = new Vector3(transform.position.x, value, transform.position.z); }
        public void SetWorldPosZ(float value) { transform.position = new Vector3(transform.position.x, transform.position.y, value); }
        public void SetLocalRot(Vector3 value) { transform.localEulerAngles = value; }
        public void SetLocalRotXY(Vector2 value) { transform.localEulerAngles = new Vector3(value.x, value.y, transform.localEulerAngles.z); }
        public void SetLocalRotXZ(Vector2 value) { transform.localEulerAngles = new Vector3(value.x, transform.localEulerAngles.y, value.y); }
        public void SetLocalRotYZ(Vector2 value) { transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, value.x, value.y); }
        public void SetLocalRotX(float value) { transform.localEulerAngles = new Vector3(value, transform.localEulerAngles.y, transform.localEulerAngles.z); }
        public void SetLocalRotY(float value) { transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, value, transform.localEulerAngles.z); }
        public void SetLocalRotZ(float value) { transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, value); }
        public void SetWorldRot(Vector3 value) { transform.eulerAngles = value; }
        public void SetWorldRotXY(Vector2 value) { transform.eulerAngles = new Vector3(value.x, value.y, transform.eulerAngles.z); }
        public void SetWorldRotXZ(Vector2 value) { transform.eulerAngles = new Vector3(value.x, transform.eulerAngles.y, value.y); }
        public void SetWorldRotYZ(Vector2 value) { transform.eulerAngles = new Vector3(transform.eulerAngles.x, value.x, value.y); }
        public void SetWorldRotX(float value) { transform.eulerAngles = new Vector3(value, transform.eulerAngles.y, transform.eulerAngles.z); }
        public void SetWorldRotY(float value) { transform.eulerAngles = new Vector3(transform.eulerAngles.x, value, transform.eulerAngles.z); }
        public void SetWorldRotZ(float value) { transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, value); }
        public void SetLocalScale(Vector3 value) { transform.localScale = value; }
        public void SetLocalScaleXY(Vector2 value) { transform.localScale = new Vector3(value.x, value.y, transform.localScale.z); }
        public void SetLocalScaleXZ(Vector2 value) { transform.localScale = new Vector3(value.x, transform.localScale.y, value.y); }
        public void SetLocalScaleYZ(Vector2 value) { transform.localScale = new Vector3(transform.localScale.x, value.x, value.y); }
        public void SetLocalScaleX(float value) { transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z); }
        public void SetLocalScaleY(float value) { transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z); }
        public void SetLocalScaleZ(float value) { transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, value); }
    }
}