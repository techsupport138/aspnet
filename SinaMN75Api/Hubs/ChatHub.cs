﻿using Microsoft.AspNetCore.SignalR;
using SinaMN75Api.Core;

namespace SinaMN75Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        public ChatHub(AppDbContext db)
        {
            _context = db;
        }

        #region One to One Messaging
        public async Task SendMessageToReceiver(string sender, string receiver, string message)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToLower() == receiver.ToLower());

            if (user != null)
            {
                //Todo: change IsloggedIn to IsOnline
                if (user.IsLoggedIn)
                    await Clients.User(user.Id).SendAsync("MessageReceived", sender, message);
                //Todo: else => push notification for receiver

            }
        }

        public async Task SendEditedMessageToReceiver(string sender, string receiver, string message, Guid messageId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToLower() == receiver.ToLower());

            if (user != null)
            {
                //Todo: change IsloggedIn to IsOnline
                if (user.IsLoggedIn)
                    await Clients.User(user.Id).SendAsync("MessageEdited", sender, message, messageId);
                //Todo: else => push notification for receiver
            }
        }

        public async Task SendDeletedMessageToReceiver(string sender, string receiver, Guid messageId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToLower() == receiver.ToLower());

            if (user != null)
            {
                //Todo: change IsloggedIn to IsOnline
                if (user.IsLoggedIn)
                    await Clients.User(user.Id).SendAsync("MessageDeleted", sender, messageId);
                //Todo: else => push notification for receiver
            }
        } 
        #endregion

        public async Task SendMessageToGroup(string roomId, List<string> receiverList, string message)
        {
            await Clients.Users(receiverList).SendAsync("MessageReceivedFromGroup", roomId, message);
        }
        public async Task SendEditedMessageToGroup(string roomId, List<string> receiverList, string message, string messageId)
        {
            await Clients.Users(receiverList).SendAsync("MessageEditedFromGroup", roomId, message, messageId);
        }
        public async Task SendDeletedMessageToGroup(string roomId, List<string> receiverList, string message, string messageId)
        {
            await Clients.Users(receiverList).SendAsync("MessageDeletedFromGroup", roomId, message, messageId);
        }

        public async Task NotifySeenMessage(string senderId, string receiverId, Guid messageId)
        {
            var userId = _context.Users.FirstOrDefault(u => u.Id.ToLower() == senderId.ToLower()).Id;

            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.User(userId).SendAsync("MessageSeen", senderId, receiverId, messageId);

            }
        }

    }
}
