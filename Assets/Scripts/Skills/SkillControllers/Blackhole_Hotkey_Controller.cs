using TMPro;
using UnityEngine;

public class Blackhole_Hotkey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    private Transform enemy;
    private Blackhole_Skill_Controller blackhole;


    public void SetupHotKey(KeyCode _myHotKey,Transform _myEnemy,Blackhole_Skill_Controller _myBlackhole)
    {
        sr = GetComponent<SpriteRenderer>();

        myText = GetComponentInChildren<TextMeshProUGUI>();

        enemy = _myEnemy;
        blackhole = _myBlackhole;

        myHotKey = _myHotKey;
        myText.text = _myHotKey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyUp(myHotKey))
        {
            blackhole.AddEnemyToList(enemy);
            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }


}
