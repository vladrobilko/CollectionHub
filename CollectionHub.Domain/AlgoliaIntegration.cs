using Algolia.Search.Clients;
using Algolia.Search.Exceptions;
using Algolia.Search.Models.Search;
using CollectionHub.DataManagement;
using CollectionHub.Domain.Converters;
using CollectionHub.Domain.Interfaces;
using CollectionHub.Domain.Models;
using CollectionHub.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace CollectionHub.Domain
{
    public class AlgoliaIntegration : IAlgoliaIntegration
    {
        private readonly ISearchClient _searchClient;

        public AlgoliaIntegration(ISearchClient searchClient) => _searchClient = searchClient;

        public async Task<List<ItemViewModel>> SearchItems(string query)
        {
            if (query.IsNullOrEmpty())
            {
                return new List<ItemViewModel>();
            }

            try
            {
                var itemsAlgolia = await _searchClient.InitIndex("Item").SearchAsync<ItemAlgoliaModel>(new Query(query));

                return itemsAlgolia.Hits.Select(item => item.ToItemViewModel()).ToList();
            }
            catch (AlgoliaApiException)
            {
                return new List<ItemViewModel>();
            }
        }

        public async Task CreateItem(ItemDb item) => await _searchClient.InitIndex("Item").SaveObjectAsync(item.ToItemAlgoliaModel());

        public async Task UpdateItem(ItemDb item)
        {
            var existingItem = await _searchClient.InitIndex("Item").GetObjectAsync<ItemAlgoliaModel>(item.Id.ToString());

            await _searchClient.InitIndex("Item").PartialUpdateObjectAsync(existingItem.SetItemDb(item));
        }

        public async Task DeleteItem(long id) => await _searchClient.InitIndex("Item").DeleteObjectAsync(id.ToString());
    }
}
