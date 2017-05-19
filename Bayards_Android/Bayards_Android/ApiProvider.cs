using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using Bayards_Android.Enums;

namespace Bayards_Android
{
    class ApiProvider
    {
        private string _host;
        const string uriGetDataTemplate = "{0}/api/getAll?lang={1}";
        const string uriImageUpload = "{0}/ui/images/{1}";
        const string uriCheckPassword = "{0}/api/checkPassword";



        //Constructor gets server address
        public ApiProvider(string host)
        {
            _host = host;
        }


        //Returns categories from server for each language
        public async Task<Model.Category[]> GetData(string[] languages)
        {
            if (languages == null || languages.Length == 0)
                throw new NullReferenceException("Languages array is empty");

            var result = new List<Model.Category>();

            using (var client = new HttpClient())
            {
                foreach (var language in languages)
                {
                    string requestUri = string.Format(uriGetDataTemplate, _host, language);
                    HttpResponseMessage response = await client.GetAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                    {
                        var resultForLanguageStr = await response.Content.ReadAsStringAsync();
                        var resultForLanguage = JsonConvert.DeserializeObject<DTO.Response>(resultForLanguageStr);

                        if (resultForLanguage != null)
                        {
                            var DTOCategories = resultForLanguage.Categories;

                            if (DTOCategories != null && DTOCategories.Length > 0)
                            {
                                result.AddRange(await ConvertCatogories(DTOCategories, language));
                            }
                            else
                                throw new NullReferenceException("The answer is empty");
                        }
                        else
                            throw new NullReferenceException("There is no response from the server");
                    }
                    else
                        throw new WebException($"The server returned an error: {response.StatusCode}");
                }
            }
            return result.ToArray();
        }


        private async Task<Model.Category[]> ConvertCatogories(DTO.Category[] catogoriesToConvert, string language)
        {
            var tasks = catogoriesToConvert
                .Where(c => c != null && !string.IsNullOrWhiteSpace(c.Id) && !string.IsNullOrWhiteSpace(c.Name))
                .Select(async c => new Model.Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Risks = await ConvertRisks(c.Risks, language),
                    Subcategories = await ConvertSubcategories(c.Subcategories, language),
                    Language = language
                }).ToArray();
            return await Task.WhenAll(tasks);
        }



        private async Task<Model.Category[]> ConvertSubcategories(DTO.Subcategory[] subcategoriesToConvert, string language)
        {
            var tasks = subcategoriesToConvert
                .Where(sc => sc != null && !string.IsNullOrWhiteSpace(sc.Id) && !string.IsNullOrWhiteSpace(sc.Name))
                .Select(async sc => new Model.Category
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    Risks = await ConvertRisks(sc.Risks, language),
                    Subcategories = null,
                    Language = language
                }).ToArray();
            return await Task.WhenAll(tasks);
        }


        private async Task<Model.Risk[]> ConvertRisks(DTO.Risk[] risksToConvert, string language)
        {
            var tasks = risksToConvert
                .Where(r => r != null && !string.IsNullOrWhiteSpace(r.Id) && !string.IsNullOrWhiteSpace(r.Name))
                .Select(async r => new Model.Risk
                {
                    Id = r.Id,
                    Name = r.Name,
                    Content = r.Content,
                    Language = language,
                    MediaObjects = await ConvertMedia(r.MediaObjects, language)
                }).ToList();

            return await Task.WhenAll(tasks);

        }

        private async Task<Model.MediaObject[]> ConvertMedia(DTO.MediaObject[] objectsToConvert, string language)
        {
            var resultObjects = new List<Model.MediaObject>();

            foreach (var DTOmediaObj in
                objectsToConvert.
                Where(o => o != null && !string.IsNullOrWhiteSpace(o.Uri) && !string.IsNullOrWhiteSpace(o.TypeMedia)))
            {
                TypeMedia mediaType = TypeMedia.Undefined;

                switch (DTOmediaObj.TypeMedia)
                {
                    case "image":
                        mediaType = TypeMedia.Image;
                        break;
                    case "video":
                        mediaType = TypeMedia.Video;
                        break;
                    default:
                        mediaType = TypeMedia.Undefined;
                        break;
                }

                resultObjects.Add(new Model.MediaObject
                {
                    Name = DTOmediaObj.Uri,
                    TypeMedia = mediaType,
                    Bytes = mediaType == TypeMedia.Image ? await DownloadImage(DTOmediaObj.Uri) : null
                });
            }
            return resultObjects
                .Where(o => o.TypeMedia != TypeMedia.Undefined).ToArray();
        }

        private async Task<byte[]> DownloadImage(string image_name)
        {
            string requestUri = string.Format(uriImageUpload, _host, image_name);

            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsByteArrayAsync();
                    }
                    else
                        throw new WebException($"The server returned an error: {response.StatusCode}");
                }
            }
            catch
            {
                return null;
            }
        }


        public async Task<bool> CheckPassword(string password)
        {
            try
            {
                int flag = 0;
                using (HttpClient hc = new HttpClient())
                {
                    var values = new Dictionary<string, string> { { "password", password } };

                    var content = new FormUrlEncodedContent(values);
                    var response = hc.PostAsync(string.Format(uriCheckPassword, _host), content).Result;
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeAnonymousType(responseString, new { check = 0 });
                    flag = res.check;
                }
                return flag == 1;
            }
            catch (Exception ex) { return false; };

        }



    }
}
