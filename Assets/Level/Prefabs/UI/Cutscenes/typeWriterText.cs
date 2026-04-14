using UnityEngine;
using System.Collections;
using TMPro;
public class typeWriterText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float speed = 0.0f;
    void Start(){
        StartCoroutine(ShowText());
    }
    public void StartTyping(string fullText){
        StopAllCoroutines();
        StartCoroutine(TypeText(fullText));
    }
    IEnumerator TypeText(string message){
        textComponent.text = "";
        foreach (char letter in message.ToCharArray()){
            textComponent.text += letter;
            yield return new WaitForSeconds(speed);
        }
    }
}
