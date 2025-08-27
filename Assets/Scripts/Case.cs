using UnityEngine;

[System.Serializable]
public class Case
{
    private string crimeScene;
    private string timeOfDiscovery;
    private string numberOfSuspects;
    private string victimIntroduction;

    public void setCrimeScene(string str)
    {
        crimeScene = str;
    }
    public void setTimeOfDiscovery(string str)
    {
        timeOfDiscovery = str;
    }
    public void setNumberOfSuspects(string str)
    {
        numberOfSuspects = str;
    }
    public void setVictimIntroduction(string str)
    {
        victimIntroduction = str;
    }
    public string getCrimeScene()
    {
        return crimeScene;
    }
    public string getTimeOfDiscovery()
    {
        return timeOfDiscovery;
    }
    public string getNumberOfSuspects()
    {
        return numberOfSuspects;
    }
    public string getVictimIntroductionCrimeScene()
    {
        return victimIntroduction;
    }
    public string getCaseInformationToString()
    {
        return "CrimeScene: " + crimeScene
            + "\nTimeOfDiscovery: " + timeOfDiscovery
            + "\nNumberOfSuspects: " + numberOfSuspects
            + "\nVictimIntroduction: " + victimIntroduction;
    }
}
