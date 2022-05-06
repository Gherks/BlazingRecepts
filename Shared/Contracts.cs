using Serilog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BlazingRecept.Shared;

public static class Contracts
{
    private const string _logProperty = "Domain";

    public static void LogAndThrowWhenNull([NotNull] object? value, string errorMessage)
    {
        if (value is null)
        {
            Log.ForContext(_logProperty, NameOfCallingClass()).Error(errorMessage);
            throw new ArgumentNullException(nameof(value), errorMessage);
        }
    }

    private static string NameOfCallingClass()
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
}