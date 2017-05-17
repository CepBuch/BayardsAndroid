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
        const string hosting = "http://vhost29450.cpsite.ru";
        const string uriGetDataTemplate = "{0}/api/getAll?lang={1}";

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
                    string requestUri = string.Format(uriGetDataTemplate, hosting, language);
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
                                result.AddRange(ConvertCatogories(DTOCategories, language));
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


        private Model.Category[] ConvertCatogories(DTO.Category[] catogoriesToConvert, string language)
        {
            return catogoriesToConvert
                .Where(c => c != null && !string.IsNullOrWhiteSpace(c.Id) && !string.IsNullOrWhiteSpace(c.Name))
                .Select(c => new Model.Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Risks = ConvertRisks(c.Risks, language),
                    Subcategories = ConvertSubcategories(c.Subcategories, language),
                    Language = language
                }).ToArray();
        }



        private Model.Category[] ConvertSubcategories(DTO.Subcategory[] subcategoriesToConvert, string language)
        {
            return subcategoriesToConvert
                .Where(sc => sc != null && !string.IsNullOrWhiteSpace(sc.Id) && !string.IsNullOrWhiteSpace(sc.Name))
                .Select(sc => new Model.Category
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    Risks = ConvertRisks(sc.Risks, language),
                    Subcategories = null,
                    Language = language
                }).ToArray();
        }


        private Model.Risk[] ConvertRisks(DTO.Risk[] risksToConvert, string language)
        {
            return risksToConvert
                .Where(r => r != null && !string.IsNullOrWhiteSpace(r.Id) && !string.IsNullOrWhiteSpace(r.Name))
                .Select(r => new Model.Risk
                {

                    Id = r.Id,
                    Name = r.Name,
                    Content = r.Content,
                    Language = language,
                    MediaObjects = ConvertMedia(r.MediaObjects, language)

                }).ToArray();
        }

        private Model.MediaObject[] ConvertMedia(DTO.MediaObject[] objectsToConvert, string language)
        {
            var resultObjects = new List<Model.MediaObject>();

            foreach (var DTOmediaObj in
                objectsToConvert.
                Where(o => o != null && !string.IsNullOrWhiteSpace(o.Link) && !string.IsNullOrWhiteSpace(o.TypeMedia)))
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
                    Link = DTOmediaObj.Link,
                    TypeMedia = mediaType
                });
            }
            return resultObjects
                .Where(o => o.TypeMedia != TypeMedia.Undefined).ToArray();
        }
    }
}
