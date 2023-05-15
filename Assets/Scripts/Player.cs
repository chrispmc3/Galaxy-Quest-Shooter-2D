using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10.5f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab; 
    [SerializeField]
    private GameObject _tripleShotPrefab;
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
    private int _score;

    private UIManager _uiManager;
    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    //variable reference to shield visualizer
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine; 

    // Start is called before the first frame update
    void Start()
    {
         //Take the current position = new position (0,0,0)
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>(); 
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>(); 
        _audioSource = GetComponent<AudioSource>();

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

    }

    // Update is called once per frame 
    void Update()
    {
        CalculateMovement();

       if (Input.GetKeyDown(KeyCode.Space) &&  Time.time > _canFire)
       {
            FireLaser();
       }

    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
    
        transform.Translate(direction * _speed * Time.deltaTime);
       

        transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);

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
    }

        void FireLaser()
        {
            
            _canFire = Time.time + _fireRate;

            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
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
                _isShieldsActive = false;
                _shieldVisualizer.SetActive(false);
                return;
            }

            _lives--;

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
            //tripleShotActive becomes true
            _isTripleShotActive = true;
            //start the power down coroutine for triple shot
            StartCoroutine(TripleShotPowerDownRoutine());
        }

        //IEnumerator TripleShotPowerDownRoutine
        IEnumerator TripleShotPowerDownRoutine()
        {
            yield return new WaitForSeconds(5.0f);
            _isTripleShotActive = false;
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

        public void ShieldsActive()
        {
            _isShieldsActive = true;
            _shieldVisualizer.SetActive(true);
            //enable the visualizer
        }


        public void AddScore(int points)
        {
            _score += points;
            _uiManager.UpdateScore(_score);
        }

}
