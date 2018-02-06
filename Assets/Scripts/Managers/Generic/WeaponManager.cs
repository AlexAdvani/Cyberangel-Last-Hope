using System.Collections.Generic;

public class WeaponManager : SingletonBehaviour<WeaponManager>
{
    // Weapon List
    public WeaponData[] aWeaponList;

    // Player Loadout
    List<WeaponData> lPlayerLoadout;
    // Default Player Loadout
    public List<WeaponData> lDefaultLoadout;

    #region Public Properties

    // Player Loadout
    public List<WeaponData> PlayerLoadout
    {
        get { return lPlayerLoadout; }
    }

    #endregion

    // Initialization
    public override void Awake()
    {
        base.Awake();

        lPlayerLoadout = new List<WeaponData>();
        LoadPlayerLoadout();
    }

    // Get Weapon Data by ID
    public WeaponData GetWeaponData(int id)
    {
        if (id < 0 || id >= aWeaponList.Length)
        {
            return null;
        }

        return aWeaponList[id];
    }

    // Set a new Player Loadout
    public void SetLoadout(List<WeaponData> loadout)
    {
        if (loadout == null || loadout.Count == 0)
        {
            return;
        }

        if (loadout.Count >= GameManager.Player.iMaxWeaponCount)
        {
            loadout.RemoveRange(GameManager.Player.iMaxWeaponCount, loadout.Count - GameManager.Player.iMaxWeaponCount);
        }

        lPlayerLoadout = loadout;
    }

    // Sets an individual weapon to the loadout
    public void SetLoadoutWeapon(WeaponData weapon, int index, bool clearList = false)
    {
        if (index < 0 || index >= GameManager.Player.iMaxWeaponCount)
        {
            return;
        }

        if (clearList)
        {
            lPlayerLoadout.Clear();
            lPlayerLoadout.Add(weapon);
        }
        else
        {
            if (lPlayerLoadout.Count <= index)
            {
                int difference = index - lPlayerLoadout.Count + 1;

                for (int i = 0; i < difference; i++)
                {
                    lPlayerLoadout.Add(null);
                }
            }

            lPlayerLoadout[index] = weapon;
        }
    }

    // Sets the Player Loadout to the Default Loadout
    public void SetToDefaultLoadout()
    {
        lPlayerLoadout = lDefaultLoadout;
    }

    // Load Player Loadout
    public void LoadPlayerLoadout()
    {
        int[] loadoutIDs = ProfileDataManager.Instance.LoadWeaponLoadout();

        if (loadoutIDs.Length == 0)
        {
            lPlayerLoadout = lDefaultLoadout;
            return;
        }

        lPlayerLoadout.Clear();

        for (int i = 0; i < loadoutIDs.Length; i++)
        {
            lPlayerLoadout.Add(aWeaponList[loadoutIDs[i]]);
        }
    }

    // Save Player Loadout
    public void SavePlayerLoadout()
    {
        int[] loadoutIDs = new int[lPlayerLoadout.Count];

        for (int i = 0; i < lPlayerLoadout.Count; i++)
        {
            for (int j = 0; j < aWeaponList.Length; j++)
            {
                if (aWeaponList[j] != lPlayerLoadout[i])
                {
                    continue;
                }

                loadoutIDs[i] = j;
                break;
            }
        }

        ProfileDataManager.Instance.SaveWeaponLoadout(loadoutIDs);
    }
}
