using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraZone : MonoBehaviour
{
    [SerializeField]
    LayerMask activateLayers = new LayerMask();
    [SerializeField]
    CinemachineVirtualCamera cam;
    [SerializeField]
    ObstacleBase obstacle = null;
    [SerializeField]
    bool setLook = false;
    [SerializeField]
    bool useCustomFOV = false;
    [SerializeField]
    float fovTransitionTimeIN = .85f; 
    [SerializeField]
    float fovTransitionTimeOUT = .85f;
    [SerializeField]
    bool preFollow = true;
    [SerializeField]
    float targetFOV = 115f;
    bool triggered = false;
    static CinemachineVirtualCamera mainVCam = null;
    static float mainCamFOV = 0f;

    Tween fovTween = null;

    static CinemachineVirtualCamera lastCam = null;
    static FollowCameraZone lastZone = null;
    void OnGameStart() {
        if (mainVCam == null)
        {
            mainVCam = (CinemachineVirtualCamera)Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
            mainCamFOV = mainVCam.m_Lens.FieldOfView;
            mainVCam.Priority = 1000;
        }

        if (preFollow)
            SetTargets();
    }

    void SetTargets() {
        cam.Follow = Player.Instance.root.transform;
        if (setLook)
        {
            if (setLook)
                cam.LookAt = Player.Instance.root.transform;
        }
    }
    private void OnTriggerEnter(Collider other){
        if (GameUtil.IsInLayerMask(other.gameObject.layer, activateLayers)) {
            EnableCam();
        }
    }
     private void OnTriggerExit(Collider other){
        if (GameUtil.IsInLayerMask(other.gameObject.layer, activateLayers)) {
            DisableCam();
        }
    }

    IEnumerator TryAgain(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableCam();
    }

    void EnableCam() {
        if (triggered) return;
        if (obstacle.moveComplete || obstacle.moveBegun)
        {
            triggered = true;
            if (lastZone != null) {
                lastZone.GetComponent<Collider>().enabled = false;
            }
            if (lastCam != null){
                lastCam.Priority = 0;
                lastCam.gameObject.SetActive(false);
            }
            cam.Priority = 1000;
            mainVCam.Priority = 0;
            if (!preFollow)
                SetTargets();
            if (useCustomFOV)
                CustomFOVIn();
           lastCam = cam;
            lastZone = this;
        }
        else {
            StartCoroutine(TryAgain(.1f));
        }
    }
    void DisableCam() {
        if (!triggered) return;

        if (useCustomFOV) {
            CustomFOVOut();
        }

        cam.Priority = 0;
        mainVCam.Priority = 1000;

    }

    void CustomFOVOut() {
        float start = Camera.main.fieldOfView;
        if (fovTween != null) fovTween.Kill();
        float toFOV = mainCamFOV;
        if (fovTransitionTimeOUT <= 0)
        {
            Camera.main.fieldOfView = toFOV;
            mainVCam.m_Lens.FieldOfView = toFOV;
            cam.m_Lens.FieldOfView = toFOV;
        }
        else
        {
            fovTween = DOTween.To(
                () => cam.m_Lens.FieldOfView,
                (x) => { cam.m_Lens.FieldOfView = x; },
                toFOV, fovTransitionTimeOUT)
            .SetUpdate(UpdateType.Late)
            .ChangeStartValue(start)
            .OnUpdate(() => {
                mainVCam.m_Lens.FieldOfView = cam.m_Lens.FieldOfView;
            });
        }
    }

    void CustomFOVIn() {
        float start = Camera.main.fieldOfView;
        if (fovTween != null) fovTween.Kill();
        float toFOV = targetFOV;
        fovTween = DOTween.To(() => cam.m_Lens.FieldOfView, (x) => { cam.m_Lens.FieldOfView = x; }, toFOV, fovTransitionTimeIN)
            .SetUpdate(UpdateType.Late)
            .ChangeStartValue(start)
            .OnUpdate(()=> {
                mainVCam.m_Lens.FieldOfView = cam.m_Lens.FieldOfView;
        });

    }

    void OnLose() {
        DisableCam();
    }
    private void OnEnable()
    {
        GameManager.OnStartGame += OnGameStart;
        GameManager.OnLose += OnLose; 
    }

    private void OnDisable()
    {
        GameManager.OnStartGame -= OnGameStart;
        GameManager.OnLose -= OnLose;    
    }



}
