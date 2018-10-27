using System;
using System.Collections.Generic;
using UnityEngine;


public enum PressedKeyCode
{
    SpeedUpPressed,
    SpeedDownPressed,
    ForwardPressed,
    BackPressed,
    LeftPressed,
    RightPressed,
    TurnLeftPressed,
    TurnRightPressed
}

public class HelicoControlPanel : MonoBehaviour
{
    public Entity entity;
    EntityCustomHelicopter entityHelico;

    float lastAudioTrigger = -1;
    bool isMusicOn = false;

    static bool showDebugLog = false;

    public static void DebugMsg(string msg)
    {
        if (showDebugLog)
        {
            Debug.Log(msg);
        }
    }

    [SerializeField]
    KeyCode SpeedUp = KeyCode.LeftShift;
    [SerializeField]
    KeyCode SpeedDown = KeyCode.Space;
    [SerializeField]
    KeyCode Forward = KeyCode.W;
    [SerializeField]
    KeyCode Back = KeyCode.S;
    [SerializeField]
    KeyCode Left = KeyCode.A;
    [SerializeField]
    KeyCode Right = KeyCode.D;
    [SerializeField]
    KeyCode TurnLeft = KeyCode.LeftArrow;
    [SerializeField]
    KeyCode TurnRight = KeyCode.RightArrow;
    [SerializeField]
    KeyCode MusicOffOn = KeyCode.Backspace;

    private KeyCode[] keyCodes;

    public Action<PressedKeyCode[]> KeyPressed;
    private void Awake()
    {
        keyCodes = new[] {
                            SpeedUp,
                            SpeedDown,
                            Forward,
                            Back,
                            Left,
                            Right,
                            TurnLeft,
                            TurnRight
                        };
    }

    public void Start ()
    {
        entityHelico = entity as EntityCustomHelicopter;
        lastAudioTrigger = Time.time;
    }

    void FixedUpdate ()
    {
	    var pressedKeyCode = new List<PressedKeyCode>();
	    for (int index = 0; index < keyCodes.Length; index++)
	    {
	        var keyCode = keyCodes[index];
	        if (Input.GetKey(keyCode))
                pressedKeyCode.Add((PressedKeyCode)index);
	    }

	    if (KeyPressed != null)
	        KeyPressed(pressedKeyCode.ToArray());

        if (entityHelico == null)
        {
            //DebugMsg("entityHelico == null!");
            entityHelico = entity as EntityCustomHelicopter;
            return;
        }

        if (!entityHelico.hasDriver)
            return;

        if (Input.GetKey(MusicOffOn) && Time.time - 1.0f > lastAudioTrigger)
        {
            DebugMsg(entityHelico.entityId.ToString() + " Music Toggle pressed: " + Time.time.ToCultureInvariantString() + " | lastAudioTrigger = " + lastAudioTrigger.ToCultureInvariantString());
            if (isMusicOn)
            {
                DebugMsg(entityHelico.entityId.ToString() + " Stopping Music: " + Time.time.ToCultureInvariantString());
                entityHelico.helicoMusic.Stop();
                //Audio.Manager.Stop(entityHelico.entityId, "Ambient_Loops/helicopter_music");
                lastAudioTrigger = Time.time;
                isMusicOn = false;
            }
            else 
            {
                DebugMsg(entityHelico.entityId.ToString() + " Starting Music: " + Time.time.ToCultureInvariantString());
                entityHelico.helicoMusic.volume = 0.6f;
                entityHelico.helicoMusic.Play();
                //Audio.Manager.Play(entityHelico, "Ambient_Loops/helicopter_music");
                lastAudioTrigger = Time.time;
                isMusicOn = true;
            }
        }
	}
}
