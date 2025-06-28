using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalogs.Application.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Catalogs.Presentation.ModelBinders
{
    public class FormCollectionModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var features = new List<CreateFeatureDTO>();
            var form = bindingContext.HttpContext.Request.Form;
            
            // Get all feature entries from form
            var featureKeys = form.Keys.Where(k => k.StartsWith("Features["));
            var featureCount = featureKeys.Count(k => k.EndsWith("].Name"));

            for (int i = 0; i < featureCount; i++)
            {
                var nameKey = $"Features[{i}].Name";
                var valueKey = $"Features[{i}].Value";

                if (form.TryGetValue(nameKey, out var name) && form.TryGetValue(valueKey, out var value))
                {
                    features.Add(new CreateFeatureDTO 
                    { 
                        Name = name.ToString(), 
                        Value = value.ToString() 
                    });
                }
            }

            bindingContext.Result = ModelBindingResult.Success(features);
            return Task.CompletedTask;
        }
    }
} 