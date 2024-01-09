﻿using CollectionHub.Models.Enums;
using CollectionHub.Models.ViewModels;

namespace CollectionHub.Services.Interfaces
{
    public interface ICollectionService
    {
        Task<List<string>> GetAllCategories();

        Task CreateCollection(CollectionViewModel collection, string userMail);

        Task<List<CollectionViewModel>> GetUserCollections(string userName);

        Task<CollectionViewModel> GetUserCollection(string userName, long id);

        Task<bool> AddCollectionItemField(string userName, long id, DataType type, string name);

        Task DeleteCollection(long id);
    }
}
