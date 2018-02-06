using UnityEngine;

public class ProfileDataManager : SingletonBehaviour<ProfileDataManager>
{
    // Profile File Name
    public string sFileName;
    // Current File Path to Profile Data
    string sCurrentFilePath = "";

    // Number of Profiles in the Save Data
    public int iNumberOfProfiles = 3;
    // Current Profile ID
    int iCurrentProfileID = 0;

    #region Public Properties

    // Current Profile ID
    public int CurrentProfileID
    {
        get { return iCurrentProfileID; }
    }

    #endregion

    // Use this for initialization
    public override void Awake()
    {
        base.Awake();

        InitializeFile();
    }

    // Initialize Save File with blank profile data
    private void InitializeFile()
    {
        sCurrentFilePath = Application.persistentDataPath + "/" + sFileName;

        if (!ES2.Exists(sCurrentFilePath))
        {
            ES2Writer writer = ES2Writer.Create(sCurrentFilePath);
            writer.Save();

            // Create file tags early for storage per profile

            writer.Dispose();
        }
    }

    // Sets the Current Profile to save to
    public void SetCurrentProfile(int profileID)
    {
        if (profileID < 0 || profileID >= iNumberOfProfiles)
        {
            return;
        }

        iCurrentProfileID = profileID;
    }

    #region Loading

    // Load Main Level Unlocked Progression
    public void LoadGameProgression()
    {
    }

    // Load Current Checkpoint 
    public void LoadCheckpoint()
    {
    }

    // Load Unlocked Weapon List
    public void LoadWeaponProgression()
    { 
    }

    // Load Current Weapon Loadout
    public int[] LoadWeaponLoadout()
    {
        if (sCurrentFilePath == "" || sCurrentFilePath == null)
        {
            InitializeFile();
        }

        if (ES2.Exists(sCurrentFilePath + "?tag=" + "Profile" + iCurrentProfileID + " Loadout"))
        {
            return ES2.LoadArray<int>(sCurrentFilePath + "?tag=" + "Profile" + iCurrentProfileID + " Loadout");
        }
        else
        {
            return new int[0];
        }
    }

    // Load VR Mission Unlocked Progression
    public void LoadVRMissionProgression()
    {

    }

    // Load VR Mission Time
    public float LoadVRMissionTime(string missionID)
    {
        if (sCurrentFilePath == "" || sCurrentFilePath == null)
        {
            InitializeFile();
        }

        if (ES2.Exists(sCurrentFilePath + "?tag=" + "Profile" + iCurrentProfileID + " " + missionID + " Time"))
        {
            return ES2.Load<float>(sCurrentFilePath + "?tag=" + "Profile" + iCurrentProfileID + " " + missionID + " Time");
        }
        else
        {
            return -1;
        }
    }

    #endregion

    #region Saving

    // Save Main Level Unlocked Progression
    public void SaveGameProgression()
    {

    }

    // Save Current Checkpoint
    public void SaveCheckpoint()
    {

    }

    // Save Weapon Unlocked Progression
    public void SaveWeaponProgression()
    {

    }

    // Save Current Weapon Loadout
    public void SaveWeaponLoadout(int[] loadoutIDs)
    {
        ES2.Save(loadoutIDs, sCurrentFilePath + "?tag=" + "Profile" + iCurrentProfileID + " Loadout");
        SaveNotificationUI.OpenSaveNotification();
    }

    // Save VR Mission Unlocked Progression
    public void SaveVRMissionProgression()
    {

    }

    // Save VR Mission Time
    public void SaveVRMissionTime(string missionID, float time)
    {
        ES2.Save(time, sCurrentFilePath + "?tag=" + "Profile" + iCurrentProfileID + " " + missionID + " Time");
        SaveNotificationUI.OpenSaveNotification();
    }

    #endregion
}
