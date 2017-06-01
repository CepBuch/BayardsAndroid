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
        private string _token;
        const string uriGetDataTemplate = "{0}/api/getAll?apiKey={1}&lang={2}";
        const string uriImageUpload = "{0}/ui/images/{1}";
        const string uriCheckPassword = "{0}/api/checkPassword";
        const string uriGetUserAgreement = "{0}/api/getUserAgreement?apiKey={1}&lang={2}";
        const string uriGetUpdateDate = "{0}/api/getUpdateDate";



        //Constructor gets server address
        public ApiProvider(string host, string token)
        {
            _host = host;
            _token = token;
        }

        public async Task<bool> CheckUpdates(DateTime lastUpdate)
        {

            DateTime? newUpdateDate = default(DateTime);
            using (var client = new HttpClient())
            {
                string requestUri = string.Format(uriGetUpdateDate, _host);
                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeAnonymousType(responseString, new { date = default(DateTime?) });
                    newUpdateDate = result.date;
                    return (newUpdateDate.HasValue && newUpdateDate.Value > lastUpdate);
                }
                else
                    throw new WebException($"The server returned an error: {response.StatusCode}");
            }
        }
        public async Task<bool> CheckPassword(string password)
        {
            int? flag = 0;
            using (HttpClient hc = new HttpClient())
            {
                var values = new Dictionary<string, string> { { "password", password } };

                var content = new FormUrlEncodedContent(values);
                var response = await hc.PostAsync(string.Format(uriCheckPassword, _host), content);
                var responseString = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeAnonymousType(responseString, new { check = new int?() });
                flag = res.check;
            }
            return flag == 1;
        }

        public async Task<string> GetUserAgreement(string language)
        {
            using (var client = new HttpClient())
            {
                string requestUri = string.Format(uriGetUserAgreement, _host, _token, language);
                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeAnonymousType(responseString, new { content = string.Empty });
                    var agreement = result.content;

                    if (agreement != null && !agreement.StartsWith("Error"))
                    {
                        return agreement;
                    }
                    else
                        throw new UnauthorizedAccessException();
                }
                else
                    throw new WebException($"The server returned an error: {response.StatusCode}");
            }
        }


        //Returns categories from server for each language
        public async Task<Tuple<Model.Category[],Model.Location[],DateTime>> GetData(string[] languages)
        {
            var categories = new List<Model.Category>();
            var locations = new List<Model.Location>();
            var lastUpdateDate = default(DateTime);


            using (var client = new HttpClient())
            {
                foreach (var language in languages)
                {
                    string requestUri = string.Format(uriGetDataTemplate, _host, _token, language);
                    HttpResponseMessage response = await client.GetAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                    {
                        var resultForLanguageStr = await response.Content.ReadAsStringAsync();

                        if (resultForLanguageStr.StartsWith("Error"))
                            throw new UnauthorizedAccessException();

                        var resultForLanguage = JsonConvert.DeserializeObject<DTO.Response>(resultForLanguageStr);
                        if (resultForLanguage != null)
                        {
                            //Contents
                            var DTOCategories = resultForLanguage.Categories;

                            if (DTOCategories != null && DTOCategories.Length > 0)
                            {
                                categories.AddRange(await ConvertCatogories(DTOCategories, language));
                            }
                            else
                                throw new NullReferenceException("The answer is empty");

                            //Loactions
                            var DTOLocations = resultForLanguage.Locations;
                            if (DTOLocations != null)
                            {
                                locations.AddRange(ConvertLocations(DTOLocations, language).ToList());
                            }

                            //UpdateDate
                            if (lastUpdateDate == default(DateTime) && resultForLanguage.UpdateDate != null &&
                                resultForLanguage.UpdateDate != default(DateTime))
                                lastUpdateDate = resultForLanguage.UpdateDate;
                        }
                        else
                            throw new NullReferenceException("There is no response from the server");
                    }
                    else
                        throw new WebException($"The server returned an error: {response.StatusCode}");
                }
            }
            return new Tuple<Model.Category[], Model.Location[], DateTime>(categories.ToArray(), locations.ToArray(), lastUpdateDate);
        }


        private Model.Location[] ConvertLocations(DTO.Location[] locations, string language)
        {
            return locations
                .Where(l => l != null && !string.IsNullOrWhiteSpace(l.Id))
                .Select(l => new Model.Location
                {
                    Id = l.Id,
                    Name = l.Name,
                    Latitude = l.Latitude,
                    Longtitude = l.Longtitude,
                    Language = language,
                    Order = l.Order
                }).ToArray();
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
                    Language = language,
                    Order = c.Order
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
                    Order = sc.Order,
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
                    Order = r.Order,
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






    }
}
