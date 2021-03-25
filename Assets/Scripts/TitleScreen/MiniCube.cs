using System;
using UnityEngine;

public class MiniCube : MonoBehaviour
{
    [SerializeField] private Texture[] sizeTextures;

    private MeshRenderer meshRenderer;

    private static float rotSpeed = 9f;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed, Space.Self);
    }

    public void Show(bool value)
    {
        meshRenderer.enabled = value;
    }

    public void ChangeSize(int size)
    {
        size -= 2;
        meshRenderer.material.mainTexture = sizeTextures[size];
        transform.localScale = Vector3.one * (0.8f + (0.1f * size));
        transform.rotation = Quaternion.Euler(-20f, 0f, 0f);
    }
}