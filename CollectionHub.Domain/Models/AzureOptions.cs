﻿namespace CollectionHub.Models
{
    public class AzureOptions
    {
        public string? ResourceGroup { get; set; }

        public string? Account { get; set; }

        public string? Container { get; set; }

        public string? ConnectionString { get; set; }

        public string? BlobURL { get; set; }
    }
}