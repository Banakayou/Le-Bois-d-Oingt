using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour {
    private int current_child = 0;
    private GameObject fadescreen;
	// Use this for initialization
	void Start () {
        fadescreen = GameObject.Find("FadeScreen");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void next()
    {
        if (current_child == 0)
        {
            fadescreen.transform.GetChild(0).GetComponent<Text>().text = "La boussole vous permet de regarder où se situe le Nord";
        }
        if (current_child == 1)
        {
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(false);
            fadescreen.transform.GetChild(0).GetComponent<Text>().text = "Gardez un oeil sur l'époque à laquelle vous vous trouvez\n et alternez entre le 13ème siècle et l'époque contemporaine grâce au bouton de voyage temporel";
            current_child += 1;
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(true);
        }
        if (current_child == 3)
        {
            fadescreen.transform.GetChild(current_child-1).gameObject.SetActive(false);
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(false);
            fadescreen.transform.GetChild(0).GetComponent<Text>().text = "Naviguez à n'importe quelle heure de la journée en déplaçant l'horloge sur la frise";
        }
        if (current_child == 4)
        {
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(false);
            fadescreen.transform.GetChild(0).GetComponent<Text>().text = "Effectuez des captures d'écran facilement en un clic";
        }
        if (current_child == 5)
        {
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(false);
            fadescreen.transform.GetChild(0).GetComponent<Text>().text = "Ce bouton sert à nous contacter, ou à relancer ce tutoriel d'initiation";
        }
        if (current_child == 6)
        {
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(false);
            fadescreen.transform.GetChild(0).GetComponent<Text>().text = "Pour visiter le Bois d'Oingt sans effort,\n Le mode visite guidée automatique vous montrera tous les endroits intéressants du village";
        }
        if (current_child == 7)
        {
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(false);
            fadescreen.transform.GetChild(0).GetComponent<Text>().text = "Ces deux derniers boutons servent à piloter manuellement la caméra\n Vous aurez besoin de votre souris et des touches fléchées";
            current_child += 1;
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(true);
        }
        if (current_child == 9)
        {
            fadescreen.transform.GetChild(current_child - 1).gameObject.SetActive(false);
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(false);
            fadescreen.transform.GetChild(0).GetComponent<Text>().text = "Merci de votre attention, et bonne visite !";
            current_child += 1;
            fadescreen.transform.GetChild(current_child).gameObject.SetActive(false);
            Invoke("endTuto", 2);
            return;
        }            
        current_child += 1;
        fadescreen.transform.GetChild(current_child).gameObject.SetActive(true);    
    }
    private void endTuto()
    {
        DataManager.instance.setMode(DataManager.instance.MODE_COURANT);
    }
    public void resetTuto()
    {
        UIManager.instance.ToggleCredits(false);
        DataManager.instance.transform.parent.GetComponentInChildren<FadeCamera>().FadeToBlack();
        current_child = 0;
        fadescreen.transform.GetChild(0).gameObject.SetActive(true);
        fadescreen.transform.GetChild(0).GetComponent<Text>().text = "Bienvenue dans la visite interactive du bois d'Oingt.\n\n Voici une courte introduction à l'utilisation de cet outil.";
        fadescreen.transform.GetChild(10).gameObject.SetActive(true);
    }
}
