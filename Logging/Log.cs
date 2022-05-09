using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BlazingRecept.Logging;

public static class Log
{
    private static string _logProperty = "Domain";

    public static void Verbose(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Verbose(messageTemplate);
    }

    public static void Verbose(
        Exception exception,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Verbose(exception, messageTemplate);
    }

    public static void Debug(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Debug(messageTemplate);
    }

    public static void Debug(
        Exception exception,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Debug(exception, messageTemplate);
    }

    public static void Information(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Information(messageTemplate);
    }

    public static void Information(
        Exception exception,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Information(exception, messageTemplate);
    }

    public static void Warning(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Warning(messageTemplate);
    }

    public static void Warning(
        Exception exception,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Warning(exception, messageTemplate);
    }

    public static void Error(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Error(messageTemplate);
    }

    public static void Error(
        Exception exception,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Error(exception, messageTemplate);
    }

    public static void Fatal(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Fatal(messageTemplate);
    }

    public static void Fatal(
        Exception exception,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        string messageTemplate = ConstructMessageTemplate(message, memberName, sourceFilePath, sourceLineNumber);

        Serilog.Log.ForContext(_logProperty, GetCallingClassName()).Fatal(exception, messageTemplate);
    }

    private static string GetCallingClassName()
    {
        string callingClassDescription = GetCallingClassDescription();
        return GetCallingClassNameFromDescription(callingClassDescription);
    }

    private static string GetCallingClassDescription()
    {
        string? fullName;
        Type? declaringType;
        int skipFrames = 3;

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
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase) ||
            declaringType.Module.Name.Equals("BlazingRecept.Contract.dll", StringComparison.OrdinalIgnoreCase));

        if (fullName == null)
        {
            return "Unrecognized";
        }

        return fullName;
    }

    private static string GetCallingClassNameFromDescription(string callingClassDescription)
    {
        int callingClassNameStartIndex = callingClassDescription.LastIndexOf(".");
        int callingClassNameEndIndex = callingClassDescription.IndexOf("`", callingClassNameStartIndex);

        if (callingClassNameEndIndex == -1)
        {
            callingClassNameEndIndex = callingClassDescription.IndexOf("+", callingClassNameStartIndex);
        }

        if (callingClassNameStartIndex == -1 || callingClassNameEndIndex == -1)
        {
            return "Unrecognized";
        }

        int startIndex = callingClassNameStartIndex + 1;
        int endIndex = callingClassNameEndIndex - callingClassNameStartIndex - 1;

        return callingClassDescription.Substring(startIndex, endIndex);
    }

    private static string ConstructMessageTemplate(string message, string memberName, string sourceFilePath, int sourceLineNumber)
    {
        return $"{message}\n" +
            $"\tMethod: {memberName}\n" +
            $"\tFile: {sourceFilePath}\n" +
            $"\tLine: {sourceLineNumber}\n";
    }
}