﻿using SinaMN75Api.Models.Enums;

namespace SinaMN75Api.Models
{
    public class Emoji
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public EmojiEnum EmojiEnum { get; set; }
        public Guid UserId { get; set; }
    }
}
