namespace BlazingRecept.Client.Utilities;

public static class InputValidation
{
    public static void ValidateNullableInt(int? variableValue, string variableName, string displayName, Dictionary<string, List<string>> errors)
    {
        if (variableValue == null)
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} måste anges."
            });
        }
        else if (variableValue.Value < 0)
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} kan ej vara ett negativt värde."
            });
        }
    }
    public static void ValidateNullableDouble(double? variableValue, string variableName, string displayName, Dictionary<string, List<string>> errors)
    {
        if (variableValue == null)
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} måste anges."
            });
        }
        else if (variableValue.Value < 0.0)
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} kan ej vara ett negativt värde."
            });
        }
    }
}