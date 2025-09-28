using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimEditorHelper : MonoBehaviour
{
    public Transform rootBone;
    [SerializeField] BoneRenderer boneRenderer;

    private void OnValidate()
    {
        if (!rootBone)
            return;

        if(boneRenderer == null)
            boneRenderer = GetComponent<BoneRenderer>();
        if (boneRenderer == null)
        {
            boneRenderer = gameObject.AddComponent<BoneRenderer>();
        }
        if(boneRenderer)
        {
            if (boneRenderer.transforms == null || boneRenderer.transforms.Length == 0)
            {
                //boneRenderer.transforms = rootBone.GetComponentsInChildren<Transform>();
            }

            boneRenderer.boneShape = BoneRenderer.BoneShape.Box;
            boneRenderer.boneColor = new Color32(107, 166, 255, 128);
        }
    }
}
