using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadableImage : MonoBehaviour
{
    [SerializeField] Transform loadableTransform;
    [SerializeField] Image fakeImage;
    [SerializeField] Image originalImage;
    [SerializeField] float speed;

    public Image OriginalImage => originalImage;
    bool isSeted = false;

    public bool IsSeted
    {
        get
        {
            return isSeted;
        }
        set
        {
            isSeted = value;
            fakeImage.gameObject.SetActive(!value);
            loadableTransform.gameObject.SetActive(!value);
            originalImage.gameObject.SetActive(value);
        }
    }


    public void Initialize()
    {
        IsSeted = false;
    }


    public void Deinitialize()
    {

    }


    private void Update()
    {
        if (!IsSeted)
        {
            loadableTransform.Rotate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}
