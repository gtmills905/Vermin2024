
using UnityEngine;
using System.Collections.Generic;


public class PlayerRoleManager : MonoBehaviour
{
    public enum Role { Farmer, Bird }

    public Role currentRole;
    public bool isRoleAvailable; // Check if the role is available

    // Role limits
    private int maxFarmers = 1;
    private int maxBirds = 4;

    // Add a reference to the list of players (you can get this from a manager)
    public List<PlayerRoleManager> players; // Assuming you have a list of PlayerRoleManagers

    private void Start()
    {
        AssignRole();
    }

    private void AssignRole()
    {
        int currentFarmers = GetRoleCount(Role.Farmer);
        int currentBirds = GetRoleCount(Role.Bird);

        // Assign the role dynamically based on available spots
        if (currentFarmers < maxFarmers)
        {
            currentRole = Role.Farmer;
        }
        else if (currentBirds < maxBirds)
        {
            currentRole = Role.Bird;
        }
        else
        {
            // Handle case where no roles are available
            Debug.LogWarning("No roles available!");
        }
    }

    private int GetRoleCount(Role role)
    {
        int count = 0;

        // Check the current players' roles to count how many have the specified role
        foreach (var player in players)
        {
            if (player.currentRole == role)
                count++;
        }
        return count;
    }

    public void SwitchRole()
    {
        if (currentRole == Role.Farmer && GetRoleCount(Role.Bird) < maxBirds)
        {
            currentRole = Role.Bird;
        }
        else if (currentRole == Role.Bird && GetRoleCount(Role.Farmer) < maxFarmers)
        {
            currentRole = Role.Farmer;
        }
    }
}
