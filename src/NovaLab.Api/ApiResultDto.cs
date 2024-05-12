// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

namespace NovaLab.Api;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record ApiResultDto<T>(
    bool Success, 
    string? Message, 
    T[] Data) {

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static ApiResultDto<T> Empty() {
        return new ApiResultDto<T>(false, "No Data Could be retrieved", []);
    }

    public static ApiResultDto<T> Successful(params T[] objects) => Successful(null, objects);
    public static ApiResultDto<T> Successful(string? msg = null, params T[] objects) {
        return new ApiResultDto<T>(true, msg ?? "", objects);
    }
}