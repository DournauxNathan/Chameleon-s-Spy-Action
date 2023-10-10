using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : Interactable, IInteractable
{
    [Header("Field Of View Parameters")]
    [SerializeField] private Transform prefabFov;
    [SerializeField] private Transform endPoint;
    private FieldOfView fieldOfView;
    [Space(5)]
    [SerializeField] [Range(0f, 360f)] private float fov;
    [SerializeField] private float viewDistance;

    Vector3 aimDir;
    [SerializeField] private Animator _anim;
    public bool isPlayerDetected;


    private void Start()
    {
        Reset();

        if (player == null) player = GameObject.Find("Player").GetComponent<PlayerController>();
        
        fieldOfView = Instantiate(prefabFov, null).GetComponent<FieldOfView>();
        fieldOfView.SetFoV(fov);
        fieldOfView.SetViewDistance(viewDistance);
    }

    private void Update()
    {
        Vector3 targetPosition = endPoint.position;
        aimDir = (targetPosition - transform.position).normalized;

        if (fieldOfView != null)
        {
            fieldOfView.SetOrigin(transform.position);
            fieldOfView.SetAimDirection(aimDir);
        }
        FindTargetPlayer();
    }

    public void FindTargetPlayer()
    {
        Debug.Log(Vector3.Distance(GetPosition(), player.GetPosition()));

        if (Vector3.Distance(GetPosition(), player.GetPosition()) < viewDistance)
        {
            Vector3 dirToPlayer = (player.GetPosition() - GetPosition()).normalized;

            if (Vector3.Angle(aimDir, dirToPlayer) < fov /2f)
            {
                Alert();

            }
            else
            {

                isPlayerDetected = false;
            }
        }
        
    }

    public void Alert()
    {
        isPlayerDetected = true;
    }

    #region Interactable interface
    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        Toogle();
    }

    public void Timer()
    {
        throw new System.NotImplementedException();
    }

    public void Toogle()
    {
        if (isActive)
        {
            isActive = false;
            _anim.enabled = false;

            StartCoroutine(DecreaseTimer());
            onDeactivate?.Invoke();
        }
        else if (!isActive)
        {
            isActive = true;
            _anim.enabled = true;
            onActivate?.Invoke();
            coolDown = resetTimer;
        }

    }

    #endregion

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, endPoint.position);
    }
}
