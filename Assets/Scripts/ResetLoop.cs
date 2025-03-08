using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLoop : MonoBehaviour
{
    string currentCharacter = "Ch";

    [SerializeField] PnjDeplacement deplacementCh, deplacementCe, deplacementP, deplacementE;
    private Dictionary<string, PnjDeplacement> deplacements = new Dictionary<string, PnjDeplacement>();
    [SerializeField] CharacterController2D playerController;

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
        deplacements[currentCharacter].gameObject.SetActive(false);
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
        DisableUnwantedPnj();
    }



}
