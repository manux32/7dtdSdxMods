using System;
using System.Collections.Generic;
using UnityEngine;

public class HelicoControlPanel : MonoBehaviour {
    public AudioSource MusicSound;

    float lastAudioTrigger = -1;

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
    KeyCode MusicOffOn = KeyCode.M;

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
                            TurnRight
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

        if (Input.GetKey(MusicOffOn) && Time.time - 2.0f > lastAudioTrigger)
        {
            if (MusicSound.isPlaying)
            {
                MusicSound.Stop();
                lastAudioTrigger = Time.time;
            }
            else if (hasDriver)
            {
                MusicSound.volume = 1;
                MusicSound.Play();
                lastAudioTrigger = Time.time;
            }
        }
	}
}
