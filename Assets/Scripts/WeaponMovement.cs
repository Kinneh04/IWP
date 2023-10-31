using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.06f;
    public float smoothTime = 3.0f;
    public float bobbingAmount = 0.1f;
    public float bobbingSpeed = 0.18f;
    public float bobbingStepAmount = 0.03f;
    public float tiltAmount = 5.0f;
    public float jumpTiltAmount = 15.0f;
    private Vector3 initialPosition;
    private Vector3 FixedInitialPos;
    private float timer = 0.0f;
    public Quaternion targetRotation;
    public float VisualRecoilTiltAmount;
    public float recoilResetDuration;
    public bool canShoot = true;
    public Vector3 targetPosition;
    public Vector3 Shootforce;
    public MusicController musicController;
    public float CameraRecoil;
    public float CurrentRecoil;
    public FirstPersonController firstPersonController;

    void Start()
    {
        initialPosition = transform.localPosition;
        FixedInitialPos = initialPosition;
    }

    void Update()
    {
        // Sway Logic
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        mouseX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount);
        mouseY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount);

        Vector3 swayDelta = new Vector3(mouseX, mouseY, 0);

        targetPosition = initialPosition + swayDelta;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothTime);

        // Bobbing Logic
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed;

            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            Vector3 sway = new Vector3(translateChange, 0, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + sway, Time.deltaTime * smoothTime);
        }

       // if (Input.GetMouseButton(0) && musicController.canFire) StartCoroutine(ShootVisualRecoil());
        //if (vertical == 0)
        //{
        //    // Tilt Logic
        //    float tilt = Mathf.Lerp(transform.localRotation.z, tiltAmount, totalAxes);
        //    if (horizontal < 0) transform.localRotation = Quaternion.Euler(0, 0, tilt);
        //    else
        //        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, -tilt);
        //}
        //else
        //{
        //    float tilt = Mathf.Lerp(transform.localRotation.x, tiltAmount, totalAxes);
        //    if (vertical < 0) transform.localRotation = Quaternion.Euler(-tilt, 0, 0);
        //    else
        //        transform.localRotation = Quaternion.Euler(tilt, transform.localRotation.y, transform.localRotation.z);
        //}
    
    // transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime);
    }

    public void TryShootVisual()
    {
        initialPosition = FixedInitialPos;
        StartCoroutine(ShootVisualRecoil());
    }
    public IEnumerator  ShootVisualRecoil()
    {

        musicController.canFire = false;
        musicController.canReload = false;
        Vector3 returnPos = initialPosition;
        initialPosition -= Shootforce;
        CurrentRecoil += CameraRecoil;

        transform.localRotation = Quaternion.Euler(VisualRecoilTiltAmount, transform.localRotation.y, transform.localRotation.z);
        float i = Time.deltaTime * recoilResetDuration * 2;
        while (i > 0)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 10);
            i -= Time.deltaTime;
            initialPosition = Vector3.Lerp(initialPosition, returnPos, Time.deltaTime * recoilResetDuration);
            firstPersonController.currentRecoil = CurrentRecoil;
            CurrentRecoil = Mathf.Lerp(CurrentRecoil, 0, Time.deltaTime * recoilResetDuration);
            yield return null;
        }
        initialPosition = returnPos;
        transform.localRotation = Quaternion.identity;
       

    }
}
