using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PictureJewel : MonoBehaviour
{
    private bool isPictureTouched = false;
    private Quaternion initialRotation;
    [SerializeField] private GameObject jewelInformations;
    [SerializeField] private GameObject picture;
    Coroutine rotatePictureCor;
    public Action<bool> OnPictureRotation;

    public float rotationDuration = 1f; // Durata della rotazione

    // Start is called before the first frame update
    void Start()
    {
        isPictureTouched = false;
        jewelInformations.SetActive(false);
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TouchPicture();
        }
    }

    public void TouchPicture()
    {
        Debug.Log("Picture touched " + isPictureTouched);
        if (picture.activeSelf)
        {
            isPictureTouched = true;
            rotatePictureCor = StartCoroutine(RotatePicture(180,picture, jewelInformations));
        }
        else if(jewelInformations.activeSelf)
        {
            isPictureTouched = false;
            rotatePictureCor = StartCoroutine(RotatePicture(-180, jewelInformations, picture));
            //jewelInformations.SetActive(false);
            //picture.SetActive(true);
        }

    }

    private IEnumerator RotatePicture(float angle, GameObject toHide, GameObject toShow)
    {
        OnPictureRotation?.Invoke(true);
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, angle, 0);
        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;

            if (elapsed >= rotationDuration * 0.5f) // Activate toShow near the end of rotation
            {
                toHide.SetActive(false);
                toShow.SetActive(true);
            }

            yield return null;
        }

        if(toHide.activeSelf)
            toHide.SetActive(false);
        if(!toShow.activeSelf)
            toShow.SetActive(true);

        transform.rotation = endRotation;
        OnPictureRotation?.Invoke(false);
    }

    public bool IsPictureTouched()
    {
        return isPictureTouched;
    }

    public void ResetPicture()
    {
        if (rotatePictureCor != null)
        {
            StopCoroutine(rotatePictureCor);
        }
        isPictureTouched = false;
        transform.rotation = initialRotation;
    }
}
