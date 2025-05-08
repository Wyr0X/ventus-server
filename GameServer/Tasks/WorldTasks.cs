using System.Net.WebSockets;
using Game.Server;
using Ventus.Network.Packets;
using VentusServer.Services;
using static LoggerUtil;

public class WorldTasks
{
    TaskScheduler _taskScheduler;
    GameServer _gameServer;

    public WorldTasks(
        TaskScheduler taskScheduler,
        GameServer gameServer
    )
    {
        _taskScheduler = taskScheduler;
        _gameServer = gameServer;

        taskScheduler.Subscribe(ClientPacket.PlayerInput, async (messagePair) => await HandlePlayerInput(messagePair));

    }
    /*************  ✨ Windsurf Command ⭐  *************/
    /// <summary>
    /// Handles an incoming player input message from a client.
    /// </summary>
    /// <param name="messagePair">The message pair containing the input message.</param>
    /// <returns>A completed task.</returns>
    /// <remarks>
    /// This function first checks if the player object associated with the account ID in the message pair exists.
    /// If it does not, an error is logged and the function exits.
    /// Otherwise, the input message is cast to a PlayerInput and checked for validity.

    /// Otherwise, the input is added to the player object's input queue using EnqueueInput.
    /// </remarks>

    private async Task HandlePlayerInput(UserMessagePair messagePair)
    {
        var accountId = messagePair.AccountId;
        PlayerObject playerObject = _gameServer.playersByAccountId[accountId];
        if (playerObject == null)
        {
            Log(LogTag.SessionTasks, $"PlayerObject not found for accountId: {accountId}", "HandlePlayerInput", isError: true);
            return;
        }
        var input = messagePair.ClientMessage as PlayerInput;
        if (input == null)
        {
            Log(LogTag.SessionTasks, $"Invalid input received for accountId: {messagePair.AccountId}", "HandlePlayerInput", isError: true);
            return;
        }
        if (input.Direction != Direction.None)
        {
            Log(LogTag.SessionTasks, $"Received input for accountId: {input} in direction: {input.Direction} and timestamp: {input.Timestamp} and sequence number: {input.SequenceNumber}", "HandlePlayerInput");

        }
        await playerObject.EnqueueInput(input);
    }



}
