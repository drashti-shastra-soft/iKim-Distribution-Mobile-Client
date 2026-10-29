
using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using System.Text.Json;

namespace i_KimMobileClient.Utilities;

//public static class CategoryCacheService
//{
//    public static List<Category> Categories { get; set; } = new();
//    public static DateTime LastFetched { get; set; } = DateTime.MinValue;

//    // Adjust as needed — or remove expiry entirely if categories rarely change
//    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

//    public static bool IsLoaded => Categories != null && Categories.Count > 0;
//    public static bool IsExpired => DateTime.Now - LastFetched > CacheDuration;

//    public static void Clear()
//    {
//        Categories = new();
//        LastFetched = DateTime.MinValue;
//    }
//}

//public static class CategoryCacheService
//{
//    private static readonly string CacheFilePath =
//        Path.Combine(FileSystem.AppDataDirectory,"category_cache.json");

//    public static List<Category> Categories { get; private set; } = new();

//    public static bool IsLoaded =>
//        Categories.Count > 0;

//    // Load cache from device storage
//    public static async Task LoadAsync()
//    {
//        try
//        {
//            if (!File.Exists(CacheFilePath))
//                return;

//            string json = await File.ReadAllTextAsync(CacheFilePath);

//            Categories =
//                JsonSerializer.Deserialize<List<Category>>(json)
//                ?? new();


//            for (int i = 0; i < Categories.Count; i++)
//            {
//                Categories[i].Index = i;
//            }

//            PrepareImages(Categories);
//        }
//        catch
//        {
//            Categories = new();
//        }

//    }


//    // Save cache to device storage
//    public static async Task SaveAsync(List<Category> categories)
//    {
//        try
//        {
//            Categories = categories;
//            string json =
//                JsonSerializer.Serialize(categories);

//            await File.WriteAllTextAsync(
//                CacheFilePath,
//                json
//            );

//            PrepareImages(Categories);
//        }
//        catch
//        {
//        }
//    }

//    // Clear cache
//    public static void Clear()
//    {
//        Categories.Clear();

//        if (File.Exists(CacheFilePath))
//            File.Delete(CacheFilePath);
//    }

//    private static void PrepareImages(List<Category> categories)
//    {
//        foreach (var category in categories)
//        {
//            if (!string.IsNullOrWhiteSpace(category.Base64String))
//            {
//                try
//                {
//                    var bytes =
//                        Convert.FromBase64String(category.Base64String);

//                    category.ImageSourceCached =
//                        ImageSource.FromStream(() =>
//                            new MemoryStream(bytes));
//                }
//                catch
//                {
//                    category.ImageSourceCached = null;
//                }
//            }
//        }
//    }
//}

public static class CategoryCacheService
{
    private static readonly string CacheFilePath =
        Path.Combine(
            FileSystem.AppDataDirectory,
            "category_cache.json"
        );

    public static List<Category> Categories { get; private set; } = new();

    public static int? CachedUserId { get; private set; }

    public static bool IsLoaded =>
        Categories.Count > 0;


    // Load cache from device storage
    public static async Task LoadAsync()
    {
        try
        {
            if (!File.Exists(CacheFilePath))
                return;

            string json =
                await File.ReadAllTextAsync(CacheFilePath);

            var cache =
                JsonSerializer.Deserialize<CategoryCacheModel>(json);

            if (cache == null)
            {
                Clear();
                return;
            }

            CachedUserId = cache.UserId;

            Categories =
                cache.Categories ?? new();

            // Set index
            for (int i = 0; i < Categories.Count; i++)
            {
                Categories[i].Index = i;
            }

            PrepareImages(Categories);
        }
        catch
        {
            Categories = new();
            CachedUserId = null;
        }
    }


    // Save cache to device storage
    public static async Task SaveAsync(
        List<Category> categories,
        int userId)
    {
        try
        {
            Categories = categories;
            CachedUserId = userId;

            var cache = new CategoryCacheModel
            {
                UserId = userId,
                Categories = categories
            };

            string json =
                JsonSerializer.Serialize(cache);

            await File.WriteAllTextAsync(
                CacheFilePath,
                json
            );

            PrepareImages(Categories);
        }
        catch
        {
        }
    }


    // Check whether cache belongs to current user
    public static bool IsForUser(int userId)
    {
        return CachedUserId.HasValue &&
               CachedUserId.Value == userId;
    }


    // Clear cache
    public static void Clear()
    {
        Categories.Clear();

        CachedUserId = null;

        if (File.Exists(CacheFilePath))
        {
            File.Delete(CacheFilePath);
        }
    }


    private static void PrepareImages(
        List<Category> categories)
    {
        foreach (var category in categories)
        {
            if (!string.IsNullOrWhiteSpace(
                category.Base64String))
            {
                try
                {
                    var bytes =
                        Convert.FromBase64String(
                            category.Base64String
                        );

                    category.ImageSourceCached =
                        ImageSource.FromStream(
                            () => new MemoryStream(bytes)
                        );
                }
                catch
                {
                    category.ImageSourceCached = null;
                }
            }
        }
    }
}


public class CategoryCacheModel
{
    public int UserId { get; set; }

    public List<Category> Categories { get; set; } = new();
}