using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MailSender : MonoBehaviour
{
    public void SendEmailExt()
    {
        string email = "contact@archipratique.com";
        string subject = "Appli Visite Virtuelle Interactive";
        string body = "Bonjour, je suis intéressé pour recevoir la newsletter d'Archipratique. Merci.";
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    public GameObject contactFormPrefab;
    private GameObject contactFormInstance;
    public string smtpServer;
    public int port;
    public string username, password, receiver;
    public void OpenContactForm()
    {
        contactFormInstance = Instantiate(contactFormPrefab);
        contactFormInstance.transform.SetParent(GameObject.Find("Credits").transform, false);
        contactFormInstance.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(()=> SendEmailInt());
        contactFormInstance.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => CloseContactForm());
    }
    public void CloseContactForm()
    {
        Destroy(contactFormInstance);
        UIManager.instance.ToggleCredits(false);
    }
    public void SendEmailInt()
    {
        string from = contactFormInstance.transform.GetChild(0).GetComponent<InputField>().text;
        string fromName = contactFormInstance.transform.GetChild(1).GetComponent<InputField>().text;
        string subject = contactFormInstance.transform.GetChild(2).GetComponent<InputField>().text;
        string message = contactFormInstance.transform.GetChild(3).GetComponent<InputField>().text;
        if (from != "")
        {
            smtpMail mail = new smtpMail(smtpServer, port, from, fromName, password, username, receiver, subject, message);
            CloseContactForm();
        }
    }
}
