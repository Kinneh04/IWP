using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using System.Collections;

	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		//[Tooltip("Acceleration and deceleration")]
		//public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;
		public CameraBob cameraBob;

		private float originalY;
		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;
		public int _jumpCount = 0;
		public bool _canDoubleJump = true;
		bool isJumping = false;

		[Header("Dashing")]
		public float dashDistance = 5.0f;
		public float dashDuration = 0.2f;
		public float dashCooldown = 0.0f;
		public float addToDashCooldown = 5.0f;
		public bool isDashing = false;
		public float currentRecoil;
		public bool canMove = true;
	[Header("UI elements for abilities")]
	public TMP_Text AbilityCountdownTMPText;
    public TMP_Text DashCountdownTMPText;
		public Image AbilityImage, DashImage;
	private Color OriginalColor;
	public Slider AbilityCooldownSlider, DashCooldownSlider;
	public GameObject HealGlow;
	[Header("Set Ability")]
	bool OnAbilityCooldown = false;
	public Ability CurrentAbility;
	public Transform SpawnAbilitySpawnablesFrom;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		public StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;


	[Header("HealthAndDeath")]
	public bool isLow = false;
	public bool isDead = false;
	public int Health = 100;
	public AudioSource HeartbeatAudioSource;
	public AudioClip Heartbeat;
	public AudioClip DeathAudioClip;
	public Slider HealthSlider;
	public TMP_Text HealthText;
	public float VisualHealth = 100;
	public GameObject DeathScreen;
	public GameObject MainGameUI;
	public bool isTransitioning;
    private static FirstPersonController _instance;
	float deathtimer = 2.0f;
	public float IFrameTimer = 0.0f;
	public float AddToIFrameTimer = 1.0f;

	[Header("EnemyBehindIndicator")]
	public GameObject BehindIndicator;

	[Header("AudioAndSFX")]
	public AudioSource SFXAudioSource;
	public AudioClip DashingSFX, Jump_1_SFX, Jump_2_SFX, JumpLandSFX; 
	public List<AudioClip> HurtAudioClips = new List<AudioClip>();
	public float addToWalkCooldown;
	float walkCooldown;
	public List<AudioClip> WalkSFX = new List<AudioClip>();

	public List<ParticleSystem> DashingParticleSystems = new List<ParticleSystem>();

	[Header("ChestAndUpgrades")]
	public GameObject ChestInteractUIPopup;

	[Header("Notif")]
	public TMP_Text NotifText;
	public Animator NotifAnimator;
	public AnimationClip NotifAnimationClip;

	[Header("Ground Pound")]
	public AudioClip GroundPoundSFX;
    bool hasPerformedGroundPound = false;
	public GameObject GroundPoundEffects;


    public void PopupNotif(string s)
    {
		NotifText.text = s;
		NotifAnimator.Play(NotifAnimationClip.name);
    }
	public void Cleanup()
    {
		Health = 100;
		isLow = false;
		isDashing = false;
		isDead = false;
		dashCooldown = 0;
		deathtimer = 2.0f;
		OnAbilityCooldown = false;
		
    }
    public static FirstPersonController Instance
    {
        get
        {
            // If the instance doesn't exist, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<FirstPersonController>();

                // If it still doesn't exist, create a new instance
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("FirstPersonController");
                    _instance = singletonObject.AddComponent<FirstPersonController>();
                }
            }

            return _instance;
        }
    }

	public void HealPlayer(int damage)
    {
		Health += damage;
		if (Health > 100) Health = 100;
		float volume = (1.0f - (Health / 100.0f)) / 2f;
		HeartbeatAudioSource.volume = volume;
		if (Health <= 80) isLow = true;
		else isLow = false;
	}
	public void TakeDamage(int damage)
	{
		if(isDashing)
        {
			PlayerRatingController.Instance.AddRating(15, "DASH BLOCK!", Color.green);
			return;
		}

		//Checks for iFrames
		if (isDead || isTransitioning || MusicController.Instance.isFinished || isDashing || hasPerformedGroundPound || IFrameTimer > 0) return;


		Health -= damage;
        float volume = (1.0f - (Health / 100.0f))/2f;
		HeartbeatAudioSource.volume = volume;

		SFXAudioSource.PlayOneShot(HurtAudioClips[Random.Range(0, HurtAudioClips.Count)]);

		if (Health <= 80) isLow = true;
		else isLow = false;

		if (Health <= 0)
		{
			MainGameUI.SetActive(false);
			DeathScreen.SetActive(true);
			isDead = true;
			Health = 0;
			MusicController.Instance.MusicAudioSource.Pause();
			MusicController.Instance.MusicAudioSource.time = 0;
			HeartbeatAudioSource.volume = 1;
			HeartbeatAudioSource.PlayOneShot(DeathAudioClip);
		}


    }	
	
		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			Application.targetFrameRate = 60;
			OriginalColor = AbilityImage.color;
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
			if (!CurrentAbility) CurrentAbility = GameObject.FindAnyObjectByType<Ability>();
		}

		public void UpdateAbilityCooldownText(float num)
		{
		if (!AbilityCooldownSlider) return;
		AbilityCooldownSlider.maxValue = 12;
		AbilityCooldownSlider.value = num;
		if (num > 0)
		{
            OnAbilityCooldown = true;
            AbilityCountdownTMPText.gameObject.SetActive(true);
			AbilityCountdownTMPText.text = ((int)num).ToString();
			AbilityImage.color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0.2f);
        }
		else
		{
            OnAbilityCooldown = false;
            AbilityCountdownTMPText.gameObject.SetActive(false);
			AbilityImage.color = OriginalColor;
        }
		}

		private void Start()
		{
			originalY = transform.localPosition.y;
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

	private void Update()
	{
		if (isDead || MusicController.Instance.canExit && deathtimer <= 0)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				MainMenuManager.Instance.ReturnToMainMenuFromGame();
				MusicController.Instance.SFXAudioSource.Stop();
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				MainMenuManager.Instance.RetryLevel();
			}
		}
		else if(isDead && deathtimer > 0)
        {
			deathtimer -= Time.deltaTime;
        }
		if (isTransitioning || MusicController.Instance.isFinished || isDead || !canMove) return;

			JumpAndGravity();
			GroundedCheck();
			Dashing();
			Move();
			CameraRotation();

		if (IFrameTimer > 0) IFrameTimer -= Time.deltaTime;

        if (!CurrentAbility) CurrentAbility = GameObject.FindAnyObjectByType<Ability>();


        if (dashCooldown > 0)
		{
			
			dashCooldown -= Time.deltaTime;
			DashCooldownSlider.value = dashCooldown;
            DashCountdownTMPText.text = ((int)dashCooldown).ToString();
		}
		else
		{
			DashCountdownTMPText.gameObject.SetActive(false);
			DashImage.color = OriginalColor;
		}

		if(Health != VisualHealth)
		{
			VisualHealth = Mathf.Lerp(VisualHealth, Health, 20 * Time.deltaTime);
			HealthText.text = ((int)VisualHealth).ToString();
			HealthSlider.value = VisualHealth;
		}

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit,10f))
		{

			ChestInteractUIPopup.SetActive(hit.transform.CompareTag("Chest") && !hit.transform.GetComponent<ChestScript>().isOpen);

			if(Input.GetKeyDown(KeyCode.F))
            {
				hit.transform.GetComponent<ChestScript>().OpenChest();
            }
		}
	}
		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
			isJumping = !Grounded;
		}

		private void Dashing()
        {
			if (Input.GetKeyDown(KeyCode.LeftShift) && MusicController.Instance.canFire && !isDashing && dashCooldown <= 0)
			{
				StartCoroutine(Dash());
			}
        if (Input.GetKeyDown(KeyCode.E) && MusicController.Instance.canFire && !OnAbilityCooldown)
        {
			CurrentAbility.UseAbility();
        }
    }
		IEnumerator Dash()
		{
				isDashing = true;
		HealPlayer(5);
			HealGlow.SetActive(true);
			foreach(ParticleSystem PS in DashingParticleSystems)
			{
				PS.Play();
			}
			if (!SFXAudioSource) SFXAudioSource = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();
			SFXAudioSource.PlayOneShot(DashingSFX);
			// Get the camera's forward and right vectors
			Vector3 cameraForward = Camera.main.transform.forward;
			Vector3 cameraRight = Camera.main.transform.right;
			dashCooldown = addToDashCooldown;
			DashCooldownSlider.maxValue = addToDashCooldown;
			DashCooldownSlider.value = addToDashCooldown;
			// Ignore the y component for calculations
			cameraForward.y = 0;
			cameraRight.y = 0;
			Vector3 dashDirection = transform.forward;
            // Calculate the dash direction based on the camera perspective
            if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
			{

			}
			else
			{
				dashDirection = (cameraForward.normalized * Input.GetAxis("Vertical")) + (cameraRight.normalized * Input.GetAxis("Horizontal"));
			}

			float elapsedTime = 0;

			while (elapsedTime < dashDuration)
			{
				transform.position += dashDirection * (dashDistance / dashDuration) * Time.deltaTime;
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			//UI
			DashImage.color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0.2f);
			DashCountdownTMPText.gameObject.SetActive(true);
			//End UI
				
			isDashing = false;
		yield return new WaitForSeconds(0.35f);
		HealGlow.SetActive(false);
		}
		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				
				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch +currentRecoil, 0.0f, cameraBob.Finaltilt.z * cameraBob.tiltAmount);
		//Debug.Log(cameraBob.Finaltilt.z);


	}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			//float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			//// accelerate or decelerate to target speed
			//if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			//{
			//	// creates curved result rather than a linear one giving a more organic speed change
			//	// note T in Lerp is clamped, so we don't need to clamp our speed
			//	_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			//	// round speed to 3 decimal places
			//	_speed = Mathf.Round(_speed * 1000f) / 1000f;
			//}
			//else
			//{
				_speed = targetSpeed;
			//}
			if(targetSpeed > 0)
			{
			walkCooldown -= Time.deltaTime;
			if(walkCooldown <= 0)
			{
				walkCooldown = addToWalkCooldown;
				SFXAudioSource.PlayOneShot(WalkSFX[Random.Range(0, WalkSFX.Count)]);
			}
			}
			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

    private void JumpAndGravity()
    {
        if (Grounded && !isJumping)
        {
			if(_jumpCount > 0)
			{
                SFXAudioSource.PlayOneShot(JumpLandSFX);
            }
            _jumpCount = 0;
            _canDoubleJump = true;
            _fallTimeoutDelta = FallTimeout;
            _verticalVelocity = (_verticalVelocity < 0.0f) ? -2f : _verticalVelocity;

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_jumpCount == 0 || (_jumpCount == 1 && _canDoubleJump))
            {
                isJumping = true;
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                if (_jumpCount == 0) SFXAudioSource.PlayOneShot(Jump_1_SFX);
                else SFXAudioSource.PlayOneShot(Jump_2_SFX);
                _jumpCount++;
				
                if (_jumpCount == 2) _canDoubleJump = false;
            }
        }

        _jumpTimeoutDelta -= Time.deltaTime;
        _fallTimeoutDelta -= Time.deltaTime;

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }

		if (Input.GetKey(KeyCode.LeftControl) && !Grounded && MusicController.Instance.canFire && !hasPerformedGroundPound)
		{
			hasPerformedGroundPound = true;
			StartCoroutine(PerformGroundPoundPushback());
            // Optional: Play ground pound SFX
            SFXAudioSource.PlayOneShot(Jump_1_SFX);
		}

    }



	IEnumerator PerformGroundPoundPushback()
	{

		while(!Grounded && canMove)
		{
            // Apply ground pound force
            transform.position -= transform.up * 15 * Time.deltaTime;
            yield return null;
        }
		if (!canMove) yield break;
        SFXAudioSource.PlayOneShot(GroundPoundSFX);
        Debug.Log("Player grounded after ground pound");
        hasPerformedGroundPound = false; // Reset the flag
        GroundPoundEffects.SetActive(true);
        foreach (GameObject GO in EnemySpawner.Instance.SpawnedEnemies)
		{
			if (GO)
			{
                Rigidbody otherObjectRB = GO.GetComponent<Rigidbody>(); // Replace OtherGameObject with the target GameObject
                if (otherObjectRB != null && Vector3.Distance(GO.transform.position, transform.position) <= 5.0f)
                {
                    Vector3 direction = otherObjectRB.transform.position - transform.position;
                    direction.Normalize();
                    otherObjectRB.AddForce(direction * 15.0f, ForceMode.Impulse);
                }
				GO.GetComponent<EnemyScript>().TakeDamage(10);
            }
		}
		yield return new WaitForSeconds(0.5f);
        GroundPoundEffects.SetActive(false);
    }


    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}