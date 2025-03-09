using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLoop : MonoBehaviour
{
    string currentCharacter = "Ch";

    [SerializeField] PnjDeplacement deplacementCh, deplacementCe, deplacementP, deplacementE;
    private Dictionary<string, PnjDeplacement> deplacements = new Dictionary<string, PnjDeplacement>();
    [SerializeField] CharacterController2D playerController;
    [SerializeField] Material colorMaterial;

    private string nextChar()
    {
        switch (currentCharacter)
        {
            case "Ch": return "Ce";
            case "Ce": return "P";
            case "P": return "E";
            case "E": return "Ch";
        }

        return currentCharacter;
    }

    private void Awake()
    {
        deplacements.Add("Ch",deplacementCh);
        deplacements.Add("Ce",deplacementCe);
        deplacements.Add("P" ,deplacementP);
        deplacements.Add("E" ,deplacementE);

        GoToNextLoop();
    }

    private void SaveHistory()
    {
        deplacements[currentCharacter].history.Clear() ;
        foreach (inputHistory _history in playerController.history) deplacements[currentCharacter].history.Add(new inputHistory(_history.time, _history.value));
    }

    private void ResetPosition()
    {
        foreach (KeyValuePair<string, PnjDeplacement> kpv in deplacements) kpv.Value.ResetPosition();
        playerController.transform.position = deplacements[currentCharacter].initialPosition;
    }

    private void LaunchReplay()
    {
        foreach (KeyValuePair<string,PnjDeplacement> kpv in deplacements) kpv.Value.Launch();
    }

    private void DisableUnwantedPnj()
    {
        playerController.name = currentCharacter;
        deplacements[currentCharacter].gameObject.SetActive(false);
    }

    public void GoToNextLoop(Action action)
    {
        foreach (KeyValuePair<string,PnjDeplacement> kpv in deplacements) kpv.Value.gameObject.SetActive(true);

        GoToNextLoop();

        action.Invoke();

    }

    private void SetShaderColor()
    {
        switch (currentCharacter)
        {
            case "Ch": colorMaterial.SetColor("_Color", new Color(216 / 255f, 188 / 255f, 90 / 255f)); break; // jaune           #d8bc5a
            case "Ce": colorMaterial.SetColor("_Color", new Color(120 / 255f, 213 / 255f, 156 / 255f)); break; // vert           #78d59c
            case "P" : 
                if(false) colorMaterial.SetColor("_Color", new Color(66  / 255f, 212 / 255f, 228 / 255f));
                else     colorMaterial.SetColor("_Color", new Color(164 / 255f, 197 / 255f, 201 / 255f));
                break; // gris -> bleu   #a4c5c9 -> #42d4e4
            case "E" : colorMaterial.SetColor("_Color", new Color(232 / 255f, 161 / 255f, 229 / 255f)); break; // rose           #e8a1e5
        }
    }

    [ContextMenu("NextLoop")]
    public void GoToNextLoop()
    {
        foreach (KeyValuePair<string,PnjDeplacement> kpv in deplacements) kpv.Value.gameObject.SetActive(true);

        SaveHistory();
        playerController.ResetHistory();

        currentCharacter = nextChar();

        ResetPosition();
        LaunchReplay();
        SetShaderColor();
        DisableUnwantedPnj();
    }



}
