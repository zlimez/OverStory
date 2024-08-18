using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepController : MonoBehaviour
{
    [Header("FootStepSounds")]
    public List<FootStepMapping> FootStepSoundMappings = new List<FootStepMapping>();
    public AudioSource audioSource; // The AudioSource component to play footstep sounds

    public void TryPlayFootStepAudio()
    {
        // Cast a ray downwards from the player's position
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 2f);
        // Debug.DrawRay(transform.position, Vector3.down, Color.red, 1.5f);

        if (hits != null)
        {

            // Create a list to store sorted hits
            List<RaycastHit> sortedHits = new List<RaycastHit>(hits);

            // Sort the hits by distance 
            sortedHits.Sort((a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit hit in sortedHits)
            {
                // Get the renderer component of the object the player is standing on
                Renderer renderer = hit.collider.GetComponent<Renderer>();

                // Check if the object has a renderer assume that it is the floor
                if (renderer != null)
                {
                    // Get the material of the object
                    Material material = renderer.material;

                    //Sometimes "(Instance)" is prepended to the material name because of some post processing code, remove it to find the actual material name
                    string actualMaterialName = material.name.Replace(" (Instance)", "");

                    // Use the material name to determine which footstep sound to play
                    AudioClip footstepSound = GetFootstepSoundByMaterial(actualMaterialName);

                    // Do something with the material information (e.g. print it to the console)
                    // Debug.Log("Player is standing on object with material: " + actualMaterialName);

                    // Play the footstep sound using the AudioSource component
                    if (footstepSound != null)
                    {
                        // Debug.Log("Playing sound");
                        audioSource.PlayOneShot(footstepSound);
                    }

                    // return;
                }
            }
        }
    }
    private AudioClip GetFootstepSoundByMaterial(string materialName)
    {
        foreach (FootStepMapping m in FootStepSoundMappings)
        {
            // Debug.Log(materialName + " Checking against " + m.GetMaterialName());
            if (m.GetMaterialNames().Contains(materialName))
            {
                return m.GetRandomSound();
            }

        }

        return null; // Return null if no footstep sound is found for the material
    }
}
