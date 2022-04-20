namespace BlazingRecept.Client.Utilities;

public static class InputValidation
{
    public static void ValidateStringToDouble(string variableValue, string variableName, string displayName, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(variableValue))
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} måste anges."
            });
        }
        else if (double.TryParse(variableValue, out double doubleValue) == false)
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} får endast innehålla siffror, med eller utan decimal."
            });
        }
        else if (doubleValue < 0.0)
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} kan ej vara ett negativt värde."
            });
        }
    }
}