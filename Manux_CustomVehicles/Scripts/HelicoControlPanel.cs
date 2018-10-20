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
    TurnRightPressed,
    ToggleFirstThirdPersonPressed
}

public class HelicoControlPanel : MonoBehaviour {
    public AudioSource MusicSound;

    float lastAudioTrigger = -1;
    bool isMusicOn;

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
    KeyCode ToggleFirstThirdPerson = KeyCode.F5;
    [SerializeField]
    KeyCode MusicOffOn = KeyCode.Backspace;

    public Entity entityHelico;
    public bool hasDriver = false;

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
                            TurnRight,
                            ToggleFirstThirdPerson
                        };
    }

    void Start ()
    {
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

        if (Input.GetKey(MusicOffOn) && Time.time - 1.0f > lastAudioTrigger)
        {
            //if (entityHelico != null)
            if(MusicSound != null)
            {
                if (MusicSound.isPlaying)
                {
                    MusicSound.Stop();
                    //Audio.Manager.Stop(entityHelico.entityId, "Ambient_Loops/helicopter_music");
                    lastAudioTrigger = Time.time;
                    isMusicOn = false;
                }
                else if (hasDriver)
                {
                    MusicSound.volume = 1;
                    MusicSound.Play();
                    //Audio.Manager.Play(entityHelico, "Ambient_Loops/helicopter_music");
                    lastAudioTrigger = Time.time;
                    isMusicOn = true;
                }
            }
        }
	}
}
