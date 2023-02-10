﻿using SinaMN75Api.Core;
using SinaMN75Api.Models;
using SinaMN75Api.Models.Enums;

namespace SinaMN75Api.Repository
{
    public interface IMessageRepository
    {
        Task AddEmojiToMessage(EmojiEnum emoji, Guid messageId, Guid userId);
        Task<List<ChatMessage>> GetPrivateMessages(string firstUser, string secondUser);
        Task EditPrivateMessages(string messageText, Guid messageId);
        Task DeletePrivateMessage(Guid messageId);
    }

    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;
        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddEmojiToMessage(EmojiEnum emoji, Guid messageId, Guid userId)
        {
            var message = await _context.Set<ChatMessage>().FirstOrDefaultAsync(x => x.Id == messageId);

            if (message != null)
            {
                message.Emojies.Add(new Emoji { EmojiEnum = emoji, UserId = userId });
                await _context.SaveChangesAsync();
            }

        }

        public async Task DeletePrivateMessage(Guid messageId)
        {
            var message = await _context.Set<ChatMessage>().FirstOrDefaultAsync(x => x.Id == messageId);
            _context.Remove<ChatMessage>(message);
            await _context.SaveChangesAsync();

        }

        public async Task EditPrivateMessages(string messageText, Guid messageId)
        {
            var message = await _context.Set<ChatMessage>().FirstOrDefaultAsync(x => x.Id == messageId);
            if(message != null)
            {
                message.MessageText = messageText;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ChatMessage>> GetPrivateMessages(string firstUser, string secondUser)
        {
            var firstMessageBatch = await _context.Set<ChatMessage>().Where(x => x.FromUserId == firstUser && x.ToUserId == secondUser).ToListAsync();
            var secondMessageBatch = await _context.Set<ChatMessage>().Where(x => x.FromUserId == secondUser && x.ToUserId == firstUser).ToListAsync();

            firstMessageBatch.AddRange(secondMessageBatch);
            var output = firstMessageBatch.OrderBy(x => x.CreatedAt).ToList();

            return output;

        }
    }
}