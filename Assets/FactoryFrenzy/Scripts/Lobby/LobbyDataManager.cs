using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;

public class LobbyDataManager : MonoBehaviour
{
    private Dictionary<string, Lobby> lobbies = new Dictionary<string, Lobby>();

    public async Task<Lobby> CreateLobbyAsync(string name, string type)
    {
        if (lobbies.ContainsKey(name))
        {
            throw new Exception("A lobby with this name already exists.");
        }

        var options = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
        {
            { "Type", new DataObject(DataObject.VisibilityOptions.Public, type) }
        }
        };

        if (type == "Private")
        {
            options.IsPrivate = true;
        }

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(name, 10, options);
        lobbies.Add(name, lobby);
        return lobby;
    }


    public Lobby GetLobbyByCode(string code)
    {
        return lobbies.Values.FirstOrDefault(lobby => lobby.LobbyCode == code);
    }

    public IEnumerable<Lobby> GetPublicLobbies()
    {
        return lobbies.Values.Where(lobby =>
            lobby.Data.ContainsKey("Type") && lobby.Data["Type"].Value == "Public");
    }


    private string GenerateLobbyCode()
    {
        return UnityEngine.Random.Range(1000, 9999).ToString();
    }
}
