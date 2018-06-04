using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gardener.Crawler.Api.Entity;

namespace Gardener.Crawler.Api.Interface
{
    public interface IApi
    {
        Task<List<Category>> GetCategoriesAsync();

        Task<List<Gallery>> GetGalleriesAsync(Category category, int page = 1);

        Task<Post> GetPost(Gallery gallery);
    }

    public enum ApiType
    {
        JDLingYu = 0,
        BCY = 1,
    }
}
