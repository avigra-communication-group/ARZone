// Default Trackable Event Handler versi AR Zone

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;
using Vuforia;

public enum AnimationType {
    ModelAnimation,
    VideoAnimation,
    DirectorAnimation
}

public class ARZoneTrackableEventHandler : DefaultTrackableEventHandler
{
    public AnimationType animationType;
    [Header("Object yang dapat dipindahkan ke kamera.")]
    public GameObject target;
    private Transform targetParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    private PlayableDirector playableDirector;
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    private Animator animator;
    private bool objectIsPicked = false;

    protected override void OnTrackingFound()
    {
        if(ARCameraTargetPicker.togglePick)
        {
            Debug.Log("Can not play until there is no object exists in camera.");
            return;
        }
        base.OnTrackingFound();

        // custom methods here....

        ARCameraTargetPicker.onPicked += SendObjectToCam;
        ARCameraTargetPicker.onPickReleased += GetBackObjectFromCam;

        if (animationType == AnimationType.ModelAnimation && !audioSource)
        {
            Debug.LogError("Please add Audio Source component to target");
            return;
        }

        if(animationType == AnimationType.VideoAnimation && !videoPlayer)
        {
            Debug.LogError("Please add Video Player component to target");
            return;
        }

        if (animationType == AnimationType.DirectorAnimation && !playableDirector)
        {
            Debug.LogError("Please add Director component to target");
            return;
        }

        switch(animationType)
        {
            case AnimationType.ModelAnimation:
                audioSource.Play();
                break;
            case AnimationType.VideoAnimation:
                videoPlayer.Play();
                break;
            case AnimationType.DirectorAnimation:
                playableDirector.Play();
                break;
            default:
                break;
        }
    }

    protected override void OnTrackingLost()
    {
        if(objectIsPicked)
        return;

        base.OnTrackingLost();

        // custom methods here...
        

        if (animationType == AnimationType.ModelAnimation && !audioSource)
        {
            Debug.LogError("Please add Audio Source component to target");
            return;
        }

        if (animationType == AnimationType.VideoAnimation && !videoPlayer)
        {
            Debug.LogError("Please add Video Player component to target");
            return;
        }

        if (animationType == AnimationType.DirectorAnimation && !playableDirector)
        {
            Debug.LogError("Please add Director component to target");
            return;
        }

        switch (animationType)
        {
            case AnimationType.ModelAnimation:
                audioSource.Stop();
                animator.Rebind();
                break;
            case AnimationType.VideoAnimation:
                videoPlayer.Stop();
                break;
            case AnimationType.DirectorAnimation:
                playableDirector.Stop();
                //playableDirector.time = 0;
                break;
            default:
                break;
        }
    }

    protected override void Start()
    {
        base.Start();

        // custom methods here....
        playableDirector = GetComponentInChildren<PlayableDirector>();
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        audioSource = GetComponentInChildren<AudioSource>();
        animator = GetComponentInChildren<Animator>();

        // make sure it's not playing on awake
        if(videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
        }
        if(audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
        if(playableDirector != null)
        {
            playableDirector.playOnAwake = false;
        }

        targetParent = target.transform.parent;
        originalPosition = target.transform.localPosition;
        originalRotation = target.transform.localRotation;
        originalScale = target.transform.localScale;
    }

    // custom method here...

    private void OnEnable() {
        
    }

    private void OnDisable() {
        ARCameraTargetPicker.onPicked -= SendObjectToCam;
        ARCameraTargetPicker.onPickReleased -= GetBackObjectFromCam;
    }

    public GameObject SendObjectToCam()
    {
        if(!target.activeInHierarchy)
        {
            return null;
        }
        objectIsPicked = true;
        ARCameraTargetPicker.animationType = animationType;
        return target;
    }

    public void GetBackObjectFromCam(GameObject go)
    {
        if(go != target)
        {
            return;
        }
        target = go;
        target.transform.localPosition = originalPosition;
        target.transform.localRotation = originalRotation;
        target.transform.localScale = originalScale;
        target.transform.SetParent(targetParent,false);
        objectIsPicked = false;
        OnTrackingLost();
    }
}
