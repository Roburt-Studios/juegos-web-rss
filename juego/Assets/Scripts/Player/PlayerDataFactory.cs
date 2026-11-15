using System.Collections.Generic;
using UnityEngine;

public static class PlayerDataFactory
{
    private static readonly Dictionary<Profile, PlayerData> cache = new();

    public static PlayerData GetData(Profile characterClass)
    {
        var key = characterClass;

        if (cache.ContainsKey(key)) return cache[key];

        PlayerData data = CreateProfile(characterClass);
        cache.Add(key, data);

        return data;
    }

    private static PlayerData CreateProfile(Profile characterClass)
    {
        switch (characterClass)
        {
            case (Profile.Strength):
                return new PlayerData(
                    Profile.Strength,
                    initialHealth: 100,
                    strength: 120,
                    charisma: 35,
                    defense: 65,
                    skillPower: 50);
            case (Profile.Defense):
                return new PlayerData(
                    Profile.Defense,
                    initialHealth: 150,
                    strength: 120,
                    charisma: 60,
                    defense: 80,
                    skillPower: 50);
            case (Profile.Intelligence):
                return new PlayerData(
                    Profile.Intelligence,
                    initialHealth: 80,
                    strength: 50,
                    charisma: 75,
                    defense: 30,
                    skillPower: 110);
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}
