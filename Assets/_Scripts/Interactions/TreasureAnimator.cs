using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Open();
        
    }
    private void OnTriggerExit(Collider other)
    {
        Close();
    }

    public void Open()
    {
        if (_animator == null) return;
        _animator.SetBool("open", true);
    }
    public void Close()
    {
        if (_animator == null) return;
        _animator.SetBool("open", false);
    }
}
