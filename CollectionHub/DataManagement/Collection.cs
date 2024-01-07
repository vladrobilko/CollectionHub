﻿namespace CollectionHub.DataManagement
{
    public partial class Collection
    {
        public long Id { get; set; }

        public string UserId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public byte CategoryId { get; set; }

        public DateTime CreationDate { get; set; }

        public string? String1Name { get; set; }

        public string? String2Name { get; set; }

        public string? String3Name { get; set; }

        public string? Int1Name { get; set; }

        public string? Int2Name { get; set; }

        public string? Int3Name { get; set; }

        public string? Text1Name { get; set; }

        public string? Text2Name { get; set; }

        public string? Text3Name { get; set; }

        public string? Bool1Name { get; set; }

        public string? Bool2Name { get; set; }

        public string? Bool3Name { get; set; }

        public string? Date1Name { get; set; }

        public string? Date2Name { get; set; }

        public string? Date3Name { get; set; }

        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<Item> Items { get; set; } = new List<Item>();

        public virtual User User { get; set; } = null!;
    }
}
