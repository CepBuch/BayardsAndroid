using System;
using System.Collections.Generic;
using System.Linq;
using Bayards_Android.Model;
using System.Net.Http;
using Bayards_Android.DTO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;

namespace Bayards_Android
{
    class ApiProvider
    {
        const string hosting = "http://vhost29450.cpsite.ru";
        const string UriSectionsListTemplate = "{0}/api/allSections?lang={1}";
        const string UriSectionContent = "{0}/api/section?sectionid={1}&&lang={2}";
        const string UriRiskContent = "{0}/api/risk?riskid={1}&lang={2}";

        //Returns categories from server (now they are just generated)
        public async Task<List<Model.Category>> GetCategories(string language)
        {
            string requestUri = string.Format(UriSectionsListTemplate, hosting, language);


            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var resultStr = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Response>(resultStr);

                    if (result != null)
                    {
                        var categories = result.Categories;
                        if (categories != null && categories.Count > 0)
                        {
                            return categories
                                 .Where(c => c.Id != null && c.Name != null)
                                 .OrderBy(c => c.Name)
                                 .Select(c => new Model.Category { Name = c.Name, ServerId = c.Id })
                                 .ToList();
                        }
                        else
                            throw new NullReferenceException("��� ����� � �����������");
                    }
                    else
                        throw new NullReferenceException("������ �� ������ ������");
                }
                else
                    throw new WebException($"������ ������ ������: {response.StatusCode}");

            }
        }


        public List<Model.Risk> GetRisks(bool withImages)
        {
            var risks = new List<Model.Risk>();
            for (int i = 1; i <= 3; i++)
            {
                Model.Risk riskToAdd = new Model.Risk()
                {
                    Name = $"Risk {i}",
                    Content_id = Resource.String.lorem
                };
                if (withImages)
                {
                    riskToAdd.Image_id = i == 1 ? Resource.Drawable.lorem1 : i == 2 ? Resource.Drawable.lorem2 : Resource.Drawable.lorem3;
                }

                risks.Add(riskToAdd);
            }
            return risks;
        }
    }
}
