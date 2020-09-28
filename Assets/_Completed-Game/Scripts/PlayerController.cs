using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;

#if UNITY_IOS || UNITY_TVOS
public class NativeAPI {
    [DllImport("__Internal")]
    public static extern void sendMessageToMobileApp(string message);
}
#endif

public class PlayerController : MonoBehaviour {
	public float speed;
	public Text countText;
	public Text winText;

	private Rigidbody rb;
	private int count;

	void Start () {
		rb = GetComponent<Rigidbody>();
		count = 0;
		SetCountText();
		winText.text = "";
	}

	void FixedUpdate () {
		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch(0);

            Vector2 touchScreenPosition = touch.position;

			Vector3 ballWorldPosition = rb.transform.position;
			Vector2 ballScreenPosition = Camera.main.WorldToScreenPoint(ballWorldPosition);

			Vector2 moveScreenVector = (touchScreenPosition - ballScreenPosition);
			Vector3 moveWorldVector = new Vector3(moveScreenVector.x, 0f, moveScreenVector.y);

			rb.AddForce (moveWorldVector * speed);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("Pick Up")) {
			other.gameObject.SetActive (false);
			count = count + 1;
			SetCountText();
		}
	}

    void sendScoreToMobileApp()
    {
#if UNITY_IOS || UNITY_TVOS
        NativeAPI.sendMessageToMobileApp(count.ToString());
#elif UNITY_ANDROID
        // Put Android-specific communication code here
#endif
    }

	void SetCountText() {
		countText.text = "Score: " + count.ToString();
		sendScoreToMobileApp(); // Send score to mobile app each time a cube is collected

		if (count >= 6)  {
			winText.text = "You Win!";
			countText.text = "";
			Application.Unload(); // Quit Mendi Unity game but not the mobile app
		}
	}

	void setBallColor(string color) {
        if (color == "red") GetComponent<Renderer>().material.color = Color.red;
        else if (color == "blue") GetComponent<Renderer>().material.color = Color.blue;
        else if (color == "green") GetComponent<Renderer>().material.color = Color.green;
        else GetComponent<Renderer>().material.color = Color.black;
	}
}