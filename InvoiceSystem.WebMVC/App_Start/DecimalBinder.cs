using System;
using System.Globalization;
using System.Web.Mvc;

public class DecimalModelBinder : IModelBinder
{
    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
        string modelName = bindingContext.ModelName;
        ValueProviderResult value = bindingContext.ValueProvider.GetValue(modelName);

        if (value == null)
            return null;

        string attempted = value.AttemptedValue;

        if (string.IsNullOrWhiteSpace(attempted))
            return null;

        attempted = attempted.Replace(",", "."); // Force dot as decimal separator

        decimal parsed;
        if (decimal.TryParse(attempted, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed))
            return parsed;

        bindingContext.ModelState.AddModelError(modelName, "Invalid decimal format");
        return null;
    }
}
