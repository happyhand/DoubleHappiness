using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UploadFiles.Api.Filters
{
    /// <summary>
    /// DisableFormValueModelBindingFilter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingFilter : Attribute, IResourceFilter
    {
        /// <summary>
        /// OnResourceExecuted
        /// </summary>
        /// <param name="context">context</param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        /// <summary>
        /// OnResourceExecuting
        /// </summary>
        /// <param name="context">context</param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            FormValueProviderFactory formValueProviderFactory = context.ValueProviderFactories
                                                                .OfType<FormValueProviderFactory>()
                                                                .FirstOrDefault();
            if (formValueProviderFactory != null)
            {
                context.ValueProviderFactories.Remove(formValueProviderFactory);
            }

            JQueryFormValueProviderFactory jqueryFormValueProviderFactory = context.ValueProviderFactories
                                                                            .OfType<JQueryFormValueProviderFactory>()
                                                                            .FirstOrDefault();
            if (jqueryFormValueProviderFactory != null)
            {
                context.ValueProviderFactories.Remove(jqueryFormValueProviderFactory);
            }
        }
    }
}