using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private bool _isThrusterActive = false;
    [SerializeField]
    private float _thrusterPower = 15f;
    private float _speedMultiplier = 3;
    [SerializeField]
    private GameObject _laserPrefab; 
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _ammoPrefab;
    [SerializeField]
    private GameObject _lifePrefab;
    [SerializeField]
    private GameObject _fireBallPrefab;
    [SerializeField ]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;

   
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldsActive = false;
    [SerializeField]
    private bool _isFireBallActive = false;

    //variable for shield protection left
    [SerializeField]
    private int _shieldCharges = 3;
    private SpriteRenderer _shieldRenderer;
    

    [SerializeField]
    private int _score;

    private UIManager _uiManager;
    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;
    [SerializeField]
    private int _ammoCount = 20;
    private int _maxAmmo = 20;

    //Camera Shake
    [SerializeField]
    private float _shakeDuration = 0.3f;
    [SerializeField]
    private float _shakeMagnitude = 0.4f;
    [SerializeField]
    CameraShake _cameraShake;
   

    //variable reference to shield visualizer
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine; 
    [SerializeField]
    private GameObject _fireBallVisualizer;

    // Start is called before the first frame update
    void Start()
    {
         //Take the current position = new position (0,0,0)
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>(); 
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>(); 
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = GameObject.Find("MainCamera").GetComponent<CameraShake>();
        

        if(_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if(_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL."); 
        }

        if (_audioSource == null)
        {
            Debug.Log("Audio Source is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

        if (_shieldVisualizer == null)
        {
            Debug.LogError("Color is Null");
        }

        _shieldRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();

        if (_shieldRenderer == null)
        {
            Debug.LogError("Sprite Renderer is Null");
        }

        if (_cameraShake == null)
        {
            Debug.LogError("Main Camera is NULL.");
        }

    }

    // Update is called once per frame 
    void Update()
    {
        CalculateMovement();

       if (Input.GetKeyDown(KeyCode.Space) &&  Time.time > _canFire && _ammoCount > 0)
       {
            FireLaser();
       }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
    
        transform.Translate(direction * GetSpeed() * Time.deltaTime);

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if(transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        if (transform.position.x > 11.65)
        {
            transform.position = new Vector3(-11.65f, transform.position.y, 0);
        }
        else if(transform.position.x < -11.65f)
        {
            transform.position = new Vector3(11.65f, transform.position.y, 0);
        }

         if (Input.GetKey(KeyCode.LeftShift))
        {
            _isThrusterActive = true;
            ThrusterActive();
        }
        else
        {
            _isThrusterActive = false;
            AddFuel();
        }

        

    }

        void FireLaser()
        {

            if (_ammoCount > 0)
            {
                LessAmmo();
                _canFire = Time.time + _fireRate;
            }
            
            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else if (_isFireBallActive == true)
            {
                Debug.Log("Fireball fires.");
                _fireBallVisualizer.SetActive(true);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            }

            _audioSource.Play();
            
        }

        public void Damage()
        {

            if (_isShieldsActive == true)
            {
                _shieldCharges--;
                _shieldRenderer.color = Color.white;

                if (_shieldCharges == 3)
                {
                    _shieldRenderer.color = Color.blue;
                }
                else if (_shieldCharges == 2)
                {
                    _shieldRenderer.color = Color.yellow;
                }
                else if (_shieldCharges == 1)
                {
                    _shieldRenderer.color = Color.red;
                    
                }
                else if (_shieldCharges == 0)
                {
                    _isShieldsActive = false;
                    _shieldVisualizer.SetActive(false);
                    _shieldCharges = 3;
                }
                return;
            }

            _lives--;
            StartCoroutine(_cameraShake.Shake(_shakeDuration, _shakeMagnitude));

            if (_lives == 2)
            {
                _leftEngine.SetActive(true);
            }
            else if (_lives == 1)
            {
                _rightEngine.SetActive(true);
            }

            _uiManager.UpdateLives(_lives);

            if (_lives < 1)
            {
                _spawnManager.OnPlayerDeath();
                Destroy(this.gameObject);
            }
        }

        public void TripleShotActive()
        {
            _isTripleShotActive = true;
            StartCoroutine(TripleShotPowerDownRoutine());
        }

        IEnumerator TripleShotPowerDownRoutine()
        {
            yield return new WaitForSeconds(5.0f);
            _isTripleShotActive = false;
        }

        public void FireBallActive()
        {
            Debug.Log("Fireball is active.");
            _isFireBallActive = true;
            StartCoroutine(FireBallPowerDownRoutine());
        }

        IEnumerator FireBallPowerDownRoutine()
        {
            yield return new WaitForSeconds(5.0f);
            _isFireBallActive = false;
        }

        public void SpeedBoostActive()
        {
            _isSpeedBoostActive = true;
            _speed *= _speedMultiplier;
            StartCoroutine(SpeedBoostPowerDownRoutine());
        }

        IEnumerator SpeedBoostPowerDownRoutine()
        {
            yield return new WaitForSeconds(5.0f);
            _isSpeedBoostActive = false;
            _speed /= _speedMultiplier;
        }

        private float GetSpeed()
        {
           return (_speed * (_isThrusterActive ? 2.0f : 1.0f) * (_isSpeedBoostActive ? 2.0f : 1.0f));
        }

        private void ThrusterActive()
        {
            if (_thrusterPower > 0)
            {
                _thrusterPower -= 1.0f * Time.deltaTime;
            }
            else if (_thrusterPower <= 0)
            {
                _isThrusterActive = false;
            }

            _uiManager.UpdateThruster((int)Mathf.Round(_thrusterPower));
        }

        private void AddFuel()
        {
            if (_thrusterPower < 15 && _isThrusterActive == false)
            {
                _thrusterPower += 0.1f * Time.deltaTime;
            }
            _uiManager.UpdateThruster((int)Mathf.Round(_thrusterPower));

        }

        public void ShieldsActive()
        {
            _isShieldsActive = true;
            _shieldVisualizer.SetActive(true);
        }

        public void AddScore(int points)
        {
            _score += points;
            _uiManager.UpdateScore(_score);
        }

        public void LessAmmo()
        {
            _ammoCount--;
            _uiManager.UpdateAmmo(_ammoCount); 
        }

        public void ReloadAmmo()
        {
            _ammoCount = _maxAmmo; 
            _uiManager.UpdateAmmo(_ammoCount);
        }

        public void AddLife()
        {
            if (_lives < 3)
            {
                _lives++;
                _uiManager.UpdateLives(_lives);
            }

            if (_lives == 1)
            {
                _rightEngine.SetActive(false);
            }
            else if (_lives == 2)
            {
                _leftEngine.SetActive(false);
            }
            else if (_lives == 3)
            {
                _rightEngine.SetActive(false);
                _leftEngine.SetActive(false);
            }
        }

}
