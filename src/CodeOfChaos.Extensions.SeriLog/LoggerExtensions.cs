// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using Serilog;
using Serilog.Core;
using System.Diagnostics.CodeAnalysis;

namespace CodeOfChaos.Extensions.SeriLog;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public static class LoggerExtensions {

    [MessageTemplateFormatMethod("messageTemplate")]
    [DoesNotReturn, AssertionMethod]
    public static void ThrowFatal(this ILogger logger, string messageTemplate, params object?[]? propertyValues) {
        var exception = (Exception)Activator.CreateInstance(typeof(Exception), messageTemplate)!;
        logger.Fatal(exception, messageTemplate, propertyValues);
        throw exception;
    }

    [MessageTemplateFormatMethod("messageTemplate")]
    [DoesNotReturn, AssertionMethod]
    public static void ThrowFatal<TException>(this ILogger logger, string messageTemplate, params object?[]? propertyValues) where TException : Exception, new() {
        var exception = (TException)Activator.CreateInstance(typeof(TException), messageTemplate)!;
        logger.Fatal(exception, messageTemplate, propertyValues);
        throw exception;
    }

    [MessageTemplateFormatMethod("messageTemplate")]
    [DoesNotReturn, AssertionMethod]
    public static void ThrowFatal<TException>(this ILogger logger, TException exception, string messageTemplate, params object?[]? propertyValues) where TException : Exception {
        logger.Fatal(exception, messageTemplate, propertyValues);
        throw exception;
    }

    [MessageTemplateFormatMethod("messageTemplate")]
    [DoesNotReturn, AssertionMethod]
    public static void ExitFatal(this ILogger logger, int exitCode, string messageTemplate, params object?[]? propertyValues) {
        logger.Fatal(messageTemplate, propertyValues);
        Environment.Exit(exitCode);
    }
}
