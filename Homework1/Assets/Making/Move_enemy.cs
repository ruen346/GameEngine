using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_enemy : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
    public float m_Speed = 12f;                 // How fast the tank moves forward and back.
    public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
    public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
    public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
    public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

    private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
    private string m_TurnAxisName;              // The name of the input axis for turning.
    private Rigidbody m_Rigidbody;              // Reference used to move the tank.
    private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
    private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks


    /// <summary>
    bool crash = false;

    float save_x = 0;
    float save_z = 0;

    /// </summary>




    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
        // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
        // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
        m_particleSystems = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Play();
        }
    }


    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;

        // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
        for (int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Stop();
        }
    }


    IEnumerator Start()
    {
        save_x = transform.position.x;
        save_z = transform.position.z;

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            
            if (Mathf.Abs(transform.position.x - save_x) + Mathf.Abs(transform.position.z - save_z) > 1)
                crash = false;
            else
                crash = true;

            save_x = transform.position.x;
            save_z = transform.position.z;
        }
    }


    private void Update()
    {
        EngineAudio();
    }


    private void EngineAudio()
    {

        // Otherwise if the tank is moving and if the idling clip is currently playing...
        if (m_MovementAudio.clip == m_EngineIdling)
        {
            // ... change the clip to driving and play.
            m_MovementAudio.clip = m_EngineDriving;
            //m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
            m_MovementAudio.Play();
        }
    }

    private void FixedUpdate()
    {
        // 이동
        if (crash == false)
        {
            Vector3 movement = transform.forward * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }
        else
        {
            Vector3 movement = transform.forward * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position - movement);
        }

        // 회전
        if (crash == true)
        {
            Debug.Log("으악");
            float turn = m_TurnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        }
    }
}
