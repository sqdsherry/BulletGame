using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _trap;
    [SerializeField] private PathChecker pathChecker;
    [SerializeField] private float _criticalPlayerScale = 0.7f;
    [SerializeField] private float _bulletSpeed = 1f;
    
    private float _smoothing = 0.02f;
    private float _increaseRate = 0.05f;
    private float _elapsedTime = 0f;

    private GameObject _currentBullet;

    private bool _isCheckNow = false;
    private bool _isMoving = false;

    private float _bulletScale;
    private float _playerScale;
    private float _trapScale;

    private Vector3 _bulletScaleVector = new Vector3();
    private Vector3 _playerScaleVector = new Vector3();
    private Vector3 _trapScaleVector = new Vector3();

    public static Action onCheckTrap;

    private void OnEnable()
    {
        onCheckTrap += CheckTrail;
    }

    private void OnDisable()
    {
        onCheckTrap -= CheckTrail;
    }

    private void OnMouseDrag()
    {
        if (_isMoving) return;
        if (_currentBullet == null)
            return;

        if (Time.time > _elapsedTime)
        {
            IncreaseBullet();
            RedusePlayer();

            _elapsedTime = Time.time + _increaseRate;
        }
    }

    private void OnMouseDown()
    {
        if (_isMoving) return; 
        Vector3 bulletPos = new Vector3(gameObject.transform.position.x - 1.65f, gameObject.transform.position.y, gameObject.transform.position.z);
        GameObject bullet = Instantiate(_bulletPrefab, bulletPos, Quaternion.identity);
        _currentBullet = bullet;

        _playerScaleVector = gameObject.transform.localScale;
        _bulletScaleVector = bullet.transform.localScale;
        _trapScaleVector = _trap.transform.localScale;
        _playerScale = _playerScaleVector.x;
        _bulletScale = _bulletScaleVector.x;
        _trapScale = _trapScaleVector.z;
    }

    private void OnMouseUp()
    {
        if (_isMoving) return; 
        _isCheckNow = false;
        Shoot();
    }

    private void IncreaseBullet()
    {
        _bulletScale += _smoothing;
        _bulletScaleVector = new Vector3(_bulletScale, _bulletScale, _bulletScale);
        _currentBullet.transform.localScale = _bulletScaleVector;
    }

    private void RedusePlayer()
    {
        _playerScale -= _smoothing;
        _trapScale -= _smoothing;
        _playerScaleVector = new Vector3(_playerScale, _playerScale, _playerScale);
        gameObject.transform.localScale = _playerScaleVector;
        _trapScaleVector = new Vector3(_trapScaleVector.x, _trapScaleVector.y, _trapScale);
        _trap.transform.localScale = _trapScaleVector;

        if (_playerScale <= _criticalPlayerScale)
            LoseGame();
    }

    private void Shoot()
    {
        Ray ray = new Ray(gameObject.transform.position, new Vector3(-1, 0, 0));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Rigidbody rb = _currentBullet.GetComponent<Rigidbody>();
            Vector3 direction = (hit.point - gameObject.transform.position).normalized;
            rb.linearVelocity = direction * _bulletSpeed; 
        }
    }

    private void CheckTrail()
    {
        if (_isCheckNow) return;
        _isCheckNow = true;

        CheckPathAndMove();
    }

    private void MovePlayer()
    {
        if (_currentBullet != null)
        {
            Destroy(_currentBullet.gameObject);
            _currentBullet = null;
        }
        _isMoving = true;
        Transform camera = Camera.main.transform;

        Vector3 playerTargetPos = new Vector3(gameObject.transform.position.x - 25f, gameObject.transform.position.y, gameObject.transform.position.z);
        Vector3 cameraTargetPos = new Vector3(camera.position.x - 25f, camera.position.y, camera.position.z);
        StartCoroutine(PlayerMoving(playerTargetPos, cameraTargetPos, camera));
    }

    private IEnumerator PlayerMoving(Vector3 playerTargetPos, Vector3 cameraTargetPos, Transform camera)
    {
        float startY = transform.position.y;
        float jumpHeight = 3f;
        float totalDistance = Mathf.Abs(transform.position.x - playerTargetPos.x);
        float movedDistance = 0f;
        Vector3 prevPos = transform.position;
        while (transform.position.x > playerTargetPos.x) 
        {
            Vector3 nextPos = Vector3.MoveTowards(transform.position, playerTargetPos, 15 * Time.deltaTime);
            movedDistance += Mathf.Abs(nextPos.x - prevPos.x);
            prevPos = nextPos;
            float t = Mathf.Clamp01(movedDistance / totalDistance);
            float yOffset = Mathf.Sin(Mathf.PI * t) * jumpHeight;
            transform.position = new Vector3(nextPos.x, startY + yOffset, nextPos.z);
            camera.position = Vector3.MoveTowards(camera.position, cameraTargetPos, 15 * Time.deltaTime); 
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);

        Collider[] nearColliders = Physics.OverlapSphere(transform.position, 17f);
        foreach (Collider col in nearColliders)
        {
            if (col.CompareTag("Door"))
            {
                var doorCtrl = col.GetComponent<DoorController>();
                if (doorCtrl != null)
                    doorCtrl.OpenDoor();
                _isCheckNow = false;
                _isMoving = false;
                yield break;
            }
        }
        _isCheckNow = false;
        _isMoving = false;
    }

    private void CheckPathAndMove()
    {
        var (canMove, doorCollider, isWinDistance) = pathChecker.CanMove();
        if (canMove)
        {
            if (doorCollider == null)
                MovePlayer();
            else
            {
                if (isWinDistance)
                {
                    var doorCtrl = doorCollider.GetComponent<DoorController>();
                    if (doorCtrl != null)
                        doorCtrl.OpenDoor();
                }
                else
                    MovePlayer();
            }
        }
    }
  
    private void LoseGame()
    {
        UIManager.Instance.ShowLosePanel();
    }
}
