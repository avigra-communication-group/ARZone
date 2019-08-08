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
    
    private PlayableDirector playableDirector;
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    private Animator animator;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        // custom methods here....
        
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
                playableDirector.time = 0;
                playableDirector.Stop();
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

        // make sure it's not playing on awake except for playableDirector
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        if(playableDirector)
        {
            playableDirector.playOnAwake = true;
        }
    }
}
