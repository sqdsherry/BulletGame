using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        _animator.SetTrigger("Open");
    }

    public void OpenWinPanel()
    {
        UIManager.Instance.ShowWinPanel();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            Destroy(collision.gameObject);
    }
} 