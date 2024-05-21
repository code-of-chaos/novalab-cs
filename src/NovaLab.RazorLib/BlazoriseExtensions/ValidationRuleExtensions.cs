// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Blazorise;

namespace NovaLab.RazorLib.BlazoriseExtensions;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public static class ValidationRuleExtensions {
    
    public static void IsInt(ValidatorEventArgs e ) {
        e.Status =  int.TryParse(e.Value as string, out _) ? ValidationStatus.Success : ValidationStatus.Error;
    }
    
    public static void IsIntPositive(ValidatorEventArgs e ) {
        e.Status = e.Value is >= 0
            ? ValidationStatus.Success
            : ValidationStatus.Error;
    }
    
    public static void IsIntNegative(ValidatorEventArgs e ) {
        e.Status = e.Value is <= 0
            ? ValidationStatus.Success
            : ValidationStatus.Error;
    }
    
    public static void IsIntPositiveNonZero(ValidatorEventArgs e ) {
        e.Status = e.Value is > 0
            ? ValidationStatus.Success
            : ValidationStatus.Error;
    }
    
    public static void IsIntNegativeNonZero(ValidatorEventArgs e ) {
        e.Status = e.Value is < 0
            ? ValidationStatus.Success
            : ValidationStatus.Error;
    }
}