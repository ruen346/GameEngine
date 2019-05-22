using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.


    private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
    private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
    private float m_CurrentHealth;                      // How much health the tank currently has.
    private bool m_Dead;




    public int m_PlayerNumber = 0;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
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
    bool find = false;

    float save_x = 0;
    float save_z = 0;

    /// </summary>
    
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
    public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

    private float m_CurrentLaunchForce;         // 발사강도
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.


    public GameObject main_tank;





    public void TakeDamage(float amount)
    {
        // Reduce current health by the amount of damage done.
        m_CurrentHealth -= amount;

        // Change the UI elements appropriately.
        SetHealthUI();

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        m_Slider.value = m_CurrentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Set the flag so that this function is only called once.
        m_Dead = true;

        // Move the instantiated explosion prefab to the tank's position and turn it on.
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        // Play the particle system of the tank exploding.
        m_ExplosionParticles.Play();

        // Play the tank explosion sound effect.
        m_ExplosionAudio.Play();

        // Turn the tank off.
        gameObject.SetActive(false);
    }





    private void Awake()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        // Get a reference to the audio source on the instantiated prefab.
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required.
        m_ExplosionParticles.gameObject.SetActive(false);





        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        // When the tank is enabled, reset the tank's health and whether or not it's dead.
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        // Update the health slider's value and color.
        SetHealthUI();






        m_Rigidbody.isKinematic = false;
        
        m_particleSystems = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Play();
        }



        //////////////////////////////////
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
        
        for (int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Stop();
        }
    }


    IEnumerator Start()
    {
        main_tank = GameObject.FindWithTag("main");



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



            ////////////////////////////////////
            float far = Mathf.Sqrt(Mathf.Pow(main_tank.transform.position.x - transform.position.x, 2) + Mathf.Pow(main_tank.transform.position.z - transform.position.z, 2));

            if (far < 16)
                find = true;
            else
                find = false;


            if (find == true)
            {
                m_CurrentLaunchForce = 10 + (far / 2);
                Fire();
            }
        }
    }


    private void Update()
    {
        EngineAudio();
    }


    private void EngineAudio()
    {
        if (m_MovementAudio.clip == m_EngineIdling)
        {
            m_MovementAudio.clip = m_EngineDriving;
            m_MovementAudio.Play();
        }
    }

    private void FixedUpdate()
    {
        if (find == false)
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
                float turn = m_TurnSpeed * Time.deltaTime;
                Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
                m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
            }
        }
        else
        {
            // 이동
            Vector3 movement = transform.forward * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

            // 회전
            float turn = 0;

            float turn_degree;

            float vector_x = Mathf.Abs(Mathf.Abs(main_tank.transform.position.x) - Mathf.Abs(transform.position.x));
            float vector_z = Mathf.Abs(Mathf.Abs(main_tank.transform.position.z) - Mathf.Abs(transform.position.z));

            float big_z = vector_z / (vector_x + vector_z) * 90;

            if (main_tank.transform.position.x - transform.position.x < 0)
            {
                if (main_tank.transform.position.z - transform.position.z < 0)
                { 
                    turn_degree = -90 - big_z;
                }
                else
                {
                    turn_degree = -90 + big_z;
                }
            }
            else
            {
                if (main_tank.transform.position.z - transform.position.z < 0)
                {
                    turn_degree = 90 + big_z;
                }
                else
                {
                    turn_degree = 90 - big_z;
                }
            }

            Debug.Log(transform.eulerAngles.y);

            float euler_y;

            if (transform.eulerAngles.y > 180)
                euler_y = transform.eulerAngles.y - 360;
            else
                euler_y = transform.eulerAngles.y;

            if (Mathf.Abs(Mathf.Abs(euler_y) - Mathf.Abs(turn_degree)) > 5)
            {
                if (euler_y < turn_degree)
                    turn = 180 * Time.deltaTime;
                else
                    turn = -180 * Time.deltaTime;
            }
            else if(turn_degree - euler_y > 180)
            {
                turn = -180 * Time.deltaTime;
            }
            else if (turn_degree - euler_y < -180)
            {
                turn = 180 * Time.deltaTime;
            }
            else
                turn = 0;

            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        }
    }




    /// //////////////////////////////////////////////////

    private void Fire()
    {
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Reset the launch force.  This is a precaution in case of missing button events.
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}
