using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class smtpMail
{
    public smtpMail(string smtpServer, int port, string from, string fromName, string password, string username,
        string receiver, string subject, string message)
    {
        SmtpClient _smtpServer = new SmtpClient(smtpServer, port);
        MailMessage _message = new MailMessage(new MailAddress(from, fromName), new MailAddress(receiver));
        _message.Bcc.Add("guillaumeschott@archipratique.com");
        _message.Subject = "Appli Visite Virtuelle : Demande de contact" ;
        _message.SubjectEncoding = System.Text.Encoding.UTF8;
        string header = "Message envoyé depuis le formulaire de contact de l'application de Visite Virtuelle Interactive.\nDe: " + fromName + " (" + from + ")\nLe : " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " à " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "\nSujet : " + subject + "\nMessage : ";
        string footer = "\n\n\n\n\n\nDiagnostic :\nDevice : " + SystemInfo.deviceType + " | " + SystemInfo.deviceName + " | " + SystemInfo.deviceModel + "\nOS : " +  SystemInfo.operatingSystem + "\nCPU : " + SystemInfo.processorCount + " cores | " + SystemInfo.processorType + " (" + SystemInfo.processorFrequency + " MHz)\nGPU : " + SystemInfo.graphicsDeviceVendor + " | " + SystemInfo.graphicsDeviceName + " | " + SystemInfo.graphicsDeviceVersion + " | VRAM : " + SystemInfo.graphicsMemorySize + " Mo " + "\nRAM : " + SystemInfo.systemMemorySize + " Mo\nApp : " + Application.productName + " v." + Application.version;
        _message.Body = header + message + footer;
        _message.BodyEncoding = System.Text.Encoding.UTF8;

        _smtpServer.Credentials = new System.Net.NetworkCredential(username, password) as ICredentialsByHost;
        _smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };

        _smtpServer.SendAsync(_message, "Envoi email");
        _message.Dispose();
    }
}