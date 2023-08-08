using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private bool locked;
    private float delay = 1f;
    private float delayTimer = 0f;

    [DrawIf("locked", true)]
    [SerializeField]
    private string keyCode;


    // Start is called before the first frame update
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    void Start()
    {
        try
        { animator = GetComponent<Animator>(); }
        catch
        { Debug.LogError("Door script must have Animator component attached to same object!"); }
    }
    
    // Need I comment? This shit is pretty self-explanatory
    public bool IsLocked()
    { return locked; }

    public string GetKeyCode()
    { return keyCode; }

    public void Unlock()
    { locked = false; }

    public void ToggleOpen()
    {
        if (delayTimer >= delay)
        {
            if (animator.GetBool("open"))
                animator.SetBool("open", false);
            else
                animator.SetBool("open", true);

            delayTimer = 0f;
        }
    }

    private void Update()
    {
        if (delayTimer < delay)
            delayTimer += Time.deltaTime;
    }
}
