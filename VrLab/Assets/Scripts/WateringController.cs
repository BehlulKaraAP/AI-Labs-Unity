using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WateringController : MonoBehaviour
{
    public ParticleSystem waterParticles;
    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    void OnReleased(SelectExitEventArgs args)
    {
        isHeld = false;
        if (waterParticles.isPlaying)
        {
            waterParticles.Stop();
        }
    }

    void Update()
    {
        if (isHeld)
        {
            float tilt = Vector3.Dot(transform.up, Vector3.down);

            if (tilt > 0.4f)
            {
                if (!waterParticles.isPlaying)
                    waterParticles.Play();
            }
            else
            {
                if (waterParticles.isPlaying)
                    waterParticles.Stop();
            }
        }
    }
}
