using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerInput : MonoBehaviour , IShooter
{
    [SerializeField] InputContainer inputsData;
    [SerializeField] PlayerData playerData;
    [SerializeField] Transform _shootPosition;
    [SerializeField] SpriteRenderer spriteCharacter;
    [SerializeField] Transform pointer;
    [SerializeField] Animator animator;
    [SerializeField] Collider collider;
    [Space]
    [SerializeField] Animator dxf;
    [SerializeField] Animator dxb, sxf, sxb;
    [SerializeField] SpriteRenderer dxfR, dxbR, sxfR, sxbR;

    Rigidbody rb;

    public Vector3 shootPosition { get { return _shootPosition.position; } }
    public Vector3 aimDirection { get; private set; }

    public System.Action OnDestroy { get; set; }
    public System.Action<InputData> OnInputPressed { get; set; }
    public System.Action OnInputReset { get; set; }

    public List<SetSequences> sequences { get; private set; }
    public List<SetSequences> currentSequencesSet { get; private set; }
    public List<SetSequences> sequencesToRemove;
    SetSequences executedSequence;
    public List<InputData> currentInputSequence { get; private set; }

    public InputData currentInput { get; private set; }

    bool sequenceStarted = false;
    int consecutiveButtonPressed = -1;
    bool buttonJustPressed = false;
    float sequenceRemainTime;
    Animator[] animators;

    #region Mono
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        sequences = new List<SetSequences>(); 
        currentSequencesSet = new List<SetSequences>();
        currentInputSequence = new List<InputData>();
        sequencesToRemove = new List<SetSequences>();
        foreach (var sequenceData in playerData.sequences)
        {
            SetSequences sequence = new SetSequences(sequenceData, this, this);
            sequence.onStartSequence    += StartSequence;
            sequence.onCompletedSection += OnCorrectSequence;
            sequence.onExecute          += Attack;
            foreach (var section in sequence.commands)
            {
                section.onCorrectInput  += OnCorrectInput;
            }
            sequences.Add(sequence);
        }

        animators = new Animator[] { dxb, dxf, sxb, sxf };

        SetRendererActive(AnimDirection.sxf);
    }

    private void OnDisable()
    {
        OnDestroy?.Invoke();
        foreach (var sequence in sequences)
        {
            sequence.ResetSequence();
            sequence.onStartSequence    -= StartSequence;
            sequence.onCompletedSection -= OnCorrectSequence;
            sequence.onExecute          -= Attack;
            foreach (var section in sequence.commands)
            {
                section.onCorrectInput  -= OnCorrectInput;
            }
        }
    }

    private void Update()
    {
        HandleSequence();
        Aim();
        HandleFire();
        HandleDodge();
    }

    private void FixedUpdate()
    {
        Movement();
    }
    #endregion

    #region API
    public void SetRendererActive(AnimDirection _anim)
    {
        dxfR.enabled = false; ; dxbR.enabled = false; sxbR.enabled = false; sxfR.enabled = false;
        switch (_anim)
        {
            case AnimDirection.dxf:
                dxfR.enabled = true;
                break;
            case AnimDirection.dxb:
                dxbR.enabled = true;
                break;
            case AnimDirection.sxf:
                sxfR.enabled = true;
                break;
            case AnimDirection.sxb:
                sxbR.enabled = true;
                break;
            default:
                break;
        }
    }

    public void SetAnim(string _string)
    {
        foreach (var anim in animators)
        {
            anim.SetTrigger(_string);
        }
    }

    public void SetAnim(string _string, bool _bool)
    {
        foreach (var anim in animators)
        {
            anim.SetBool(_string, _bool);
        }
    }
    #endregion

    #region OtherInputHandler
    bool canDash = true;

    IEnumerator dodgeCorutine;
    private void HandleDodge()
    {
        if ((Input.GetKeyDown(playerData.dodgeKeyboard) || Input.GetAxis("Fire1") < 0) && canDash && canMove)
        {
            dodgeCorutine = Dodge();
            StartCoroutine(dodgeCorutine);
        }
    }

    float dodgeTimer = 0;
    IEnumerator Dodge()
    {
        canDash = false;
        canMove = false;
        rb.useGravity = false;
        collider.isTrigger = true;
        
        playerData.TempInvulnerability(playerData.dodgeDuration);
        Vector3 dodgeVelocity;
        if (usingJoypad)
            dodgeVelocity = stickAxis.normalized * playerData.dodgeSpeed * Time.deltaTime;
        else
            dodgeVelocity = keyAxis.normalized * playerData.dodgeSpeed * Time.deltaTime;

        RaycastHit hit;
        while (dodgeTimer < playerData.dodgeDuration)
        {
            dodgeTimer += Time.deltaTime;
            if (Physics.Raycast(transform.position, dodgeVelocity.normalized, out hit, 0.8f))
            {
                if (hit.transform.gameObject.tag == "Wall")
                {
                    yield return null;
                    continue;
                }
            }
            transform.Translate(dodgeVelocity, Space.World);
            yield return null;
        }
        dodgeTimer = 0;

        rb.useGravity = true;
        collider.isTrigger = false;
        canMove = true;

        yield return new WaitForSeconds(playerData.dodgeCooldown - playerData.dodgeDuration);
        canDash = true;
        yield return null;
    }

    bool usingJoypad = false;
    bool canMove = true;
    bool isMove = false;

    Vector3 stickAxis;
    Vector3 keyAxis;
    Vector3 stickDirection;
    Vector3 transformVelocity;
    void Movement()
    {
        stickAxis = new Vector3(Input.GetAxis("HorizontalStick"), 0, Input.GetAxis("VerticalStick"));
        keyAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (usingJoypad)
            transformVelocity = stickAxis.normalized * playerData.speed * Time.deltaTime;
        else
            transformVelocity = keyAxis.normalized * playerData.speed * Time.deltaTime;

        if (canMove)
        {
            transform.Translate(transformVelocity, Space.World);
        }
        
        if (animator != null)
        {
            if (transformVelocity == Vector3.zero)
            {
                if (isMove == true)
                {
                    isMove = false;
                    animator.SetTrigger("Idle");
                    SetAnim("Move", isMove);
                }
            }
            else
            {
                if (isMove == false)
                {
                    isMove = true;
                    SetAnim("Move", isMove);
                }

                if (transformVelocity.z > 0f)
                {
                    if (transformVelocity.x < 0f)
                    {
                        animator.SetTrigger("SXB");
                    }
                    else if (transformVelocity.x >= 0f)
                    {
                        animator.SetTrigger("DXB");
                    }
                }
                else if (transformVelocity.z <= 0f)
                {
                    if (transformVelocity.x < 0f)
                    {
                        animator.SetTrigger("SXF");
                    }
                    else if (transformVelocity.x >= 0f)
                    {
                        animator.SetTrigger("DXF");
                    }
                }
            }
        }

        if ((stickAxis.x < -0.1f || stickAxis.x > 0.1f) || (stickAxis.z < -0.1f || stickAxis.z > 0.1f))
        {
            stickDirection = Quaternion.LookRotation(stickAxis) * Vector3.forward;
        }

        if (spriteCharacter)
        {
            if (stickAxis.x < 0 && spriteCharacter.flipX == false)
            {
                spriteCharacter.flipX = true;
            }
            else if (stickAxis.x > 0 && spriteCharacter.flipX == true)
            {
                spriteCharacter.flipX = false;
            }
        }
    }

    Vector2 mouseVelocity;
    void Aim()
    {
        mouseVelocity = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if ((stickAxis.x < -0.1f || stickAxis.x > 0.1f) || (stickAxis.z < -0.1f || stickAxis.z > 0.1f))
        {
            aimDirection = stickDirection;
            usingJoypad = true;
            Debug.Log("joypad");
        }

        if ((mouseVelocity.x < -0.1f || mouseVelocity.x > 0.1f || mouseVelocity.y < -0.1f || mouseVelocity.y > 0.1f ||
            keyAxis.x < -0.1f || keyAxis.x > 0.1f || keyAxis.y < -0.1f || keyAxis.y > 0.1f || Input.GetKeyDown(KeyCode.Mouse0)) && usingJoypad == true)
        {
            usingJoypad = false;
            Debug.Log("mouse");
        }

        if (usingJoypad == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity))
            {
                Vector3 raycastPoint = raycastHit.point;
                Vector3 pointOnPlayerHight = new Vector3(raycastPoint.x, transform.position.y, raycastPoint.z);
                Vector3 _direction = pointOnPlayerHight - transform.position;
                aimDirection = _direction.normalized;
            }
        }

        if (pointer)
            pointer.rotation = Quaternion.LookRotation(aimDirection, Vector3.forward) * Quaternion.Euler(0, -90, 0);
    }

    bool shooted;
    void HandleFire()
    {
        if (BulletPoolManager.instance != null)
        {
            if (Input.GetAxis("Fire1") > 0 && shooted == false)
            {
                shooted = true;
                BulletPoolManager.instance.Shoot(playerData.bullet, _shootPosition.position, aimDirection, this, null);
                Attack();
            }
            else if (Input.GetAxis("Fire1") <= 0 && shooted == true)
            {
                shooted = false;
            }

            if (Input.GetButtonDown("Fire2"))
            {
                BulletPoolManager.instance.Shoot(playerData.bullet, _shootPosition.position, aimDirection, this, null);
                Attack();
            }
        }
    } 

    void Attack(SetSequences set = null)
    {
        SetAnim("Attack");
    }
    #endregion

    #region Sequence
    void HandleSequence()
    {
        buttonJustPressed = false;

        if (sequenceStarted == false)
        {
            HandleInput();
            foreach (var sequence in sequences)
            {
                if (sequence.canExecute)
                    sequence.HandleSetSequences();
            }
        }
        else
        {
            HandleInput();
            foreach (var sequence in sequencesToRemove)
            {
                currentSequencesSet.Remove(sequence);
            }
            sequencesToRemove.Clear();

            foreach (var sequence in currentSequencesSet)
            {
                sequence.HandleSetSequences();
            }
        }

        if (sequenceStarted == true && (Time.time > sequenceRemainTime))
        {
             ExecuteSequence();
        }
    }

    void HandleInput()
    {
        if (Input.anyKeyDown)
        {
            foreach (var input in inputsData.inputs)
            {
                if (input.CheckInputPressed())
                {
                    if (sequenceStarted == false)
                    {
                        sequenceStarted = true;
                    }
                    currentInputSequence.Add(input);
                    OnInputPressed?.Invoke(input);
                    buttonJustPressed = true;
                    sequenceRemainTime = Time.time + playerData.timeForSequence;
                }
            }
        }
    }

    void StartSequence(SetSequences s)
    {
        if (sequenceStarted == false)
        {
            sequenceStarted = true;
        }
        currentSequencesSet.Add(s);
    }

    void OnCorrectSequence(SetSequences s)
    {
        executedSequence = s;
    }

    void OnCorrectInput(InputData input)
    {
        if (!buttonJustPressed)
        {
            currentInputSequence.Add(input);
            consecutiveButtonPressed++;
            buttonJustPressed = true;
        }
        sequenceRemainTime = Time.time + playerData.timeForSequence;
    }

    void ResetSequences()
    {
        consecutiveButtonPressed = -1;
        foreach (var sequence in currentSequencesSet)
        {
            sequence.ResetSequence();
        }
        currentSequencesSet.Clear();
        currentInputSequence.Clear();
        OnInputReset?.Invoke();
        sequenceStarted = false;
    }

    void ExecuteSequence()
    {
        if (executedSequence != null)
        {
            if (currentSequencesSet.Contains(executedSequence))
            {
                executedSequence.Execute();
                StopCoroutine(executedSequence.cooldownCorutine);
                executedSequence.RestartCooldownCorutine();
                StartCoroutine(executedSequence.cooldownCorutine);
                executedSequence = null;
            }
        }
        ResetSequences();
    }
    #endregion

    public enum AnimDirection
    {
        dxf, dxb, sxf, sxb,
    }
}