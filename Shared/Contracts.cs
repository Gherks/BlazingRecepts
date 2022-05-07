using Serilog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BlazingRecept.Shared;

public static class Contracts
{
    private static string _logProperty = "Domain";

    public static void LogAndThrowWhenNull([NotNull] object? value, string errorMessage)
    {
        if (value is null)
        {
            string callingClassDescription = GetCallingClassDescription();
            string callingClassName = GetCallingClassName(callingClassDescription);

            Log.ForContext(_logProperty, callingClassName).Error(errorMessage);
            throw new ArgumentNullException(nameof(value), errorMessage);
        }
    }

    private static string GetCallingClassDescription()
    {
        string? fullName;
        Type? declaringType;
        int skipFrames = 2;

        do
        {
            MethodBase? method = new StackFrame(skipFrames, false).GetMethod();

            if (method == null)
            {
                return "Unrecognized";
            }

            declaringType = method.DeclaringType;

            if (declaringType == null)
            {
                return method.Name;
            }

            skipFrames++;
            fullName = declaringType.FullName;
        }
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

        if (fullName == null)
        {
            return "Unrecognized";
        }

        return fullName;
    }

    private static string GetCallingClassName(string callingClassDescription)
    {
        int callingClassNameStartIndex = callingClassDescription.LastIndexOf(".");
        int callingClassNameEndIndex = callingClassDescription.IndexOf("+", callingClassNameStartIndex);

        if (callingClassNameStartIndex == -1 || callingClassNameEndIndex == -1)
        {
            return "Unrecognized";
        }

        int startIndex = callingClassNameStartIndex + 1;
        int endIndex = callingClassNameEndIndex - callingClassNameStartIndex - 1;

        return callingClassDescription.Substring(startIndex, endIndex);
    }
}