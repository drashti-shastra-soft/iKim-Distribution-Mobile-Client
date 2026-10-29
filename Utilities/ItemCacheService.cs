using i_KimMobileClient.Models;
using System.Text.Json;

namespace i_KimMobileClient.Utilities;

//public static class ItemCacheService
//{
//    private static readonly string CacheFilePath =
//        Path.Combine(FileSystem.AppDataDirectory, "item_cache.json");


//    public static List<Item> Items { get; private set; } = new();


//    public static bool IsLoaded => Items.Count > 0;


//    public static async Task LoadAsync()
//    {
//        try
//        {
//            if (!File.Exists(CacheFilePath))
//                return;


//            string json =
//                await File.ReadAllTextAsync(CacheFilePath);


//            Items =
//                JsonSerializer.Deserialize<List<Item>>(json) ?? new();


//            PrepareImages(Items);
//        }
//        catch
//        {
//            Items = new();
//        }
//    }


//    public static async Task SaveAsync(List<Item> items)
//    {
//        try
//        {
//            Items = items;


//            string json =
//                JsonSerializer.Serialize(items);


//            await File.WriteAllTextAsync(
//                CacheFilePath,
//                json
//            );


//            PrepareImages(Items);
//        }
//        catch
//        {

//        }
//    }


//    public static void Clear()
//    {
//        Items.Clear();

//        if (File.Exists(CacheFilePath))
//        {
//            File.Delete(CacheFilePath);
//        }
//    }


//    private static void PrepareImages(List<Item> items)
//    {
//        foreach (var item in items)
//        {
//            if (!string.IsNullOrWhiteSpace(item.Base64String))
//            {
//                try
//                {
//                    var bytes =
//                        Convert.FromBase64String(item.Base64String);


//                    item.ImageSourceCached =
//                        ImageSource.FromStream(() =>
//                            new MemoryStream(bytes));
//                }
//                catch
//                {
//                    item.ImageSourceCached = null;
//                }
//            }
//        }
//    }
//}

public static class ItemCacheService
{
    private static readonly string CacheFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "item_cache.json");

    public static List<Item> Items { get; private set; } = new();

    public static int? CachedUserId { get; private set; }

    public static bool IsLoaded => Items.Count > 0;


    public static async Task LoadAsync()
    {
        try
        {
            if (!File.Exists(CacheFilePath))
                return;

            string json = await File.ReadAllTextAsync(CacheFilePath);

            var cache =
                JsonSerializer.Deserialize<ItemCacheModel>(json);

            if (cache == null)
            {
                Clear();
                return;
            }

            CachedUserId = cache.UserId;

            Items = cache.Items ?? new();

            PrepareImages(Items);
        }
        catch
        {
            Items = new();
            CachedUserId = null;
        }
    }


    public static async Task SaveAsync(
        List<Item> items,
        int userId)
    {
        try
        {
            Items = items;
            CachedUserId = userId;

            var cache = new ItemCacheModel
            {
                UserId = userId,
                Items = items
            };

            string json =
                JsonSerializer.Serialize(cache);

            await File.WriteAllTextAsync(
                CacheFilePath,
                json
            );

            PrepareImages(Items);
        }
        catch
        {
            // Log if required
        }
    }


    public static bool IsForUser(int userId)
    {
        return CachedUserId.HasValue &&
               CachedUserId.Value == userId;
    }


    public static void Clear()
    {
        Items.Clear();
        CachedUserId = null;

        if (File.Exists(CacheFilePath))
        {
            File.Delete(CacheFilePath);
        }
    }


    private static void PrepareImages(List<Item> items)
    {
        foreach (var item in items)
        {
            if (!string.IsNullOrWhiteSpace(item.Base64String))
            {
                try
                {
                    var bytes =
                        Convert.FromBase64String(item.Base64String);

                    item.ImageSourceCached =
                        ImageSource.FromStream(() =>
                            new MemoryStream(bytes));
                }
                catch
                {
                    item.ImageSourceCached = null;
                }
            }
        }
    }
}

public class ItemCacheModel
{
    public int UserId { get; set; }

    public List<Item> Items { get; set; } = new();
}